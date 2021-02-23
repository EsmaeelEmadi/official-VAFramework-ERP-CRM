﻿/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Movement Model
 * Class Used     : X_VAM_InventoryTransfer, DocAction(Interface)
 * Chronological Development
 * Veena         26-Oct-2009
 ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using System.Reflection;//Arpit

namespace VAdvantage.Model
{
    /// <summary>
    /// Inventory Movement Model
    /// </summary>
    public class MVAMInventoryTransfer : X_VAM_InventoryTransfer, DocAction
    {
        /**	Lines						*/
        private MVAMInvTrfLine[] _lines = null;
        /** Confirmations				*/
        private MVAMInvTrfConfirm[] _confirms = null;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;

        private string query = "";
        private Decimal? trxQty = 0;
        private bool isGetFroMVAMStorage = false;
        MVAAsset ast = null;
        private bool isAsset = false;

        MVABAccountBook acctSchema = null;
        MVAMProduct product1 = null;
        decimal currentCostPrice = 0;
        string conversionNotFoundInOut = "";
        string conversionNotFoundMovement = "";
        string conversionNotFoundMovement1 = "";
        MVAMVAMProductCostElement costElement = null;
        ValueNamePair pp = null;
        /**is container applicable */
        private bool isContainerApplicable = false;

        /** Reversal Indicator			*/
        public const String REVERSE_INDICATOR = "^";

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_InventoryTransfer_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMInventoryTransfer(Ctx ctx, int VAM_InventoryTransfer_ID, Trx trxName)
            : base(ctx, VAM_InventoryTransfer_ID, trxName)
        {
            if (VAM_InventoryTransfer_ID == 0)
            {
                //	SetVAB_DocTypes_ID (0);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetIsInTransit(false);
                SetMovementDate(new DateTime(CommonFunctions.CurrentTimeMillis()));	// @#Date@
                SetPosted(false);
                base.SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MVAMInventoryTransfer(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of lines</returns>
        public MVAMInvTrfLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MVAMInvTrfLine> list = new List<MVAMInvTrfLine>();
            String sql = "SELECT * FROM VAM_InvTrf_Line WHERE VAM_InventoryTransfer_ID=@moveid ORDER BY Line";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@moveid", GetVAM_InventoryTransfer_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MVAMInvTrfLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetLines", e);
            }

            _lines = new MVAMInvTrfLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Confirmations
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of confirmations</returns>
        public MVAMInvTrfConfirm[] GetConfirmations(Boolean requery)
        {
            if (_confirms != null && !requery)
                return _confirms;

            List<MVAMInvTrfConfirm> list = new List<MVAMInvTrfConfirm>();
            String sql = "SELECT * FROM VAM_InvTrf_Confirm WHERE VAM_InventoryTransfer_ID=@moveid";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@moveid", GetVAM_InventoryTransfer_ID());

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MVAMInvTrfConfirm(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetConfirmations", e);
            }

            _confirms = new MVAMInvTrfConfirm[list.Count];
            _confirms = list.ToArray();
            return _confirms;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>file if success</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, GetVAB_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetVAB_DocTypes_ID() == 0)
            {
                MVABDocTypes[] types = MVABDocTypes.GetOfDocBaseType(GetCtx(), MVABMasterDocType.DOCBASETYPE_MATERIALMOVEMENT);
                if (types.Length > 0)	//	Get first
                    SetVAB_DocTypes_ID(types[0].GetVAB_DocTypes_ID());
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @VAB_DocTypes_ID@"));
                    return false;
                }
            }

            // when we have record on movement line - then we can't change warehouse
            if (!newRecord && Is_ValueChanged("VAM_Warehouse_ID"))
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_InvTrf_Line WHERE VAM_InventoryTransfer_ID = " + GetVAM_InventoryTransfer_ID(), null, Get_Trx())) > 0)
                {
                    log.SaveError("VIS_ToWarehouseCantChange", "");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Set Processed.
        ///	Propergate to Lines/Taxes
        /// </summary>
        /// <param name="processed">processed</param>
        public void SetProcessed(Boolean processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE VAM_InvTrf_Line SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE VAM_InventoryTransfer_ID=" + GetVAM_InventoryTransfer_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine("Processed=" + processed + " - Lines=" + noLine);
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public Boolean ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());

            //	Std Period open?
            if (!MVABYearPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetVAF_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate(), GetVAF_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MVAMInvTrfLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts

            /* nnayak - Bug 1750251 : check material policy and update storage
               at the line level in completeIt()*/
            //checkMaterialPolicy();

            if (isContainerApplicable && IsReversal())
            {
                // when we reverse record, and movement line having MoveFullContainer = true
                // system will check qty on transaction must be same as qty on movement line
                // if not matched, then we can not allow to user for its reverse, he need to make a new Movement for move container
                string sql = DBFunctionCollection.CheckContainerQty(GetVAM_InventoryTransfer_ID());
                string productName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (!string.IsNullOrEmpty(productName))
                {
                    // Qty Alraedy consumed from Container for Product are : 
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_QtyConsumed") + productName;
                    SetProcessMsg(_processMsg);
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //	Confirmation
            if (GetDescription() != null)
            {
                if (GetDescription().Substring(0, 3) != "{->")
                {
                    if (dt.IsInTransit())
                        CreateConfirmation();
                }
            }
            else
            {
                if (dt.IsInTransit())
                    CreateConfirmation();
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Create Movement Confirmation
        /// </summary>
        private void CreateConfirmation()
        {
            MVAMInvTrfConfirm[] confirmations = GetConfirmations(false);
            if (confirmations.Length > 0)
                return;

            //	Create Confirmation
            MVAMInvTrfConfirm.Create(this, false);
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            #region[Prevent from completing, If on hand quantity of Product not available as per qty entered at line and Disallow negative is true at Warehouse. By Sukhwinder on 22 Dec, 2017. Only if DTD001 Module Installed.]
            if (Env.IsModuleInstalled("DTD001_"))
            {
                string sql = "";
                sql = "SELECT ISDISALLOWNEGATIVEINV FROM VAM_Warehouse WHERE VAM_Warehouse_ID = " + Util.GetValueOfInt(GetDTD001_MWarehouseSource_ID());
                string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
                int[] movementLine = MVAMInvInOutLine.GetAllIDs("VAM_InvTrf_Line", "VAM_InventoryTransfer_ID = " + GetVAM_InventoryTransfer_ID(), Get_TrxName());
                if (disallow.ToUpper() == "Y")
                {
                    int VAM_Locator_id = 0;
                    int VAM_Product_id = 0;
                    StringBuilder products = new StringBuilder();
                    StringBuilder locators = new StringBuilder();
                    bool check = false;
                    for (int i = 0; i < movementLine.Length; i++)
                    {
                        MVAMInvTrfLine mmLine = new MVAMInvTrfLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        //MVAMInvInOutLine iol = new MVAMInvInOutLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        VAM_Locator_id = Util.GetValueOfInt(mmLine.GetVAM_Locator_ID());
                        VAM_Product_id = Util.GetValueOfInt(mmLine.GetVAM_Product_ID());


                        sql = "SELECT VAM_PFeature_Set_ID FROM VAM_Product WHERE VAM_Product_ID = " + VAM_Product_id;
                        int VAM_ProductFeature_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        if (VAM_ProductFeature_ID == 0)
                        {
                            if (!isContainerApplicable)
                            {
                                sql = "SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id;
                            }
                            else
                            {
                                sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, 
                        t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                   " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + mmLine.GetVAM_Locator_ID() +
                                   " AND t.VAM_Product_ID = " + mmLine.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + mmLine.GetVAM_PFeature_SetInstance_ID() +
                                   " AND NVL(t.VAM_ProductContainer_ID, 0) = " + mmLine.GetVAM_ProductContainer_ID();
                            }
                            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            int qtyToMove = Util.GetValueOfInt(mmLine.GetMovementQty());
                            if (qty < qtyToMove)
                            {
                                check = true;
                                products.Append(VAM_Product_id + ", ");
                                locators.Append(VAM_Locator_id + ", ");
                                continue;
                            }
                        }
                        else
                        {
                            if (!isContainerApplicable)
                            {
                                sql = "SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + mmLine.GetVAM_PFeature_SetInstance_ID();
                            }
                            else
                            {
                                sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, 
                        t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                 " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + mmLine.GetVAM_Locator_ID() +
                                 " AND t.VAM_Product_ID = " + mmLine.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + mmLine.GetVAM_PFeature_SetInstance_ID() +
                                 " AND NVL(t.VAM_ProductContainer_ID, 0) = " + mmLine.GetVAM_ProductContainer_ID();
                            }
                            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            int qtyToMove = Util.GetValueOfInt(mmLine.GetMovementQty());
                            if (qty < qtyToMove)
                            {
                                check = true;
                                products.Append(VAM_Product_id + ",");
                                locators.Append(VAM_Locator_id + ",");
                                continue;
                            }
                        }
                    }

                    if (check)
                    {
                        sql = DBFunctionCollection.ConcatinateListOfLocators(locators.ToString());
                        string loc = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

                        sql = DBFunctionCollection.ConcatinateListOfProducts(products.ToString());
                        string prod = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));

                        _processMsg = Msg.GetMsg(Env.GetCtx(), "VIS_InsufficientQuantityFor") + prod + Msg.GetMsg(Env.GetCtx(), "VIS_OnLocators") + loc;
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }

                // SI_0750 If difference between the requisition qty and delivered qty is -ve or zero. system should not allow to complete inventory move and internal use.
                if (!IsReversal())
                {
                    StringBuilder delReq = new StringBuilder();
                    bool delivered = false;
                    for (int i = 0; i < movementLine.Length; i++)
                    {
                        MVAMInvTrfLine mmLine = new MVAMInvTrfLine(Env.GetCtx(), movementLine[i], Get_TrxName());
                        if (mmLine.GetVAM_RequisitionLine_ID() != 0)
                        {
                            MVAMRequisitionLine reqLine = new MVAMRequisitionLine(GetCtx(), mmLine.GetVAM_RequisitionLine_ID(), Get_TrxName());
                            if (reqLine.GetQty() - reqLine.GetDTD001_DeliveredQty() <= 0)
                            {
                                delivered = true;
                                delReq.Append(mmLine.GetVAM_RequisitionLine_ID() + ",");
                            }
                        }
                    }

                    if (delivered)
                    {
                        sql = DBFunctionCollection.ConcatnatedListOfRequisition(delReq.ToString());
                        string req = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));

                        _processMsg = Msg.GetMsg(Env.GetCtx(), "RequisitionAlreadyDone") + ": " + req;
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }
            }
            #endregion

            List<ParentChildContainer> parentChildContainer = null;
            if (isContainerApplicable)
            {
                #region Check Container existence  in specified Warehouse and Locator
                // during completion - system will verify -- container avialble on line is belongs to same warehouse and locator
                // if not then not to complete this record

                // For From Container
                string sqlContainerExistence = DBFunctionCollection.MovementContainerNotMatched(GetVAM_InventoryTransfer_ID()); ;
                string containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sqlContainerExistence, null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFound") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }

                // To Container
                sqlContainerExistence = DBFunctionCollection.MovementContainerToNotMatched(GetVAM_InventoryTransfer_ID());
                containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sqlContainerExistence, null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFoundTo") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion

                //If User try to complete the Transactions if Movement Date is lesser than Last MovementDate on Product Container then we need to stop that transaction to Complete.
                #region Check MovementDate and Last Inventory Date Neha 31 Aug,2018

                string _qry = DBFunctionCollection.MovementContainerNotAvailable(GetVAM_InventoryTransfer_ID());

                string misMatch = Util.GetValueOfString(DB.ExecuteScalar(_qry, null, Get_Trx()));
                if (!String.IsNullOrEmpty(misMatch))
                {
                    SetProcessMsg(misMatch + Msg.GetMsg(GetCtx(), "VIS_ContainerNotAvailable"));
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion


                // when user try to move full container, system will check qty on movement line and on Container.
                // if not matched, then not able to complete this record
                if (!IsReversal() && !IsMoveFullContainerPossible(GetVAM_InventoryTransfer_ID()))
                {
                    return DocActionVariables.STATUS_INVALID;
                }

                parentChildContainer = new List<ParentChildContainer>();
                // during full move container - count no of Products on movement line in container must be equal to no. of Products on Tansaction for the same container.
                // even we compare Qty on movementline and transaction 
                if (!IsReversal())
                {
                    parentChildContainer = ProductChildContainer(GetVAM_InventoryTransfer_ID());

                    string mismatch = IsMoveContainerProductCount(GetVAM_InventoryTransfer_ID(), parentChildContainer);
                    if (!String.IsNullOrEmpty(mismatch))
                    {
                        // Qty in container has been increased/decreased  for Product : 
                        SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_MisMatchProduct") + mismatch);
                        return DocActionVariables.STATUS_INVALID;
                    }
                }

                //if (!IsReversal() && !ParentMoveFromPath(GetVAM_InventoryTransfer_ID()))
                //{
                //    return DocActionVariables.STATUS_INVALID;
                //}

                //if (!IsReversal() && !ParentMoveToPath(GetVAM_InventoryTransfer_ID()))
                //{
                //    return DocActionVariables.STATUS_INVALID;
                //}
            }

            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            // JID_1290: Set the document number from completede document sequence after completed (if needed)
            SetCompletedDocumentNo();

            // To check weather future date records are available in Transaction window
            // this check implement after "SetCompletedDocumentNo" function, because this function overwrit movement date
            _processMsg = MVAMInvTrx.CheckFutureDateRecord(GetMovementDate(), Get_TableName(), GetVAM_InventoryTransfer_ID(), Get_Trx());
            if (!string.IsNullOrEmpty(_processMsg))
            {
                return DocActionVariables.STATUS_INVALID;
            }

            // check column name new 12 jan 0 vikas
            int _count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT Count(*) FROM VAF_Column WHERE columnname = 'DTD001_SourceReserve' "));

            //	Outstanding (not processed) Incoming Confirmations ?
            MVAMInvTrfConfirm[] confirmations = GetConfirmations(true);
            for (int i = 0; i < confirmations.Length; i++)
            {
                #region Outstanding (not processed) Incoming Confirmations
                MVAMInvTrfConfirm confirm = confirmations[i];
                if (!confirm.IsProcessed())
                {
                    //SI_0630.1 :  Check status of confimation, if voided then we need to create confirmation again
                    if (confirm.GetDocStatus() == DOCSTATUS_Voided)
                    {
                        // compare confirmation record count, if it is not last record then need to check another record
                        // else create move confirmation again
                        if ((i + 1) != confirmations.Length)
                            continue;
                        else
                        {
                            //SI_0630.1 : create confirmation
                            confirm = MVAMInvTrfConfirm.Create(this, false);
                        }
                        _processMsg = "Open: @VAM_InvTrf_Confirm_ID@ - "
                            + confirm.GetDocumentNo();
                        SetProcessMsg(_processMsg);
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                    else if (confirm.GetDocStatus() != DOCSTATUS_Voided ||
                             confirm.GetDocStatus() != DOCSTATUS_Closed ||
                             confirm.GetDocStatus() != DOCSTATUS_Completed)
                    {
                        //SI_0630.2 : display message on UI "Confirmation is already pending for approval"
                        //_processMsg = Msg.GetMsg(GetCtx(), "VIS_ConfirmationPending") + confirm.GetDocumentNo();
                        _processMsg = "Open: @VAM_InvTrf_Confirm_ID@ - " + confirm.GetDocumentNo();
                        SetProcessMsg(_processMsg);
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }
                // if found any Processed record then break the loop
                // because confirmation found in completed stage
                break;
                #endregion
            }

            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());

            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MVAFClient client = MVAFClient.Get(GetCtx(), GetVAF_Client_ID());

            //int countVA024 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE ISACTIVE = 'Y' AND  PREFIX='VA024_'"));
            //int tableId = 0;
            //try
            //{
            //    query = @"SELECT VAF_TABLEVIEW_ID  FROM VAF_TABLEVIEW WHERE tablename LIKE 'VA024_T_ObsoleteInventory' AND IsActive = 'Y'";
            //    tableId = Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx()));
            //}
            //catch { }

            //query = "SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA203_'";
            int countKarminati = Env.IsModuleInstalled("VA203_") ? 1 : 0;

            if (isContainerApplicable)
            {
                StringBuilder sqlContainer = new StringBuilder();
                // Update Last Inventory date on To Container in case of full container
                sqlContainer.Append("UPDATE VAM_ProductContainer SET DateLastInventory = " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                    @" WHERE VAM_ProductContainer_ID IN 
                                (SELECT DISTINCT NVL(Ref_VAM_ProductContainerTo_ID, 0) FROM VAM_InvTrf_Line WHERE IsActive = 'Y' 
                                  AND MoveFullContainer = 'Y' AND VAM_InventoryTransfer_ID =  " + GetVAM_InventoryTransfer_ID() + ") ");
                DB.ExecuteQuery(sqlContainer.ToString(), null, Get_Trx());

                // need to update Organization / warehouse / locator / DateLastInventory / Parent container reference (if any)
                if (!IsReversal())
                {
                    UpdateContainerLocation(GetVAM_InventoryTransfer_ID(), parentChildContainer);
                }
            }

            MVAMInvTrfLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvTrfLine line = lines[i];

                /* nnayak - Bug 1750251 : If you have multiple lines for the same product
                in the same Sales Order, or if the generate shipment process was generating
                multiple shipments for the same product in the same run, the first layer 
                was Getting consumed by all the shipments. As a result, the first layer had
                negative Inventory even though there were other positive layers. */
                // Ignore the Material Policy when is Reverse Correction
                if (!IsReversal())
                {
                    CheckMaterialPolicy(line);
                }

                MVAMInvTrx trxFrom = null;
                if (line.GetVAM_PFeature_SetInstance_ID() == 0 || line.GetVAM_PFeature_SetInstance_ID() != 0)
                {
                    MVAMInvTrfLineMP[] mas = MVAMInvTrfLineMP.Get(GetCtx(),
                        line.GetVAM_InvTrf_Line_ID(), Get_TrxName());
                    for (int j = 0; j < mas.Length; j++)
                    {
                        Decimal? containerCurrentQty = 0;
                        MVAMInvTrfLineMP ma = mas[j];
                        //
                        MVAMStorage storageFrom = MVAMStorage.Get(GetCtx(), line.GetVAM_Locator_ID(),
                            line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                        if (storageFrom == null)
                            storageFrom = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_Locator_ID(),
                                line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                        //
                        MVAMStorage storageTo = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(),
                            line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                        if (storageTo == null)
                            storageTo = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(),
                                line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                        //
                        // When Locator From and Locator To are different, then need to take impacts on Storage on hand qty
                        if (line.GetVAM_Locator_ID() != line.GetVAM_LocatorTo_ID())
                        {
                            storageFrom.SetQtyOnHand(Decimal.Subtract(storageFrom.GetQtyOnHand(), ma.GetMovementQty()));
                        }
                        if (line.GetVAM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 &&
                        {
                            storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), ma.GetMovementQty()));
                        }
                        if (!storageFrom.Save(Get_TrxName()))
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Storage From not updated (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //
                        // When Locator From and Locator To are different, then need to take impacts on Storage onhand qty
                        if (line.GetVAM_Locator_ID() != line.GetVAM_LocatorTo_ID())
                        {
                            storageTo.SetQtyOnHand(Decimal.Add(storageTo.GetQtyOnHand(), ma.GetMovementQty()));
                        }
                        if (!storageTo.Save(Get_TrxName()))
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Storage To not updated (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        #region Update Transaction / Future Date entry for From Locator
                        // Done to Update Current Qty at Transaction
                        Decimal? trxQty = 0;
                        //                        MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
                        //                        int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
                        //                        isGetFroMVAMStorage = false;
                        //                        if (attribSet_ID > 0)
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                        //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                        //                                    + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), true, line.GetVAM_Locator_ID());
                        //                                isGetFroMVAMStorage = true;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                        //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                        //                                     + " AND VAM_PFeature_SetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), false, line.GetVAM_Locator_ID());
                        //                                isGetFroMVAMStorage = true;
                        //                            }
                        //                        }
                        //                        if (!isGetFroMVAMStorage)
                        //                        {
                        //                            trxQty = GetProductQtyFroMVAMStorage(line, line.GetVAM_Locator_ID());
                        //                        }

                        query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + line.GetVAM_Locator_ID() +
                           " AND t.VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + line.GetVAM_PFeature_SetInstance_ID();
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                        // get container Current qty from transaction
                        if (isContainerApplicable && line.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            containerCurrentQty = GetContainerQtyFroMVAMInvTrx(line, GetMovementDate(), line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID());
                        }

                        //
                        trxFrom = new MVAMInvTrx(GetCtx(), line.GetVAF_Org_ID(),
                            MVAMInvTrx.MOVEMENTTYPE_MovementFrom,
                            line.GetVAM_Locator_ID(), line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                            Decimal.Negate(ma.GetMovementQty()), GetMovementDate(), Get_TrxName());
                        trxFrom.SetVAM_InvTrf_Line_ID(line.GetVAM_InvTrf_Line_ID());
                        trxFrom.SetCurrentQty(trxQty + Decimal.Negate(ma.GetMovementQty()));
                        // set Material Policy Date
                        trxFrom.SetMMPolicyDate(ma.GetMMPolicyDate());
                        if (isContainerApplicable && trxFrom.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            trxFrom.SetVAM_ProductContainer_ID(line.GetVAM_ProductContainer_ID());
                            // update containr or withot container qty Current Qty 
                            trxFrom.SetContainerCurrentQty(containerCurrentQty + Decimal.Negate(ma.GetMovementQty()));
                        }
                        if (!trxFrom.Save())
                        {
                            Get_TrxName().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Transaction From not inserted (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        if (isContainerApplicable && trxFrom.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            String errorMessage = UpdateTransactionContainer(line, trxFrom, trxQty.Value + Decimal.Negate(ma.GetMovementQty()), line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID());
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                SetProcessMsg(errorMessage);
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            UpdateTransaction(line, trxFrom, trxQty.Value + Decimal.Negate(ma.GetMovementQty()), line.GetVAM_Locator_ID());
                        }
                        //UpdateCurrentRecord(line, trxFrom, Decimal.Negate(ma.GetMovementQty()), line.GetVAM_Locator_ID());
                        #endregion
                        /*************************************************************************************************/
                        Tuple<String, String, String> mInfo = null;
                        if (Env.HasModulePrefix("DTD001_", out mInfo))
                        {
                            if (line.GetVAM_RequisitionLine_ID() > 0)
                            {
                                #region Requisition Case handled
                                decimal reverseRequisitionQty = 0;
                                MVAMRequisitionLine reqLine = new MVAMRequisitionLine(GetCtx(), line.GetVAM_RequisitionLine_ID(), Get_Trx());
                                MVAMRequisition req = new MVAMRequisition(GetCtx(), reqLine.GetVAM_Requisition_ID(), Get_Trx());        // Trx used to handle query stuck problem

                                if (!IsReversal())
                                {
                                    // ((qty Request) - (qty delivered)) >= (Attribute qty) then reduce (Attribute qty) from Requisition Ordered / Reserved qty
                                    if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= ma.GetMovementQty())
                                    {
                                        reverseRequisitionQty = ma.GetMovementQty();
                                    }
                                    // reduce diff ((qty Request) - (qty delivered)) on Requisition Ordered / Reserved qty
                                    else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < ma.GetMovementQty())
                                    {
                                        // when deleiverd is greater than requistion qty then make it as ZERO - no impacts goes to Requisition Ordered / Reserved qty
                                        if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                            reverseRequisitionQty = 0;
                                        else
                                            reverseRequisitionQty = Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty());
                                    }
                                    DB.ExecuteQuery("UPDATE VAM_InvTrf_Line SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                              @" WHERE VAM_InvTrf_Line_ID = " + line.GetVAM_InvTrf_Line_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    // during reversal -- only actual Requisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                                    reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());

                                    // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                                    line.SetActualReqReserved(0);
                                    //reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                                }

                                reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), ma.GetMovementQty()));
                                reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), ma.GetMovementQty()));
                                if (!reqLine.Save())
                                {
                                    Get_Trx().Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = "Requisition Line not updated. " + pp.GetName();
                                    else
                                        _processMsg = "Requisition Line not updated";
                                    return DocActionVariables.STATUS_INVALID;
                                }

                                if (_count > 0)
                                {
                                    // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                                    int ResLocator_ID = 0;
                                    if (reqLine.Get_ColumnIndex("ReserveLocator_ID") > 0)
                                    {
                                        ResLocator_ID = reqLine.GetReserveLocator_ID();
                                    }
                                    else
                                    {
                                        ResLocator_ID = line.GetVAM_Locator_ID();
                                    }
                                    if (ResLocator_ID > 0 && req.GetDocStatus() != "CL")
                                    {
                                        // JID_0657: Requistion is without ASI but on move selected the ASI system is minus the Reserved qty from ASI field but not removing the reserved qty without ASI
                                        MVAMStorage ordStorage = MVAMStorage.Get(GetCtx(), ResLocator_ID, line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                                        ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                        if (!ordStorage.Save(Get_TrxName()))
                                        {
                                            Get_TrxName().Rollback();
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                _processMsg = pp.GetName();
                                            else
                                                _processMsg = "Storage From not updated (MA)";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                }

                                // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                                int OrdLocator_ID = 0;
                                if (reqLine.Get_ColumnIndex("OrderLocator_ID") > 0)
                                {
                                    OrdLocator_ID = reqLine.GetOrderLocator_ID();
                                }
                                else
                                {
                                    OrdLocator_ID = line.GetVAM_LocatorTo_ID();
                                }
                                if (OrdLocator_ID > 0)
                                {
                                    if (req.GetDocStatus() != "CL" && countKarminati > 0)
                                    {
                                        MVAMStorage newsg = MVAMStorage.Get(GetCtx(), OrdLocator_ID, line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                        //if (newsg == null)
                                        //{
                                        //    newsg = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                        //}
                                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                                        if (!newsg.Save())
                                        {
                                            Get_Trx().Rollback();               //Arpit
                                            _processMsg = "Storage Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                    else if (req.GetDocStatus() != "CL")
                                    {
                                        MVAMStorage newsg = MVAMStorage.Get(GetCtx(), OrdLocator_ID, line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                                        if (!newsg.Save(Get_Trx()))
                                        {
                                            Get_Trx().Rollback(); //Arpit
                                            _processMsg = "Storage Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                }
                                #endregion
                            }
                            #region Asset Work
                            string sql = "SELECT DTD001_ISCONSUMABLE FROM VAM_Product WHERE VAM_Product_ID=" + line.GetVAM_Product_ID();
                            if (Util.GetValueOfString(DB.ExecuteScalar(sql)) == "N")
                            {

                                //sql = "SELECT pcat.VAA_AssetGroup_ID FROM VAM_Product prd INNER JOIN VAM_ProductCategory pcat ON prd.VAM_ProductCategory_ID=pcat.VAM_ProductCategory_ID WHERE prd.VAM_Product_ID=" + line.GetVAM_Product_ID();
                                //if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)

                                // Check Asset ID instead of Asset Group to consider Asset Movement.
                                if (line.GetA_Asset_ID() > 0)
                                {
                                    isAsset = true;
                                }
                                else
                                {
                                    isAsset = false;
                                }
                            }
                            else
                            {
                                isAsset = false;
                            }

                            if (isAsset == true)
                            {
                                DataSet DSReq = null;
                                if (line.GetVAM_RequisitionLine_ID() > 0)
                                {
                                    string NEWStr = "SELECT req.VAB_BusinessPartner_id FROM VAM_RequisitionLine rqln INNER JOIN VAM_Requisition req  ON req.VAM_Requisition_id  = rqln.VAM_Requisition_id  WHERE rqln.VAM_RequisitionLine_id=" + line.GetVAM_RequisitionLine_ID();
                                    DSReq = DB.ExecuteDataset(NEWStr, null, null);
                                }
                                if (line.GetA_Asset_ID() > 0)
                                {
                                    ast = new MVAAsset(GetCtx(), line.GetA_Asset_ID(), Get_Trx());
                                    Tuple<String, String, String> aInfo = null;
                                    if (Env.HasModulePrefix("VAFAM_", out aInfo))
                                    {
                                        MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                                        ast.CopyTo(aHist);
                                        aHist.SetA_Asset_ID(line.GetA_Asset_ID());
                                        if (!aHist.Save() && !ast.Save())
                                        {
                                            _processMsg = "Asset History Not Updated";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                    ast.SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());
                                    if (DSReq != null)
                                    {
                                        if (DSReq.Tables[0].Rows.Count > 0)
                                        {
                                            ast.SetVAB_BusinessPartner_ID(Util.GetValueOfInt(DSReq.Tables[0].Rows[0]["VAB_BusinessPartner_id"]));
                                        }
                                    }

                                    ast.SetVAM_Locator_ID(line.GetVAM_LocatorTo_ID());
                                    ast.Save();
                                }
                                else
                                {
                                    Get_TrxName().Rollback();               //Arpit
                                    _processMsg = "Asset Not Selected For Movement Line";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            #endregion
                        }

                        #region Update Transaction / Future Date entry for To Locator
                        // Done to Update Current Qty at Transaction Decimal? trxQty = 0;
                        //                        pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
                        //                        attribSet_ID = pro.GetVAM_PFeature_Set_ID();
                        //                        isGetFroMVAMStorage = false;
                        //                        if (attribSet_ID > 0)
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                        //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID()
                        //                                    + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), true, line.GetVAM_LocatorTo_ID());
                        //                                isGetFroMVAMStorage = true;
                        //                            }
                        //                        }
                        //                        else
                        //                        {
                        //                            query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                        //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID()
                        //                                     + " AND VAM_PFeature_SetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        //                            if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                        //                            {
                        //                                trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), false, line.GetVAM_LocatorTo_ID());
                        //                                isGetFroMVAMStorage = true;
                        //                            }
                        //                        }
                        //                        if (!isGetFroMVAMStorage)
                        //                        {
                        //                            trxQty = GetProductQtyFroMVAMStorage(line, line.GetVAM_LocatorTo_ID());
                        //                        }

                        query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID() +
                           " AND t.VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + line.GetVAM_PFeature_SetInstance_ID();
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                        // get container Current qty from transaction
                        if (isContainerApplicable && line.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // when move full container, then check from container qty else check qty in To Container
                            containerCurrentQty = GetContainerQtyFroMVAMInvTrx(line, GetMovementDate(), line.GetVAM_LocatorTo_ID(),
                                line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                        }

                        // Done to Update Current Qty at Transaction
                        // create transaction entry with To Org
                        MVAMInvTrx trxTo = new MVAMInvTrx(GetCtx(), line.GetVAF_Org_ID(),
                            MVAMInvTrx.MOVEMENTTYPE_MovementTo,
                            line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                            ma.GetMovementQty(), GetMovementDate(), Get_TrxName());
                        trxTo.SetVAM_InvTrf_Line_ID(line.GetVAM_InvTrf_Line_ID());
                        trxTo.SetCurrentQty(trxQty.Value + ma.GetMovementQty());
                        // set Material Policy Date
                        trxTo.SetMMPolicyDate(ma.GetMMPolicyDate());
                        if (isContainerApplicable && trxTo.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            // when move full container, then check from container qty else check qty in To Container
                            trxTo.SetVAM_ProductContainer_ID(line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                            // update containr or withot container qty Current Qty 
                            trxTo.SetContainerCurrentQty(containerCurrentQty + ma.GetMovementQty());
                        }
                        if (!trxTo.Save())
                        {
                            Get_Trx().Rollback();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Transaction To not inserted (MA)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        if (isContainerApplicable && trxTo.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // when move full container, then check from container qty else check qty in To Container
                            String errorMessage = UpdateTransactionContainer(line, trxTo, trxQty.Value + ma.GetMovementQty(), line.GetVAM_LocatorTo_ID(),
                                 line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                SetProcessMsg(errorMessage);
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            UpdateTransaction(line, trxTo, trxQty.Value + ma.GetMovementQty(), line.GetVAM_LocatorTo_ID());
                        }
                        //UpdateCurrentRecord(line, trxTo, ma.GetMovementQty(), line.GetVAM_LocatorTo_ID());
                        #endregion
                    }
                }
                //	Fallback - We have ASI
                if (trxFrom == null)
                {
                    #region WHEN ASI available on line -- when Data on Attribute Tab not found
                    Decimal? containerCurrentQty = 0;
                    MVAMRequisitionLine reqLine = null;
                    MVAMRequisition req = null;
                    decimal reverseRequisitionQty = 0;
                    if (line.GetVAM_RequisitionLine_ID() > 0)
                    {
                        #region Requisition Case Handling
                        reqLine = new MVAMRequisitionLine(GetCtx(), line.GetVAM_RequisitionLine_ID(), Get_Trx());
                        req = new MVAMRequisition(GetCtx(), reqLine.GetVAM_Requisition_ID(), Get_Trx());         // Trx used to handle query stuck problem
                        if (!IsReversal())
                        {
                            // ((qty Request) - (qty delivered)) >= (movement qty) then reduce (movement qty) from Requisition Ordered / Reserved qty
                            if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) >= line.GetMovementQty())
                            {
                                reverseRequisitionQty = line.GetMovementQty();
                            }
                            // reduce diff ((qty Request) - (qty delivered)) on Requisition Ordered / Reserved qty
                            else if (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()) < line.GetMovementQty())
                            {
                                // when delivered > request qty then no impact on Requisition Ordered / Reserved qty
                                if (reqLine.GetDTD001_DeliveredQty() >= reqLine.GetQty())
                                    reverseRequisitionQty = 0;
                                else
                                    reverseRequisitionQty = (Decimal.Subtract(reqLine.GetQty(), reqLine.GetDTD001_DeliveredQty()));
                            }
                            DB.ExecuteQuery("UPDATE VAM_InvTrf_Line SET ActualReqReserved = NVL(ActualReqReserved , 0) + " + reverseRequisitionQty +
                                          @" WHERE VAM_InvTrf_Line_ID = " + line.GetVAM_InvTrf_Line_ID(), null, Get_Trx());
                        }
                        else
                        {
                            // during reversal -- only actual equisition reserver qty should be impacted on Requisition Ordered or Reserved qty
                            reverseRequisitionQty = Decimal.Negate(line.GetActualReqReserved());
                            // set actual requisition reserved as ZERO -- bcz for next iteration we get ZERO - no impacts goes 
                            line.SetActualReqReserved(0);
                        }

                        if (Env.IsModuleInstalled("DTD001_"))
                        {
                            reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), line.GetMovementQty()));
                            if (line.GetVAM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 && 
                            {
                                reqLine.SetDTD001_ReservedQty(Decimal.Subtract(reqLine.GetDTD001_ReservedQty(), line.GetMovementQty()));
                            }
                            reqLine.Save();
                        }
                        #endregion
                    }
                    MVAMStorage storageFrom = MVAMStorage.Get(GetCtx(), line.GetVAM_Locator_ID(),
                        line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                    if (storageFrom == null)
                        storageFrom = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_Locator_ID(),
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                    if (line.GetVAM_RequisitionLine_ID() > 0) // line.GetMovementQty() > 0 && 
                    {
                        storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), line.GetMovementQty()));
                    }

                    MVAMStorage storageTo = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(),
                        line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstanceTo_ID(), Get_TrxName());
                    if (storageTo == null)
                        storageTo = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(),
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstanceTo_ID(), Get_TrxName());

                    // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                    if (Env.IsModuleInstalled("DTD001_") && line.GetVAM_RequisitionLine_ID() > 0)
                    {
                        #region Requisition cases handling
                        //SI_0657: Issue
                        if (_count > 0)
                        {
                            // SI_0682_2 : On completion of Inventory move system is not removing the Requisition Ordered qty from the same locator.
                            int ResLocator_ID = 0;
                            if (reqLine.Get_ColumnIndex("ReserveLocator_ID") > 0)
                            {
                                ResLocator_ID = reqLine.GetReserveLocator_ID();
                            }
                            else
                            {
                                ResLocator_ID = line.GetVAM_LocatorTo_ID();
                            }
                            if (ResLocator_ID > 0 && req.GetDocStatus() != "CL")
                            {
                                // JID_0657: Requistion is without ASI but on move selected the ASI system is minus the Reserved qty from ASI field but not removing the reserved qty without ASI
                                MVAMStorage ordStorage = MVAMStorage.Get(GetCtx(), ResLocator_ID, line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_TrxName());
                                ordStorage.SetDTD001_SourceReserve(Decimal.Subtract(ordStorage.GetDTD001_SourceReserve(), reverseRequisitionQty));
                                if (!ordStorage.Save(Get_TrxName()))
                                {
                                    Get_TrxName().Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = pp.GetName();
                                    else
                                        _processMsg = "Storage not updated";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                        }
                        //                        
                        int OrdLocator_ID = 0;
                        if (reqLine.Get_ColumnIndex("OrderLocator_ID") > 0)
                        {
                            OrdLocator_ID = reqLine.GetOrderLocator_ID();
                        }
                        else
                        {
                            OrdLocator_ID = line.GetVAM_Locator_ID();
                        }
                        if (OrdLocator_ID > 0 && req.GetDocStatus() != "CL")
                        {
                            #region Commented
                            //Update product Qty at storage and Checks Product have Attribute Set Or Not.
                            //MVAMProduct newproduct = new MVAMProduct(GetCtx(), line.GetVAM_Product_ID(), Get_Trx());
                            //if (countKarminati > 0 && line.GetVAM_RequisitionLine_ID() > 0)
                            //{
                            //    if ((newproduct.GetVAM_PFeature_Set_ID() != null) && (newproduct.GetVAM_PFeature_Set_ID() != 0))
                            //    {
                            //        MVAMStorage newsg = null;
                            //        if (reqLine != null)
                            //        {
                            //            newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        else
                            //        {
                            //            newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        if (newsg != null && req != null)
                            //        {
                            //            Tuple<String, String, String> aInfo = null;
                            //            if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //            {
                            //                if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //                {
                            //                    if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //                else if (Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //                {
                            //                    if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        MVAMStorage newsg = null;
                            //        if (reqLine != null)
                            //        {
                            //            newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        else
                            //        {
                            //            newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            if (newsg == null)
                            //            {
                            //                newsg = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //            }
                            //        }
                            //        if (newsg != null && req != null)
                            //        {
                            //            Tuple<String, String, String> aInfo = null;
                            //            if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //            {
                            //                if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //                {
                            //                    if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //                else if (Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //                {
                            //                    if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                    {
                            //                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                    }
                            //                    if (!newsg.Save(Get_Trx()))
                            //                    {
                            //                        Get_Trx().Rollback();
                            //                        _processMsg = "Storage Not Updated";
                            //                        return DocActionVariables.STATUS_INVALID;
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //else if ((newproduct.GetVAM_PFeature_Set_ID() != null) && (newproduct.GetVAM_PFeature_Set_ID() != 0))
                            //{
                            //    MVAMStorage newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            //    if (newsg != null && req != null)
                            //    {
                            //        Tuple<String, String, String> aInfo = null;
                            //        if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //        {
                            //            if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //            {
                            //                if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //            else if (Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //            {
                            //                if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    MVAMStorage newsg = MVAMStorage.Get(GetCtx(), line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), true, Get_TrxName());
                            //    if (newsg != null && req != null)
                            //    {
                            //        Tuple<String, String, String> aInfo = null;
                            //        if (Env.HasModulePrefix("DTD001_", out aInfo))
                            //        {
                            //            if (newsg.GetDTD001_QtyReserved() != null && Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //            {
                            //                if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //            else if (Util.GetValueOfString(req.GetDocStatus()) != "CL")
                            //            {
                            //                if (line.GetVAM_RequisitionLine_ID() > 0)
                            //                {
                            //                    newsg.SetDTD001_QtyReserved(Decimal.Subtract(0, reverseRequisitionQty));
                            //                }
                            //                if (!newsg.Save(Get_Trx()))
                            //                {
                            //                    Get_Trx().Rollback();
                            //                    _processMsg = "Storage Not Updated";
                            //                    return DocActionVariables.STATUS_INVALID;
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion

                            MVAMStorage newsg = MVAMStorage.Get(GetCtx(), OrdLocator_ID, line.GetVAM_Product_ID(), reqLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                            newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), reverseRequisitionQty));

                            if (!newsg.Save(Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                _processMsg = "Storage Not Updated";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        #endregion
                    }

                    // When Locator From and Locator To are different, no need to take impacts on Storage
                    if (line.GetVAM_Locator_ID() != line.GetVAM_LocatorTo_ID())
                    {
                        storageFrom.SetQtyOnHand(Decimal.Subtract(storageFrom.GetQtyOnHand(), line.GetMovementQty()));
                    }
                    if (!storageFrom.Save(Get_TrxName()))
                    {
                        Get_TrxName().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Storage From not updated";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //
                    // When Locator From and Locator To are different, no need to take impacts on Storage
                    if (line.GetVAM_Locator_ID() != line.GetVAM_LocatorTo_ID())
                    {
                        storageTo.SetQtyOnHand(Decimal.Add(storageTo.GetQtyOnHand(), line.GetMovementQty()));
                    }
                    if (!storageTo.Save(Get_TrxName()))
                    {
                        Get_TrxName().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Storage To not updated";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    /***************************************************/
                    #region Asset Work
                    Tuple<String, String, String> iInfo = null;
                    if (Env.HasModulePrefix("DTD001_", out iInfo))
                    {
                        string sql = "SELECT DTD001_ISCONSUMABLE FROM VAM_Product WHERE VAM_Product_ID=" + line.GetVAM_Product_ID();
                        if (Util.GetValueOfString(DB.ExecuteScalar(sql)) != "Y")
                        {
                            sql = "SELECT pcat.VAA_AssetGroup_ID FROM VAM_Product prd INNER JOIN VAM_ProductCategory pcat ON prd.VAM_ProductCategory_ID=pcat.VAM_ProductCategory_ID WHERE prd.VAM_Product_ID=" + line.GetVAM_Product_ID();
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
                            {
                                isAsset = true;
                            }
                            else
                            {
                                isAsset = false;
                            }
                        }

                        else
                        {
                            isAsset = false;
                        }

                        if (isAsset == true)
                        {
                            if (line.GetA_Asset_ID() > 0)
                            {
                                ast = new MVAAsset(GetCtx(), line.GetA_Asset_ID(), Get_Trx());
                                Tuple<String, String, String> aInfo = null;
                                if (Env.HasModulePrefix("VAFAM_", out aInfo))
                                {
                                    MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                                    ast.CopyTo(aHist);
                                    aHist.SetA_Asset_ID(line.GetA_Asset_ID());
                                    if (!aHist.Save() && !ast.Save())
                                    {
                                        _processMsg = "Asset History Not Updated";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                ast.SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());
                                ast.SetVAM_Locator_ID(line.GetVAM_LocatorTo_ID());
                                ast.Save();
                            }
                            else
                            {
                                Get_Trx().Rollback();// not to impact on Storare & Transaction of Product Move   ...Arpit
                                _processMsg = "Asset Not Selected For Movement Line";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                    }
                    #endregion
                    /********************************************************/

                    #region Update Transaction / Future Date entry for From Locator
                    // Done to Update Current Qty at Transaction
                    Decimal? trxQty = 0;
                    //                    MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
                    //                    int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
                    //                    isGetFroMVAMStorage = false;
                    //                    if (attribSet_ID > 0)
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                    //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                    //                                + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), true, line.GetVAM_Locator_ID());
                    //                            isGetFroMVAMStorage = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                    //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                    //                                      + " AND VAM_PFeature_SetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), false, line.GetVAM_Locator_ID());
                    //                            isGetFroMVAMStorage = true;
                    //                        }
                    //                    }
                    //                    if (!isGetFroMVAMStorage)
                    //                    {
                    //                        trxQty = GetProductQtyFroMVAMStorage(line, line.GetVAM_Locator_ID());
                    //                    }

                    query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + line.GetVAM_Locator_ID() +
                           " AND t.VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + line.GetVAM_PFeature_SetInstance_ID();
                    trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                    // get container Current qty from transaction
                    if (isContainerApplicable && line.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        containerCurrentQty = GetContainerQtyFroMVAMInvTrx(line, GetMovementDate(), line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID());
                    }

                    //
                    trxFrom = new MVAMInvTrx(GetCtx(), line.GetVAF_Org_ID(),
                        MVAMInvTrx.MOVEMENTTYPE_MovementFrom,
                        line.GetVAM_Locator_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                        Decimal.Negate(line.GetMovementQty()), GetMovementDate(), Get_TrxName());
                    trxFrom.SetVAM_InvTrf_Line_ID(line.GetVAM_InvTrf_Line_ID());
                    trxFrom.SetCurrentQty(trxQty + Decimal.Negate(line.GetMovementQty()));
                    // set Material Policy Date
                    trxFrom.SetMMPolicyDate(GetMovementDate());
                    if (isContainerApplicable && trxFrom.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        // Update Product Container on Transaction
                        trxFrom.SetVAM_ProductContainer_ID(line.GetVAM_ProductContainer_ID());
                        // update containr or withot container qty Current Qty 
                        trxFrom.SetContainerCurrentQty(containerCurrentQty + Decimal.Negate(line.GetMovementQty()));
                    }
                    if (!trxFrom.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Transaction From not inserted";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    if (isContainerApplicable && trxFrom.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        String errorMessage = UpdateTransactionContainer(line, trxFrom, trxQty.Value + Decimal.Negate(line.GetMovementQty()), line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID());
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            SetProcessMsg(errorMessage);
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        UpdateTransaction(line, trxFrom, trxQty.Value + Decimal.Negate(line.GetMovementQty()), line.GetVAM_Locator_ID());
                    }
                    //UpdateCurrentRecord(line, trxFrom, Decimal.Negate(line.GetMovementQty()), line.GetVAM_Locator_ID());
                    #endregion

                    #region Update Transaction / Future Date entry for To Locator
                    // Done to Update Current Qty at Transaction
                    //                    pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
                    //                    attribSet_ID = pro.GetVAM_PFeature_Set_ID();
                    //                    isGetFroMVAMStorage = false;
                    //                    if (attribSet_ID > 0)
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                    //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID()
                    //                                + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), true, line.GetVAM_LocatorTo_ID());
                    //                            isGetFroMVAMStorage = true;
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        query = @"SELECT COUNT(*)   FROM VAM_Inv_Trx
                    //                                    WHERE IsActive = 'Y' AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID()
                    //                                      + " AND VAM_PFeature_SetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                    //                        if (Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx())) > 0)
                    //                        {
                    //                            trxQty = GetProductQtyFroMVAMInvTrx(line, GetMovementDate(), false, line.GetVAM_LocatorTo_ID());
                    //                            isGetFroMVAMStorage = true;
                    //                        }
                    //                    }
                    //                    if (!isGetFroMVAMStorage)
                    //                    {
                    //                        trxQty = GetProductQtyFroMVAMStorage(line, line.GetVAM_LocatorTo_ID());
                    //                    }

                    query = @"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + line.GetVAM_LocatorTo_ID() +
                           " AND t.VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + line.GetVAM_PFeature_SetInstance_ID();
                    trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, Get_Trx()));

                    // get container Current qty from transaction
                    if (isContainerApplicable && line.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        containerCurrentQty = GetContainerQtyFroMVAMInvTrx(line, GetMovementDate(), line.GetVAM_LocatorTo_ID(),
                            line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                    }

                    // Done to Update Current Qty at Transaction
                    // create transaction to with Locator To refernce
                    MVAMInvTrx trxTo = new MVAMInvTrx(GetCtx(), line.GetVAF_Org_ID(),
                        MVAMInvTrx.MOVEMENTTYPE_MovementTo,
                        line.GetVAM_LocatorTo_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstanceTo_ID(),
                        line.GetMovementQty(), GetMovementDate(), Get_TrxName());
                    trxTo.SetVAM_InvTrf_Line_ID(line.GetVAM_InvTrf_Line_ID());
                    trxTo.SetCurrentQty(trxQty + line.GetMovementQty());
                    // set Material Policy Date
                    trxTo.SetMMPolicyDate(GetMovementDate());
                    if (isContainerApplicable && trxTo.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        // Update Product Container on Transaction
                        trxTo.SetVAM_ProductContainer_ID(line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                        // update containr or withot container qty Current Qty 
                        trxTo.SetContainerCurrentQty(containerCurrentQty + line.GetMovementQty());
                    }
                    if (!trxTo.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = pp.GetName();
                        else
                            _processMsg = "Transaction To not inserted";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    if (isContainerApplicable && trxTo.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        String errorMessage = UpdateTransactionContainer(line, trxTo, trxQty.Value + line.GetMovementQty(), line.GetVAM_LocatorTo_ID(),
                               line.IsMoveFullContainer() ? line.GetVAM_ProductContainer_ID() : line.GetRef_VAM_ProductContainerTo_ID());
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            SetProcessMsg(errorMessage);
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    else
                    {
                        UpdateTransaction(line, trxTo, trxQty.Value + line.GetMovementQty(), line.GetVAM_LocatorTo_ID());
                    }
                    //UpdateCurrentRecord(line, trxTo, line.GetMovementQty(), line.GetVAM_LocatorTo_ID());
                    #endregion

                    #endregion
                }	//	Fallback

                // Enhanced by Amit for Cost Queue 10-12-2015
                if (client.IsCostImmediate())
                {
                    #region Costing Calculation

                    // create object of To Locator where we are moving products
                    MVAMLocator locatorTo = MVAMLocator.Get(GetCtx(), line.GetVAM_LocatorTo_ID());

                    // is used to maintain cost of "move to" 
                    Decimal toCurrentCostPrice = 0;

                    #region get price from VAM_ProductCost (Current Cost Price)
                    if (GetDescription() != null && GetDescription().Contains("{->"))
                    {
                        // do not update current cost price during reversal, this time reverse doc contain same amount which are on original document
                    }
                    else
                    {
                        // For From Warehouse
                        currentCostPrice = 0;
                        currentCostPrice = MVAMVAMProductCost.GetproductCosts(line.GetVAF_Client_ID(), line.GetVAF_Org_ID(),
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetDTD001_MWarehouseSource_ID());

                        // For To Warehouse
                        toCurrentCostPrice = MVAMVAMProductCost.GetproductCosts(line.GetVAF_Client_ID(), locatorTo.GetVAF_Org_ID(),
                           line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), locatorTo.GetVAM_Warehouse_ID());

                        //line.SetCurrentCostPrice(currentCostPrice);
                        //if (!line.Save(Get_Trx()))
                        //{
                        //    ValueNamePair pp = VLogger.RetrieveError();
                        //    log.Info("Error found for Movement Line for this Line ID = " + line.GetVAM_InvTrf_Line_ID() +
                        //               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                        //    //Get_Trx().Rollback();
                        //}

                        DB.ExecuteQuery("UPDATE VAM_InvTrf_Line SET CurrentCostPrice = " + currentCostPrice + @" , ToCurrentCostPrice = " + toCurrentCostPrice + @"
                                                WHERE VAM_InvTrf_Line_ID = " + line.GetVAM_InvTrf_Line_ID(), null, Get_Trx());
                    }
                    #endregion

                    //query = "SELECT VAF_Org_ID FROM VAM_Warehouse WHERE IsActive = 'Y' AND VAM_Warehouse_ID = " + GetVAM_Warehouse_ID();
                    // Get Org of "To Warehouse"
                    //int ToWarehouseOrg = MVAMLocator.Get(GetCtx(), line.GetVAM_LocatorTo_ID()).GetVAF_Org_ID();
                    //if (GetVAF_Org_ID() != ToWarehouseOrg)
                    //{
                    product1 = new MVAMProduct(GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
                    if (product1.GetProductType() == "I") // for Item Type product
                    {
                        if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), product1, line.GetVAM_PFeature_SetInstance_ID(),
                          "Inventory Move", null, null, line, null, null, 0, line.GetMovementQty(), Get_TrxName(), out conversionNotFoundInOut, optionalstr: "window"))
                        {
                            if (!conversionNotFoundMovement1.Contains(conversionNotFoundMovement))
                            {
                                conversionNotFoundMovement1 += conversionNotFoundMovement + " , ";
                            }
                            _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                            if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                            {
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else if (!IsReversal()) // not to update cost for reversed document
                        {
                            currentCostPrice = MVAMVAMProductCost.GetproductCosts(line.GetVAF_Client_ID(), line.GetVAF_Org_ID(),
                         line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetDTD001_MWarehouseSource_ID());

                            toCurrentCostPrice = MVAMVAMProductCost.GetproductCosts(line.GetVAF_Client_ID(), locatorTo.GetVAF_Org_ID(),
                           line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), locatorTo.GetVAM_Warehouse_ID());

                            DB.ExecuteQuery("UPDATE VAM_InvTrf_Line SET PostCurrentCostPrice = " + currentCostPrice +
                                                @" , ToPostCurrentCostPrice = " + toCurrentCostPrice + @" , IsCostImmediate = 'Y' 
                                                WHERE VAM_InvTrf_Line_ID = " + line.GetVAM_InvTrf_Line_ID(), null, Get_Trx());
                        }
                    }
                    //}
                    #endregion
                }
                //End

            }	//	for all lines
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Set the document number feom Completed Doxument Sequence after completed
        /// </summary>
        private void SetCompletedDocumentNo()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (IsReversal())
            {
                return;
            }

            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetMovementDate(DateTime.Now.Date);

                //	Std Period open?
                if (!MVABYearPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetVAF_Org_ID()))
                {
                    throw new Exception("@PeriodClosed@");
                }
            }

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MVAFRecordSeq.GetDocumentNo(GetVAB_DocTypes_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        /// verify - is it possible to move container from one locator to other
        /// </summary>
        /// <param name="movement_Id"></param>
        /// <returns></returns>
        public bool IsMoveFullContainerPossible(int movement_Id)
        {
            // when we complete record, and movement line having MoveFullContainer = true
            // system will check qty on transaction in Container must be same as qty on movement line (which represent qty on transaction  based on movement date)
            // if not matched, then we can not allow to user for its complete, he need to make a new Movement for move container
            //            string sql = @"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM 
            //                               (SELECT PName, ROW_NUMBER () OVER (ORDER BY PName ) RN, COUNT (*) OVER () CNT FROM 
            //                               (
            //                                SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
            //                                FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m ON m.VAM_InventoryTransfer_id = ml.VAM_InventoryTransfer_id
            //                                INNER JOIN VAM_Product p ON p.VAM_Product_id = ml.VAM_Product_id
            //                                LEFT JOIN VAM_PFeature_SetInstance asi ON NVL(asi.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
            //                                 WHERE ml.MoveFullContainer ='Y' AND m.VAM_InventoryTransfer_ID =" + movement_Id + @"
            //                                    AND (ml.movementqty) <>
            //                                     NVL((SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.VAM_Inv_Trx_ID) AS CurrentQty
            //                                     FROM VAM_Inv_Trx t INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
            //                                      WHERE t.MovementDate <= 
            //                                            (Select MAX(movementdate) from VAM_Inv_Trx where 
            //                                            VAF_Client_ID = m.VAF_Client_ID  AND VAM_Locator_ID = ml.VAM_Locator_ID
            //                                            AND VAM_Product_ID = ml.VAM_Product_ID AND NVL(VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID, 0)
            //                                            AND NVL(VAM_ProductContainer_ID, 0) = NVL(ml.VAM_ProductContainer_ID, 0) )
            //                                       AND t.VAF_Client_ID                     = m.VAF_Client_ID
            //                                       AND t.VAM_Locator_ID                     = ml.VAM_Locator_ID
            //                                       AND t.VAM_Product_ID                     = ml.VAM_Product_ID
            //                                       AND NVL(t.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID, 0)
            //                                       AND NVL(t.VAM_ProductContainer_ID, 0)    =  NVL(ml.VAM_ProductContainer_ID, 0)  ), 0) 
            //                                       AND ROWNUM <= 100 )
            //                               ) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ";

            string sql = DBFunctionCollection.CheckMoveContainer(GetVAM_InventoryTransfer_ID());
            log.Info(sql);
            string productName = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (!string.IsNullOrEmpty(productName))
            {
                // Qty in Continer not matched with qty in container on spcified date : 
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_ContainerQtyNotMatched") + productName;
                SetProcessMsg(_processMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// during full move container - count no of Products on movement line in container must be equal to no. of Products on Tansaction for the same container.
        /// even we compare Qty on movementline and transaction 
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        public string IsMoveContainerProductCount(int movementId, List<ParentChildContainer> listParentChildContainer)
        {
            DataSet ds = null;
            StringBuilder misMatch = new StringBuilder();

            StringBuilder childContainer = new StringBuilder();
            if (listParentChildContainer.Count > 0)
            {
                childContainer.Clear();
                for (int i = 0; i < listParentChildContainer.Count; i++)
                {
                    if (String.IsNullOrEmpty(childContainer.ToString()))
                        childContainer.Append(listParentChildContainer[i].childContainer);
                    else
                        childContainer.Append(" , " + listParentChildContainer[i].childContainer);
                }
            }

            // get data from Transaction
            string sql = @"SELECT VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductContainer_ID, ContainerCurrentQty, Name FROM (
                            SELECT VAM_Product_ID, VAM_PFeature_SetInstance_ID,  VAM_ProductContainer_ID, ContainerCurrentQty, Name FROM 
                            (SELECT DISTINCT t.VAM_Product_ID, NVL(t.VAM_PFeature_SetInstance_ID, 0) AS VAM_PFeature_SetInstance_ID, t.VAM_ProductContainer_ID ,
                              First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID , t.VAM_ProductContainer_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC)  AS ContainerCurrentQty , p.Name
                               FROM VAM_Inv_Trx t INNER JOIN VAM_Product p ON p.VAM_Product_ID   = t.VAM_Product_ID WHERE t.IsActive = 'Y' AND VAM_ProductContainer_ID IN 
                               (" + childContainer + @" ) )t WHERE ContainerCurrentQty <> 0 
                          UNION ALL
                            SELECT m.VAM_Product_id, NVL(m.VAM_PFeature_SetInstance_id, 0) AS VAM_PFeature_SetInstance_id,
                              m.VAM_ProductContainer_ID, m.movementqty AS ContainerCurrentQty , p.Name
                              FROM VAM_InvTrf_Line m INNER JOIN VAM_Product p ON p.VAM_Product_ID  = m.VAM_Product_ID
                              WHERE VAM_InventoryTransfer_id   = " + movementId + @" AND movefullcontainer = 'Y' ) final
                             GROUP BY VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_ProductContainer_ID, ContainerCurrentQty, Name HAVING COUNT(1) = 1	";
            log.Info(sql);
            ds = DB.ExecuteDataset(sql, null, Get_Trx());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!misMatch.ToString().Contains(Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"])))
                    {
                        misMatch.Append(", " + Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]));
                    }
                }
            }
            ds.Dispose();
            return misMatch.ToString();
        }

        /// <summary>
        /// Is used to update container location where we moved
        /// </summary>
        /// <param name="movementId">Record ID</param>
        /// <returns></returns>
        /// <writer>Amit Bansal</writer>
        public bool UpdateContainerLocation(int movementId, List<ParentChildContainer> listParentChildContainer)
        {
            if (listParentChildContainer.Count > 0)
            {
                for (int i = 0; i < listParentChildContainer.Count; i++)
                {
                    // check to warehouse id defined on movement header or not
                    int warehouseTo = Util.GetValueOfInt(listParentChildContainer[i].VAM_WarehouseTo_ID);
                    MVAMLocator locator = MVAMLocator.Get(GetCtx(), Util.GetValueOfInt(listParentChildContainer[i].VAM_LocatorTo_ID));
                    if (warehouseTo == 0)
                    {
                        // if to warehouse not defined on header then get warehouse id on behalf of "To Locator"
                        warehouseTo = locator.GetVAM_Warehouse_ID();
                    }

                    // update warehouse, locator and DateLastInventory on parent and its child record
                    int no = DB.ExecuteQuery(@"UPDATE VAM_ProductContainer SET VAF_Org_ID = " + locator.GetVAF_Org_ID() +
                                             @" , VAM_Warehouse_ID = " + warehouseTo +
                                             @" , VAM_Locator_ID = " + Util.GetValueOfInt(listParentChildContainer[i].VAM_LocatorTo_ID) +
                                             @" , DateLastInventory = " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                             @" WHERE VAM_ProductContainer_ID IN (" + Util.GetValueOfString(listParentChildContainer[i].childContainer) + ")", null, Get_Trx());

                    // update on target container - "Parent Containr" reference where we moved this container
                    no = DB.ExecuteQuery(@"UPDATE VAM_ProductContainer SET Ref_M_Container_ID = " + (Util.GetValueOfInt(listParentChildContainer[i].VAM_ProductContainerTo_ID) > 0
                                                                        ? Util.GetValueOfString(listParentChildContainer[i].VAM_ProductContainerTo_ID) : "null") +
                                             @" WHERE VAM_ProductContainer_ID = " + Util.GetValueOfInt(listParentChildContainer[i].TagetContainer_ID), null, Get_Trx());
                }
            }
            return true;
        }

        /// <summary>
        /// is used to get detail of all Child container including Target Container
        /// </summary>
        /// <param name="movementId">Record ID</param>
        /// <returns>list(ParentChildContainer) -- which contain All Child containe in movement, ToWarehouse, ToLocator, ToContainer, Target Container</returns>
        /// <writer>Amit Bansal</writer>
        public List<ParentChildContainer> ProductChildContainer(int movementId)
        {
            List<ParentChildContainer> listParentChildContainer = new List<ParentChildContainer>();
            ParentChildContainer parentChildContainer = null;
            bool ispostgerSql = DatabaseType.IsPostgre;
            // Get all UNIQUE movement line 
            DataSet dsMovementLine = DB.ExecuteDataset(@"SELECT DISTINCT ml.TargetContainer_ID AS ParentContainer, m.VAM_Warehouse_ID AS ToWarehouse,
                                               ml.VAM_LocatorTo_ID AS ToLocator,  ml.Ref_VAM_ProductContainerTo_ID AS ToContainer  
                                             FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m ON ml.VAM_InventoryTransfer_ID = m.VAM_InventoryTransfer_ID 
                                             WHERE ml.MoveFullContainer = 'Y' AND ml.IsActive = 'Y' AND ml.VAM_InventoryTransfer_ID = " + movementId, null, Get_Trx());
            if (dsMovementLine != null && dsMovementLine.Tables.Count > 0 && dsMovementLine.Tables[0].Rows.Count > 0)
            {
                StringBuilder childContainerId = new StringBuilder();
                for (int i = 0; i < dsMovementLine.Tables[0].Rows.Count; i++)
                {
                    parentChildContainer = new ParentChildContainer();
                    childContainerId.Clear();
                    string pathContainer = "";
                    String sql = "";
                    // Get path upto "Target Container" (top most parent to "target container")
                    if (ispostgerSql)
                    {
                        sql = @"WITH RECURSIVE pops (VAM_ProductContainer_id, level, name_path) AS (
                        SELECT  VAM_ProductContainer_id, 0,  ARRAY[VAM_ProductContainer_id]
                        FROM    VAM_ProductContainer
                        WHERE   Ref_M_Container_ID is null
                        UNION ALL
                        SELECT  p.VAM_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.VAM_ProductContainer_id)
                        FROM    VAM_ProductContainer p
                                INNER JOIN pops t0 ON t0.VAM_ProductContainer_id = p.Ref_M_Container_ID
                    )
                        SELECT    ARRAY_TO_STRING(name_path, '->')
                        FROM    pops  where VAM_ProductContainer_id = " + Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]);
                        pathContainer = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                    }
                    else
                    {
                        pathContainer = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT sys_connect_by_path(VAM_ProductContainer_id,'->') tree
                                            FROM VAM_ProductContainer 
                                           WHERE VAM_ProductContainer_id = " + Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]) + @"
                                            START WITH ref_m_container_id IS NULL CONNECT BY prior VAM_ProductContainer_id = ref_m_container_id
                                           ORDER BY tree", null, Get_Trx()));
                    }

                    #region get All child of Target Container including target container reference
                    DataSet dsChildContainer = null;
                    if (ispostgerSql)
                    {
                        sql = @"WITH RECURSIVE pops (VAM_ProductContainer_id, level, name_path) AS (
                                SELECT  VAM_ProductContainer_id, 0,  ARRAY[VAM_ProductContainer_id]
                                FROM    VAM_ProductContainer
                                WHERE   Ref_M_Container_ID is null
                                UNION ALL
                                SELECT  p.VAM_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.VAM_ProductContainer_id)
                                FROM    VAM_ProductContainer p
                                        INNER JOIN pops t0 ON t0.VAM_ProductContainer_id = p.Ref_M_Container_ID )
                            SELECT  VAM_ProductContainer_id, level,  ARRAY_TO_STRING(name_path, '->')
                            FROM    pops  where ARRAY_TO_STRING(name_path, '->') like '" + pathContainer + "%'";
                        dsChildContainer = DB.ExecuteDataset(sql, null, Get_Trx());
                    }
                    else
                    {
                        dsChildContainer = DB.ExecuteDataset(@"SELECT tree, VAM_ProductContainer_id FROM
                                                        (SELECT sys_connect_by_path(VAM_ProductContainer_id,'->') tree , VAM_ProductContainer_id
                                                         FROM VAM_ProductContainer
                                                         START WITH ref_m_container_id IS NULL
                                                         CONNECT BY prior VAM_ProductContainer_id = ref_m_container_id
                                                         ORDER BY tree  
                                                         )
                                                     WHERE tree LIKE ('" + pathContainer + "%') ", null, Get_Trx());
                    }
                    if (dsChildContainer != null && dsChildContainer.Tables.Count > 0 && dsChildContainer.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsChildContainer.Tables[0].Rows.Count; j++)
                        {
                            if (String.IsNullOrEmpty(childContainerId.ToString()))
                                childContainerId.Append(Util.GetValueOfString(dsChildContainer.Tables[0].Rows[j]["VAM_ProductContainer_id"]));
                            else
                                childContainerId.Append("," + Util.GetValueOfString(dsChildContainer.Tables[0].Rows[j]["VAM_ProductContainer_id"]));
                        }

                        parentChildContainer.childContainer = childContainerId.ToString();
                        parentChildContainer.VAM_WarehouseTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToWarehouse"]);
                        parentChildContainer.VAM_LocatorTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToLocator"]);
                        parentChildContainer.VAM_ProductContainerTo_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ToContainer"]);
                        parentChildContainer.TagetContainer_ID = Util.GetValueOfInt(dsMovementLine.Tables[0].Rows[i]["ParentContainer"]);
                        listParentChildContainer.Add(parentChildContainer);
                    }
                    dsChildContainer.Dispose();
                    #endregion

                }
            }
            dsMovementLine.Dispose();
            return listParentChildContainer;
        }

        /// <summary>
        /// Is Used to get the Parent Container which are to be fully moved with child
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool ParentMoveFromPath(int movementId)
        //        {
        //            String path = null;
        //            // Get Path from Parent upto defined Product Container
        //            string sql = @"SELECT sys_connect_by_path(VAM_ProductContainer_id,'->') tree ,  VAM_ProductContainer_id
        //                            FROM VAM_ProductContainer 
        //                           WHERE (VAM_ProductContainer_id IN 
        //                            (SELECT VAM_ProductContainer_ID FROM VAM_InvTrf_Line WHERE IsParentMove= 'Y' AND VAM_InventoryTransfer_ID = " + movementId + @" ))
        //                            START WITH ref_m_container_id IS NULL CONNECT BY prior VAM_ProductContainer_id = ref_m_container_id
        //                           ORDER BY tree ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    UpdateFromPath(path, movementId);
        //                }
        //            }
        //            return true;
        //        }

        /// <summary>
        /// update From path on movement header
        /// </summary>
        /// <param name="containerPath"></param>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool UpdateFromPath(String containerPath, int movementId)
        //        {
        //            String path = null;
        //            // Get Path from selected Product Container to all child
        //            string sql = @"SELECT tree FROM
        //                            (SELECT sys_connect_by_path(VAM_ProductContainer_id,'->') tree
        //                             FROM VAM_ProductContainer
        //                             START WITH ref_m_container_id IS NULL
        //                             CONNECT BY prior VAM_ProductContainer_id = ref_m_container_id
        //                             ORDER BY tree  
        //                             )
        //                           WHERE tree LIKE ('" + containerPath + "%') ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (String.IsNullOrEmpty(path))
        //                        path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    else
        //                        path += "," + Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                }
        //            }
        //            int no = DB.ExecuteQuery("UPDATE VAM_InventoryTransfer SET FromPath = FromPath || '|'  || '" + path + @"' WHERE VAM_InventoryTransfer_ID = " + movementId, null, Get_Trx());
        //            return true;
        //        }

        /// <summary>
        /// Update To path on move header
        /// </summary>
        /// <param name="movementId"></param>
        /// <returns></returns>
        //        public bool ParentMoveToPath(int movementId)
        //        {
        //            String path = null;
        //            // Get Path from Parent upto defined Product Container To
        //            string sql = @"SELECT sys_connect_by_path(VAM_ProductContainer_id,'->') tree ,  VAM_ProductContainer_id
        //                            FROM VAM_ProductContainer 
        //                           WHERE (VAM_ProductContainer_id IN 
        //                            (SELECT Ref_VAM_ProductContainerTo_ID FROM VAM_InvTrf_Line WHERE IsParentMove= 'Y' AND VAM_InventoryTransfer_ID = " + movementId + @" ))
        //                            START WITH ref_m_container_id IS NULL CONNECT BY prior VAM_ProductContainer_id = ref_m_container_id
        //                           ORDER BY tree ";
        //            log.Finest(sql);
        //            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (String.IsNullOrEmpty(path))
        //                        path = Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                    else
        //                        path += "|" + Util.GetValueOfString(ds.Tables[0].Rows[i]["tree"]);
        //                }
        //            }

        //            int no = DB.ExecuteQuery("UPDATE VAM_InventoryTransfer SET ToPath = '" + path + @"' WHERE VAM_InventoryTransfer_ID = " + movementId, null, Get_Trx());

        //            return true;
        //        }

        /// <summary>
        /// Get Parent container
        /// </summary>
        /// <param name="path"></param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        //public int GetContainerId(string path, int containerId)
        //{
        //    // represent array of "parent to child" container path 
        //    String[] splittedParentContainer = GetFromPath().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (splittedParentContainer.Length > 0)
        //    {
        //        String[] ParentToChildPath;
        //        for (int i = 0; i < splittedParentContainer.Length; i++)
        //        {
        //            // represent path of individual container (parent ot child)
        //            ParentToChildPath = splittedParentContainer[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (ParentToChildPath.Length > 0)
        //            {
        //                // find Parent Container Id
        //                int lastIndexValue = ParentToChildPath[0].LastIndexOf("->");
        //                int ParentContaineId = int.Parse(ParentToChildPath[0].Substring(lastIndexValue, ParentToChildPath.Length));
        //                if (ParentContaineId == containerId)
        //                {
        //                    int secondlastIndexValue = lastIndexValue > 0 ? ParentToChildPath[0].LastIndexOf("->", lastIndexValue - 1) : -1;
        //                    if (secondlastIndexValue == -1)
        //                    {
        //                        // itself parent
        //                        return 0;
        //                    }
        //                    else
        //                    {
        //                        // parent container id 
        //                        secondlastIndexValue += 2;
        //                        int parentContainerRefernce = int.Parse(ParentToChildPath[0].Substring(secondlastIndexValue, (lastIndexValue - secondlastIndexValue)));
        //                        return parentContainerRefernce;
        //                    }
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //        }
        //    }
        //    return 0;
        //}


        private void updateCostQueue(MVAMProduct product, int M_ASI_ID, MVABAccountBook mas,
          int Org_ID, MVAMVAMProductCostElement ce, decimal movementQty)
        {
            //MVAMVAMProductCostQueue[] cQueue = MVAMVAMProductCostQueue.GetQueue(product1, sLine.GetVAM_PFeature_SetInstance_ID(), acctSchema, GetVAF_Org_ID(), costElement, null);
            MVAMVAMProductCostQueue[] cQueue = MVAMVAMProductCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, null);
            if (cQueue != null && cQueue.Length > 0)
            {
                Decimal qty = movementQty;
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MVAMVAMProductCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    qty = MVAMVAMProductCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    //if (cq == cQueue.Length - 1 && qty < 0) // last record
                    //{
                    //    queue.SetCurrentQty(qty);
                    //    if (!queue.Save())
                    //    {
                    //        ValueNamePair pp = VLogger.RetrieveError();
                    //        log.Info("Cost Queue not updated for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    //    }
                    //}
                    if (qty <= 0)
                    {
                        queue.Delete(true);
                        qty = Decimal.Negate(qty);
                    }
                    else
                    {
                        queue.SetCurrentQty(qty);
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Info("Cost Queue not updated for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                        }
                    }
                    if (value)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="trxFrom"></param>
        /// <param name="qtyMove"></param>
        private void UpdateTransaction(MVAMInvTrfLine line, MVAMInvTrx trxFrom, decimal qtyMove, int loc_ID)
        {
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";
            DataSet ds = new DataSet();
            MVAMInvTrx trx = null;
            MVAMInventoryLine inventoryLine = null;
            MVAMInventory inventory = null;

            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + qtyMove + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID
                    //     + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id , MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID()
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC ,  created ASC";
                }
                else
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + qtyMove + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID
                    //       + " AND VAM_PFeature_SetInstance_ID =  0 ";
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id , MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(trxFrom.GetMovementDate().Value.AddDays(1), true)
                             + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = 0 "
                             + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC ,  created ASC";
                }

                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                    Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MVAMInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MVAMInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetVAM_Inventory_ID()), Get_TrxName());         // Trx used to handle query stuck problem
                                if (!inventory.IsInternalUse())
                                {
                                    //break;
                                    inventoryLine.SetQtyBook(qtyMove);
                                    inventoryLine.SetOpeningStock(qtyMove);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]));
                                    }

                                    trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                    }
                                    else
                                    {
                                        qtyMove = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != qtyMove)
                                        {
                                            storage.SetQtyOnHand(qtyMove);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                            trx.SetCurrentQty(qtyMove + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                            }
                            else
                            {
                                qtyMove = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                           Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != qtyMove)
                                {
                                    storage.SetQtyOnHand(qtyMove);
                                    storage.Save();
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }

        private string UpdateTransactionContainer(MVAMInvTrfLine sLine, MVAMInvTrx mtrx, decimal Qty, int loc_ID, int containerId)
        {
            string errorMessage = null;
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
            MVAMInvTrx trx = null;
            MVAMInventoryLine inventoryLine = null;
            MVAMInventory inventory = null;
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";
            DataSet ds = new DataSet();
            Decimal containerCurrentQty = mtrx.GetContainerCurrentQty();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID();
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty , NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID()
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC, created ASC";
                }
                else
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 ";
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC , created ASC";
                }
                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MVAMInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MVAMInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetVAM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    if (inventoryLine.GetVAM_ProductContainer_ID() == containerId)
                                    {
                                        inventoryLine.SetQtyBook(containerCurrentQty);
                                        inventoryLine.SetOpeningStock(containerCurrentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]));
                                        }
                                    }

                                    trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                                    if (trx.GetVAM_ProductContainer_ID() == containerId)
                                    {
                                        trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]))));
                                    }
                                    else
                                    {
                                        trx.SetCurrentQty(Decimal.Add(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["movementqty"])));
                                    }
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                        }
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                        if (sLine.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == containerId)
                                            containerCurrentQty = trx.GetContainerCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != Qty)
                                        {
                                            storage.SetQtyOnHand(Qty);
                                            if (!storage.Save())
                                            {
                                                Get_TrxName().Rollback();
                                                ValueNamePair pp = VLogger.RetrieveError();
                                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                {
                                                    _processMsg = pp.GetName();
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                                }
                                                else
                                                {
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                                }
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (trx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == containerId)
                            {
                                trx.SetContainerCurrentQty(containerCurrentQty + trx.GetMovementQty());
                            }
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                Get_TrxName().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                {
                                    _processMsg = pp.GetName();
                                    return _processMsg;
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                }
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                                if (trx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == containerId)
                                    containerCurrentQty = trx.GetContainerCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != Qty)
                                {
                                    storage.SetQtyOnHand(Qty);
                                    if (!storage.Save())
                                    {
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch (Exception e)
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
                errorMessage = Msg.GetMsg(GetCtx(), "ExceptionOccureOnUpdateTrx");
            }
            return errorMessage;
        }

        private void UpdateCurrentRecord(MVAMInvTrfLine line, MVAMInvTrx trxM, decimal qtyDiffer, int loc_ID)
        {
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from VAM_Inv_Trx  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID;
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + "  and VAM_Locator_ID=" + loc_ID + " )order by VAM_Inv_Trx_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + " and VAM_Locator_ID=" + loc_ID + ") order by VAM_Inv_Trx_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }

                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                    //     + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from VAM_Inv_Trx  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID;
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + "  and VAM_Locator_ID=" + loc_ID + " )order by VAM_Inv_Trx_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + loc_ID + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + " and VAM_Locator_ID=" + loc_ID + ") order by VAM_Inv_Trx_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID();
                }

                // int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
            }
            catch
            {
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFroMVAMStorage(MVAMInvTrfLine line, int loc_ID)
        {
            return 0;
            //MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
            //int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            //string sql = "";

            //if (attribSet_ID > 0)
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM VAM_Storage WHERE VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID
            //         + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
            //}
            //else
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM VAM_Storage WHERE VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + loc_ID;
            //}
            //return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// Get Latest Current Quantity based on movementdate
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFroMVAMInvTrx(MVAMInvTrfLine line, DateTime? movementDate, bool isAttribute, int locatorId)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id  =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            return result;
        }

        /// <summary>
        /// This function is used to get current Qty based on Product Container
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <returns></returns>
        private Decimal? GetContainerQtyFroMVAMInvTrx(MVAMInvTrfLine line, DateTime? movementDate, int locatorId, int containerId)
        {
            Decimal result = 0;
            string sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, 
                        t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS ContainerCurrentQty
                           FROM VAM_Inv_Trx t
                           WHERE t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND t.VAF_Client_ID                       = " + line.GetVAF_Client_ID() + @"
                           AND t.VAM_Locator_ID                       = " + locatorId + @"
                           AND t.VAM_Product_ID                       = " + line.GetVAM_Product_ID() + @"
                           AND NVL(t.VAM_PFeature_SetInstance_ID , 0) = COALESCE(" + line.GetVAM_PFeature_SetInstance_ID() + @",0)
                           AND NVL(t.VAM_ProductContainer_ID, 0)              = " + containerId;
            result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
            return result;
        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        private void CheckMaterialPolicy()
        {
            int no = MVAMInvTrfLineMP.DeleteMovementMA(GetVAM_InventoryTransfer_ID(), Get_TrxName());
            if (no > 0)
                log.Config("Delete old #" + no);
            MVAMInvTrfLine[] lines = GetLines(false);

            MVAFClient client = MVAFClient.Get(GetCtx());

            //	Check Lines
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvTrfLine line = lines[i];

                Boolean needSave = false;

                //	Attribute Set Instance
                if (line.GetVAM_PFeature_SetInstance_ID() == 0)
                {
                    MVAMProduct product = MVAMProduct.Get(GetCtx(), line.GetVAM_Product_ID());
                    MVAMProductCategory pc = MVAMProductCategory.Get(GetCtx(), product.GetVAM_ProductCategory_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                        MMPolicy = client.GetMMPolicy();
                    //
                    MVAMStorage[] storages = MVAMStorage.GetAllWithASI(GetCtx(),
                        line.GetVAM_Product_ID(), line.GetVAM_Locator_ID(),
                        MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                    Decimal qtyToDeliver = line.GetMovementQty();
                    for (int ii = 0; ii < storages.Length; ii++)
                    {
                        MVAMStorage storage = storages[ii];
                        if (ii == 0)
                        {
                            if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                            {
                                line.SetVAM_PFeature_SetInstance_ID(storage.GetVAM_PFeature_SetInstance_ID());
                                needSave = true;
                                log.Config("Direct - " + line);
                                qtyToDeliver = Env.ZERO;
                            }
                            else
                            {
                                log.Config("Split - " + line);
                                MVAMInvTrfLineMP ma = new MVAMInvTrfLineMP(line,
                                    storage.GetVAM_PFeature_SetInstance_ID(),
                                    storage.GetQtyOnHand());
                                if (!ma.Save())
                                    ;
                                qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                            }
                        }
                        else	//	 create Addl material allocation
                        {
                            MVAMInvTrfLineMP ma = new MVAMInvTrfLineMP(line,
                                storage.GetVAM_PFeature_SetInstance_ID(),
                                qtyToDeliver);
                            if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                qtyToDeliver = Env.ZERO;
                            else
                            {
                                ma.SetMovementQty(storage.GetQtyOnHand());
                                qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                            }
                            if (!ma.Save())
                                ;
                            log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                        }
                        if (Env.Signum(qtyToDeliver) == 0)
                            break;
                    }	//	 for all storages

                    //	No AttributeSetInstance found for remainder
                    if (Env.Signum(qtyToDeliver) != 0)
                    {
                        MVAMInvTrfLineMP ma = new MVAMInvTrfLineMP(line,
                            0, qtyToDeliver);
                        if (!ma.Save())
                            ;
                        log.Fine("##: " + ma);
                    }
                }	//	attributeSetInstance

                if (needSave && !line.Save())
                    log.Severe("NOT saved " + line);
            }	//	for all lines

        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        /// <param name="line">movement line</param>
        private void CheckMaterialPolicy(MVAMInvTrfLine line)
        {
            int no = MVAMInvTrfLineMP.DeleteMovementLineMA(line.GetVAM_InvTrf_Line_ID(), Get_TrxName());
            if (no > 0)
                log.Config("Delete old #" + no);

            // check is any record available of physical Inventory after date Movement -
            // if available - then not to create Attribute Record - neither to take impacts on Container Storage
            if (isContainerApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_ContainerStorage WHERE IsPhysicalInventory = 'Y'
                 AND MMPolicyDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                      @" AND VAM_Product_ID = " + line.GetVAM_Product_ID() +
                      @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + line.GetVAM_PFeature_SetInstance_ID() +
                      @" AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() +
                      @" AND NVL(VAM_ProductContainer_ID , 0) = " + line.GetVAM_ProductContainer_ID(), null, Get_Trx())) > 0)
            {
                return;
            }

            MVAFClient client = MVAFClient.Get(GetCtx());
            Boolean needSave = false;
            //bool isLifoChecked = false;

            //	Attribute Set Instance
            //if (line.GetVAM_PFeature_SetInstance_ID() == 0)
            //{
            MVAMProduct product = MVAMProduct.Get(GetCtx(), line.GetVAM_Product_ID());
            MVAMProductCategory pc = MVAMProductCategory.Get(GetCtx(), product.GetVAM_ProductCategory_ID());
            String MMPolicy = pc.GetMMPolicy();
            if (MMPolicy == null || MMPolicy.Length == 0)
                MMPolicy = client.GetMMPolicy();

            //
            dynamic[] storages = null;
            if (isContainerApplicable)
            {
                storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
                 line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                 line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                 MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), false, Get_TrxName());
            }
            else
            {
                storages = MVAMStorage.GetWarehouse(GetCtx(), GetDTD001_MWarehouseSource_ID(), line.GetVAM_Product_ID(),
                         line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                            line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                            MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
            }

            Decimal qtyToDeliver = line.GetMovementQty();

            //LIFOManage:
            for (int ii = 0; ii < storages.Length; ii++)
            {
                dynamic storage = storages[ii];

                // when storage qty is less than equal to ZERO then continue to other record
                if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()) <= 0)
                    continue;

                if ((isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()).CompareTo(qtyToDeliver) >= 0)
                {
                    MVAMInvTrfLineMP ma = MVAMInvTrfLineMP.GetOrCreate(line,
                    storage.GetVAM_PFeature_SetInstance_ID(),
                    qtyToDeliver, isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Handle exception
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                    qtyToDeliver = Env.ZERO;
                    log.Fine("#" + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                }
                else
                {
                    log.Config("Split - " + line);
                    MVAMInvTrfLineMP ma = MVAMInvTrfLineMP.GetOrCreate(line,
                                storage.GetVAM_PFeature_SetInstance_ID(),
                            isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand(),
                            isContainerApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Handle exception
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, (isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand()));

                    log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                }
                if (Env.Signum(qtyToDeliver) == 0)
                    break;

                if (isContainerApplicable && ii == storages.Length - 1 && !MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy))
                {
                    storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
                                 line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                                 line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                                 MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                    ii = -1;
                }
            }	//	 for all storages

            //if (isContainerApplicable && !MClient.MMPOLICY_FiFo.Equals(MMPolicy) && !isLifoChecked && qtyToDeliver != 0)
            //{
            //    isLifoChecked = true;
            //    // Get Data from Container Storage based on Policy
            //    storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
            //  line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
            //  line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
            //  MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
            //    goto LIFOManage;
            //}

            if (Env.Signum(qtyToDeliver) != 0)
            {
                MVAMInvTrfLineMP ma = MVAMInvTrfLineMP.GetOrCreate(line, line.GetVAM_PFeature_SetInstance_ID(), qtyToDeliver, GetMovementDate());
                if (!ma.Save(line.Get_Trx()))
                {
                    // Handle exception
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                    else
                        throw new ArgumentException("Attribute Tab not saved");
                }
                log.Fine("##: " + ma);
            }
            //}	//	attributeSetInstance


            if (needSave && !line.Save())
                log.Severe("NOT saved " + line);

        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }
            MVABDocTypes doctype = new MVABDocTypes(GetCtx(), GetVAB_DocTypes_ID(), Get_Trx());
            bool InTransit = (bool)doctype.Get_Value("IsInTransit");
            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MVAMInvTrfLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MVAMInvTrfLine line = lines[i];
                    Decimal old = line.GetMovementQty();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        // line.SetMovementQty(Env.ZERO); //can't make Move Qty Zero ..for reference see aftersave (Movement Line)
                        line.SetMovementQty(decimal.Negate(old)); //Arpit To set void record quantity to negative ..asked by Surya Sir
                        line.AddDescription("Void (" + old + ")");
                        line.Save(Get_TrxName());
                    }
                    //Amit 13-nov-2014
                    if (line.GetVAM_RequisitionLine_ID() > 0)
                    {
                        MVAMRequisitionLine requisitionLine = new MVAMRequisitionLine(GetCtx(), line.GetVAM_RequisitionLine_ID(), Get_Trx());
                        requisitionLine.SetDTD001_ReservedQty(Decimal.Subtract(requisitionLine.GetDTD001_ReservedQty(), old));
                        requisitionLine.Save(Get_Trx());

                        MVAMStorage storageFrom = MVAMStorage.Get(GetCtx(), line.GetVAM_Locator_ID(),
                            line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        if (storageFrom == null)
                            storageFrom = MVAMStorage.GetCreate(GetCtx(), line.GetVAM_Locator_ID(),
                                line.GetVAM_Product_ID(), 0, Get_Trx());
                        storageFrom.SetQtyReserved(Decimal.Subtract(storageFrom.GetQtyReserved(), old));
                        storageFrom.Save(Get_Trx());
                    }
                    //Amit

                }
                // Added By Arpit on 9th Dec,2016 to set Void the document of Move Confirmation if found on the following conditions
                //  MVAMInventoryTransfer InvMove = new MVAMInventoryTransfer(GetCtx(), Get_ID(), Get_Trx());
                //  MVABDocTypes doctype = new MVABDocTypes(GetCtx(), GetVAB_DocTypes_ID(), Get_Trx());
                // bool InTransit = (bool)doctype.Get_Value("IsInTransit");
                if (InTransit == true)
                {
                    String Qry = "Select VAM_InvTrf_Confirm_ID from VAM_InvTrf_Confirm Where VAM_InventoryTransfer_ID=" + Get_ID();
                    int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
                    if (MoveConf_ID > 0)
                    {
                        MVAMInvTrfConfirm MoveConfirm = new MVAMInvTrfConfirm(GetCtx(), MoveConf_ID, Get_Trx());
                        MoveConfirm.SetDocStatus(DOCACTION_Void);
                        MoveConfirm.SetDocAction(DOCACTION_Void);
                        MoveConfirm.SetProcessed(true);
                        if (!MoveConfirm.Save(Get_Trx()))
                        {
                            Get_Trx().Rollback();
                            _processMsg = "Error while Processing ";
                            return false;
                            // set Transaction RollBack here if not saved
                        }
                        Qry = string.Empty;
                        Qry = "Select VAM_InvTrf_LineConfirm_ID from VAM_InvTrf_LineConfirm Where VAM_InvTrf_Confirm_ID=" + MoveConf_ID;
                        DataSet ds = DB.ExecuteDataset(Qry);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            MVAMInvTrfLineConfirm lineMoveConf = null;
                            for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                lineMoveConf = null;
                                lineMoveConf = new MVAMInvTrfLineConfirm(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InvTrf_LineConfirm_ID"]), Get_Trx());
                                lineMoveConf.SetProcessed(true);
                                lineMoveConf.Save(Get_Trx());
                            }
                        }

                    }
                }
                //Arpit
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns>true if success</returns>
        public Boolean CloseIt()
        {
            log.Info(ToString());

            //	Close Not delivered Qty
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReverseCorrectIt()
        {
            log.Info(ToString());

            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            if (!MVABYearPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType(), GetVAF_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate(), GetVAF_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }


            // when Inventory move is of Full Container, then not to reverse Inventory Move
            if (isContainerApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_InvTrf_Line WHERE MoveFullContainer = 'Y' AND IsActive = 'Y' AND VAM_InventoryTransfer_ID = " + GetVAM_InventoryTransfer_ID(), null, Get_Trx())) > 0)
            {
                SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_FullContainernotReverse"));
                return false;
            }

            //start Added by Arpit Rai on 9th Dec,2016
            //  MVABDocTypes DocType = new MVABDocTypes(GetCtx(), GetVAB_DocTypes_ID(), Get_Trx());
            bool InTransit = (bool)dt.Get_Value("IsInTransit");
            if (InTransit == true)
            {
                String Qry = "Select VAM_InvTrf_Confirm_ID from VAM_InvTrf_Confirm Where VAM_InventoryTransfer_ID=" + Get_ID();
                int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
                if (MoveConf_ID > 0)
                {
                    MVAMInvTrfConfirm MoveConfirm = new MVAMInvTrfConfirm(GetCtx(), MoveConf_ID, Get_Trx());
                    if (!MoveConfirm.ReverseCorrectIt())
                    {
                        Get_Trx().Rollback();
                        _processMsg = "Reversal ERROR: " + MoveConfirm.GetProcessMsg();
                        return false;
                    };
                    //}
                }
            }
            //end Arpit
            //	Deep Copy

            MVAMInventoryTransfer reversal = new MVAMInventoryTransfer(GetCtx(), 0, Get_TrxName());
            CopyValues(this, reversal, GetVAF_Client_ID(), GetVAF_Org_ID());

            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);	//	indicate reversals

            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetIsApproved(false);
            reversal.SetIsInTransit(false);
            reversal.SetPosted(false);
            reversal.SetProcessed(false);
            if (reversal.Get_ColumnIndex("ReversalDoc_ID") > 0 && reversal.Get_ColumnIndex("IsReversal") > 0)
            {
                // set Reversal property for identifying, record is reversal or not during saving or for other actions
                reversal.SetIsReversal(true);
                // Set Orignal Document Reference
                reversal.SetReversalDoc_ID(GetVAM_InventoryTransfer_ID());
            }

            // for reversal document set Temp Document No to empty
            if (reversal.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                reversal.SetTempDocumentNo("");
            }

            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (!reversal.Save())
            {
                pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Could not create Movement Reversal , " + pp.GetName();
                else
                    _processMsg = "Could not create Movement Reversal";
                return false;
            }

            //	Reverse Line Qty
            MVAMInvTrfLine[] oLines = GetLines(true);
            for (int i = 0; i < oLines.Length; i++)
            {
                MVAMInvTrfLine oLine = oLines[i];
                MVAMInvTrfLine rLine = new MVAMInvTrfLine(GetCtx(), 0, Get_TrxName());
                CopyValues(oLine, rLine, oLine.GetVAF_Client_ID(), oLine.GetVAF_Org_ID());
                rLine.SetVAM_InventoryTransfer_ID(reversal.GetVAM_InventoryTransfer_ID());
                //
                rLine.SetMovementQty(Decimal.Negate(rLine.GetMovementQty()));
                rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                rLine.SetTargetQty(Env.ZERO);
                rLine.SetScrappedQty(Env.ZERO);
                rLine.SetConfirmedQty(Env.ZERO);
                rLine.SetProcessed(false);
                if (rLine.Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    // Set Original Line reference
                    rLine.SetReversalDoc_ID(oLine.GetVAM_InvTrf_Line_ID());
                }
                rLine.SetActualReqReserved(oLine.GetActualReqReserved());
                if (!rLine.Save())
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Could not create Movement Reversal Line , " + pp.GetName();
                    else
                        _processMsg = "Could not create Movement Reversal Line";
                    return false;
                }

                //We need to copy Attribute MA
                MVAMInvTrfLineMP[] mas = MVAMInvTrfLineMP.Get(GetCtx(),
                        oLine.GetVAM_InvTrf_Line_ID(), Get_TrxName());
                for (int j = 0; j < mas.Length; j++)
                {
                    MVAMInvTrfLineMP ma = new MVAMInvTrfLineMP(rLine,
                            mas[j].GetVAM_PFeature_SetInstance_ID(),
                            Decimal.Negate(mas[j].GetMovementQty()), mas[j].GetMMPolicyDate());
                    if (!ma.Save(rLine.Get_TrxName()))
                    {
                        pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = "Could not create Movement Reversal Attribute , " + pp.GetName();
                        else
                            _processMsg = "Could not create Movement Reversal Attribute";
                        return false;
                    }
                }
            }
            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            MVAMInvTrfLine[] mlines = GetLines(true);
            for (int i = 0; i < mlines.Length; i++)
            {
                MVAMInvTrfLine mline = mlines[i];
                if (mline.GetA_Asset_ID() > 0)
                {
                    ast = new MVAAsset(GetCtx(), mline.GetA_Asset_ID(), Get_Trx());
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("VAFAM_", out aInfo))
                    {
                        MVAFAMAssetHistory aHist = new MVAFAMAssetHistory(GetCtx(), 0, Get_Trx());
                        ast.CopyTo(aHist);
                        aHist.SetA_Asset_ID(mline.GetA_Asset_ID());
                        if (!aHist.Save())
                        {
                            _processMsg = "Asset History Not Updated";
                            return false;
                        }
                    }
                    ast.SetVAM_Locator_ID(mline.GetVAM_Locator_ID());
                    ast.Save();
                }
            }
            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_TrxName()); //Pass Transaction Arpit

            //JID_0889: show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reversal.GetDocumentNo();

            //	Update Reversed (this)
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");
            // SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);	//	may come from void
            SetDocAction(DOCACTION_None);
            SetProcessed(true);
            Save(Get_Trx());
            ////start Added by Arpit Rai on 9th Dec,2016
            //MVABDocTypes DocType = new MVABDocTypes(GetCtx(), GetVAB_DocTypes_ID(), Get_Trx());
            //bool InTransit = (bool)DocType.Get_Value("IsInTransit");
            //if (InTransit == true)
            //{
            //    String Qry = "Select VAM_InvTrf_Confirm_ID from VAM_InvTrf_Confirm Where VAM_InventoryTransfer_ID=" + Get_ID();
            //    int MoveConf_ID = Convert.ToInt32(DB.ExecuteScalar(Qry));
            //    if (MoveConf_ID > 0)
            //    {
            //        MVAMInvTrfConfirm MoveConfirm = new MVAMInvTrfConfirm(GetCtx(), MoveConf_ID, Get_Trx());
            //        if (!MoveConfirm.ReverseCorrectIt()) {
            //            _processMsg = "Reversal ERROR: " + MoveConfirm.GetProcessMsg();
            //            return false;
            //        };
            //        //}
            //    }
            //}
            ////end Arpit
            return true;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public Boolean ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInventoryTransfer[");
            sb.Append(Get_ID())
                .Append("-").Append(GetDocumentNo())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>VAF_UserContact_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>VAB_Currency_ID</returns>
        public int GetVAB_Currency_ID()
        {
            //	MVAMPriceList pl = MVAMPriceList.Get(GetCtx(), GetVAM_PriceList_ID());
            //	return pl.GetVAB_Currency_ID();
            return 0;
        }

        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }



        public void SetProcessMsg(string processMsg)
        {
            _processMsg = processMsg;
        }
        #endregion

    }


    public class ParentChildContainer
    {
        public string childContainer { get; set; }
        public int VAM_WarehouseTo_ID { get; set; }
        public int VAM_LocatorTo_ID { get; set; }
        public int VAM_ProductContainerTo_ID { get; set; }
        public int TagetContainer_ID { get; set; }
    }
}