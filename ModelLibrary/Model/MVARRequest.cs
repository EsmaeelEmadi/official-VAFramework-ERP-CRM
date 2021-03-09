﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.IO;
//////using System.Windows.Forms;

using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Threading;

namespace VAdvantage.Model
{
    public class MVARRequest : X_VAR_Request
    {
        /**
	     * 	Get Request ID from mail text
	     *	@param mailText mail text
	     *	@return ID if it contains request tag otherwise 0
	     */
        public static int GetVAR_Request_ID(String mailText)
        {
            if (mailText == null)
                return 0;
            int indexStart = mailText.IndexOf(TAG_START);
            if (indexStart == -1)
                return 0;
            int indexEnd = mailText.IndexOf(TAG_END, indexStart);
            if (indexEnd == -1)
                return 0;
            //
            indexStart += 5;
            String idString = mailText.Substring(indexStart, indexEnd);
            int VAR_Request_ID = 0;
            try
            {
                VAR_Request_ID = int.Parse(idString);
            }
            catch (Exception e)
            {
                _log.Severe("Cannot parse " + idString + " Err" + e.Message);
            }
            return VAR_Request_ID;
        }

        //	Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVARRequest).FullName);
        /** Request Tag Start				*/
        private const String TAG_START = "[Req#";
        /** Request Tag End					*/
        private const String TAG_END = "#ID]";

        private StringBuilder message = null;
        private String subject = "";
        private int mailText_ID = 0;

        /**************************************************************************
         * 	Constructor
         * 	@param ctx context
         * 	@param VAR_Request_ID request or 0 for new
         *	@param trxName transaction
         */
        public MVARRequest(Ctx ctx, int VAR_Request_ID, Trx trxName) :
            base(ctx, VAR_Request_ID, trxName)
        {

            if (VAR_Request_ID == 0)
            {
                SetDueType(DUETYPE_Due);
                //  SetSalesRep_ID (0);
                //	SetDocumentNo (null);
                SetConfidentialType(CONFIDENTIALTYPE_PublicInformation);	// A
                SetConfidentialTypeEntry(CONFIDENTIALTYPEENTRY_PublicInformation);	// A
                SetProcessed(false);
                SetRequestAmt(Env.ZERO);
                SetPriorityUser(PRIORITY_Low);
                //  SetVAR_Req_Type_ID (0);
                //  SetSummary (null);
                SetIsEscalated(false);
                SetIsSelfService(false);
                SetIsInvoiced(false);
            }
        }

        /**
         * 	SelfService Constructor
         * 	@param ctx context
         * 	@param SalesRep_ID SalesRep
         * 	@param VAR_Req_Type_ID request type
         * 	@param Summary summary
         * 	@param isSelfService self service
         *	@param trxName transaction
         */
        public MVARRequest(Ctx ctx, int SalesRep_ID,
            int VAR_Req_Type_ID, String Summary, Boolean isSelfService, Trx trxName)
            : this(ctx, 0, trxName)
        {
            Set_Value("SalesRep_ID", (int)SalesRep_ID);	//	could be 0
            Set_Value("VAR_Req_Type_ID", (int)VAR_Req_Type_ID);
            SetSummary(Summary);
            SetIsSelfService(isSelfService);
            GetRequestType();
            if (_requestType != null)
            {
                String ct = _requestType.GetConfidentialType();
                if (ct != null)
                {
                    SetConfidentialType(ct);
                    SetConfidentialTypeEntry(ct);
                }
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName transaction
         */
        public MVARRequest(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        public MVARRequest(Ctx ctx, IDataReader dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Import Constructor
         *	@param imp import
         */
        public MVARRequest(X_VAI_Request imp)
            : this(imp.GetCtx(), 0, imp.Get_TrxName())
        {

            PO.CopyValues(imp, this, imp.GetVAF_Client_ID(), imp.GetVAF_Org_ID());
        }

        /** Request Type				*/
        private MVARRequestType _requestType = null;
        /**	Changed						*/
        private bool _changed = false;
        /**	BPartner					*/
        private MVABBusinessPartner _partner = null;
        /** User/Contact				*/
        private MVAFUserContact _user = null;
        /** List of EMail Notices		*/
        private StringBuilder _emailTo = new StringBuilder();

        /** Separator line				*/
        public const String SEPARATOR =
            "\n---------.----------.----------.----------.----------.----------\n";

        private int _success = 0;
        private int _failure = 0;
        private int _notices = 0;


        /**************************************************************************
         * 	Set Default Request Type.
         */
        public void SetVAR_Req_Type_ID()
        {
            _requestType = MVARRequestType.GetDefault(GetCtx());
            if (_requestType == null)
            {
                log.Warning("No default found");
            }
            else
            {
                base.SetVAR_Req_Type_ID(_requestType.GetVAR_Req_Type_ID());
            }
        }

        /**
         * 	Set Default Request Status.
         */
        public void SetVAR_Req_Status_ID()
        {
            MVARReqStatus status = MVARReqStatus.GetDefault(GetCtx(), GetVAR_Req_Type_ID());
            if (status == null)
            {
                log.Warning("No default found");
                if (GetVAR_Req_Status_ID() != 0)
                    SetVAR_Req_Status_ID(0);
            }
            else
                SetVAR_Req_Status_ID(status.GetVAR_Req_Status_ID());
        }

        /**
         * 	Add To Result
         * 	@param Result
         */
        public void AddToResult(String Result)
        {
            String oldResult = GetResult();
            if (Result == null || Result.Length == 0)
            {
                ;
            }
            else if (oldResult == null || oldResult.Length == 0)
                SetResult(Result);
            else
                SetResult(oldResult + "\n-\n" + Result);
        }

        /**
         * 	Set DueType based on Date Next Action
         */
        public void SetDueType()
        {
            DateTime? due = GetDateNextAction();
            if (due == null)
                return;
            //
            int dueDateTolerance = GetRequestType().GetDueDateTolerance();
            DateTime overdue = TimeUtil.AddDays(due, dueDateTolerance);
            DateTime now = System.DateTime.Now;
            //
            String DueType = DUETYPE_Due;
            if (now < due)
            {
                DueType = DUETYPE_Scheduled;
            }
            else if (now > overdue)
            {
                DueType = DUETYPE_Overdue;
            }
            base.SetDueType(DueType);
        }

        /*
         * 	Get Action History
         *	@return array of actions
         */
        public MVARReqHistory[] GetActions()
        {
            String sql = "SELECT * FROM VAR_Req_History "
                + "WHERE VAR_Request_ID= " + GetVAR_Request_ID()
                + " ORDER BY Created DESC";
            List<MVARReqHistory> list = new List<MVARReqHistory>();
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVARReqHistory(GetCtx(), dr, Get_TrxName()));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            //
            MVARReqHistory[] retValue = new MVARReqHistory[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Updates
         * 	@param confidentialType maximum confidential type - null = all
         *	@return updates
         */
        public MVARRequestUpdate[] GetUpdates(String confidentialType)
        {
            String sql = "SELECT * FROM VAR_Req_Update "
                + "WHERE VAR_Request_ID= " + GetVAR_Request_ID()
                + " ORDER BY Created DESC";
            List<MVARRequestUpdate> list = new List<MVARRequestUpdate>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MVARRequestUpdate ru = new MVARRequestUpdate(GetCtx(), dr, Get_TrxName());
                    if (confidentialType != null)
                    {
                        //	Private only if private
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PrivateInformation)
                            && !confidentialType.Equals(CONFIDENTIALTYPEENTRY_PrivateInformation))
                            continue;
                        //	Internal not if Customer/Public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_Internal)
                            && (confidentialType.Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                                || confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation)))
                            continue;
                        //	No Customer if public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                            && confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation))
                            continue;
                    }
                    list.Add(ru);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            //
            MVARRequestUpdate[] retValue = new MVARRequestUpdate[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        //Added By Manjot 09 July 2015
        public MVARRequestUpdate[] GetUpdatedRecord(String confidentialType)
        {
            //String sql = "SELECT * FROM VAR_Req_Update "
            //    + "WHERE VAR_Request_ID= " + GetVAR_Request_ID()
            //    + " ORDER BY Created DESC";

            String sql = @"SELECT * FROM VAR_Req_Update
                    WHERE VAR_Req_Update_ID = (SELECT MAX(VAR_Req_Update_ID) FROM VAR_Req_Update
                    WHERE VAR_Request_ID= " + GetVAR_Request_ID()
                    + " ) ORDER BY Created DESC";

            List<MVARRequestUpdate> list = new List<MVARRequestUpdate>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MVARRequestUpdate ru = new MVARRequestUpdate(GetCtx(), dr, Get_TrxName());
                    if (confidentialType != null)
                    {
                        //	Private only if private
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PrivateInformation)
                            && !confidentialType.Equals(CONFIDENTIALTYPEENTRY_PrivateInformation))
                            continue;
                        //	Internal not if Customer/Public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_Internal)
                            && (confidentialType.Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                                || confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation)))
                            continue;
                        //	No Customer if public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                            && confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation))
                            continue;
                    }
                    list.Add(ru);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            //
            MVARRequestUpdate[] retValue = new MVARRequestUpdate[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
        // Manjot
        /**
         * 	Get Public Updates
         *	@return public updates
         */
        public MVARRequestUpdate[] GetUpdatesPublic()
        {
            return GetUpdates(CONFIDENTIALTYPE_PublicInformation);
        }

        /**
         * 	Get Customer Updates
         *	@return customer updates
         */
        public MVARRequestUpdate[] GetUpdatesCustomer()
        {
            return GetUpdates(CONFIDENTIALTYPE_PartnerConfidential);
        }

        /**
         * 	Get Internal Updates
         *	@return internal updates
         */
        public MVARRequestUpdate[] GetUpdatesInternal()
        {
            return GetUpdates(CONFIDENTIALTYPE_Internal);
        }

        /**
         *	Get Request Type
         *	@return Request Type 	
         */
        public MVARRequestType GetRequestType()
        {
            if (_requestType == null)
            {
                int VAR_Req_Type_ID = GetVAR_Req_Type_ID();
                if (VAR_Req_Type_ID == 0)
                {
                    SetVAR_Req_Type_ID();
                    VAR_Req_Type_ID = GetVAR_Req_Type_ID();
                }
                _requestType = MVARRequestType.Get(GetCtx(), VAR_Req_Type_ID);
            }
            return _requestType;
        }

        /**
         *	Get Request Type Text (for jsp)
         *	@return Request Type Text	
         */
        public String GetRequestTypeName()
        {
            if (_requestType == null)
                GetRequestType();
            if (_requestType == null)
                return "??";
            return _requestType.GetName();
        }

        /**
         * 	Get Request Category
         *	@return category
         */
        public MVARCategory GetCategory()
        {
            if (GetVAR_Category_ID() == 0)
                return null;
            return MVARCategory.Get(GetCtx(), GetVAR_Category_ID());
        }

        /**
         * 	Get Request Category Name
         *	@return name
         */
        public String GetCategoryName()
        {
            MVARCategory cat = GetCategory();
            if (cat == null)
                return "";
            return cat.GetName();
        }

        /**
         * 	Get Request Group
         *	@return group
         */
        public MVARGroup GetGroup()
        {
            if (GetR_Group_ID() == 0)
                return null;
            return MVARGroup.Get(GetCtx(), GetR_Group_ID());
        }

        /**
         * 	Get Request Group Name
         *	@return name
         */
        public String GetGroupName()
        {
            MVARGroup grp = GetGroup();
            if (grp == null)
                return "";
            return grp.GetName();
        }

        /**
         * 	Get Status
         *	@return status
         */
        public MVARReqStatus GetStatus()
        {
            if (GetVAR_Req_Status_ID() == 0)
                return null;
            return MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
        }

        /**
         * 	Get Request Status Name
         *	@return name
         */
        public String GetStatusName()
        {
            MVARReqStatus sta = GetStatus();
            if (sta == null)
                return "?";
            return sta.GetName();
        }

        /**
         * 	Get Request Resolution
         *	@return resolution
         */
        public MVARResolution GetResolution()
        {
            if (GetR_Resolution_ID() == 0)
                return null;
            return MVARResolution.Get(GetCtx(), GetR_Resolution_ID());
        }

        /**
         * 	Get Request Resolution Name
         *	@return name
         */
        public String GetResolutionName()
        {
            MVARResolution res = GetResolution();
            if (res == null)
                return "";
            return res.GetName();
        }

        /**
         * 	Is Overdue
         *	@return true if overdue
         */
        public Boolean IsOverdue()
        {
            return DUETYPE_Overdue.Equals(GetDueType());
        }

        /**
         * 	Is due
         *	@return true if due
         */
        public Boolean IsDue()
        {
            return DUETYPE_Due.Equals(GetDueType());
        }

        /**
         * 	Get DueType Text (for jsp)
         *	@return text
         */
        public String GetDueTypeText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), DUETYPE_VAF_Control_Ref_ID, GetDueType());
        }

        /**
         * 	Get Priority Text (for jsp)
         *	@return text
         */
        public String GetPriorityText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), PRIORITY_VAF_Control_Ref_ID, GetPriority());
        }

        /**
         * 	Get Importance Text (for jsp)
         *	@return text
         */
        public String GetPriorityUserText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), PRIORITYUSER_VAF_Control_Ref_ID, GetPriorityUser());
        }

        /**
         * 	Get Confidential Text (for jsp)
         *	@return text
         */
        public String GetConfidentialText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), CONFIDENTIALTYPE_VAF_Control_Ref_ID, GetConfidentialType());
        }

        /**
         * 	Get Confidential Entry Text (for jsp)
         *	@return text
         */
        public String GetConfidentialEntryText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), CONFIDENTIALTYPEENTRY_VAF_Control_Ref_ID, GetConfidentialTypeEntry());
        }

        /**
         * 	Set Date Last Alert to today
         */
        public void SetDateLastAlert()
        {
            //    base.SetDateLastAlert(new DateTime((CommonFunctions.CurrentTimeMillis()) * TimeSpan.TicksPerMillisecond));
            base.SetDateLastAlert(DateTime.Now);
        }

        /**
         * 	Get Sales Rep
         *	@return Sales Rep User
         */
        public MVAFUserContact GetSalesRep()
        {
            if (GetSalesRep_ID() == 0)
                return null;
            return MVAFUserContact.Get(GetCtx(), GetSalesRep_ID());
        }

        /**
         * 	Get Sales Rep Name
         *	@return Sales Rep User
         */
        public String GetSalesRepName()
        {
            MVAFUserContact sr = GetSalesRep();
            if (sr == null)
                return "n/a";
            return sr.GetName();
        }

        /**
         * 	Get Name of creator
         *	@return name
         */
        public String GetCreatedByName()
        {
            MVAFUserContact user = MVAFUserContact.Get(GetCtx(), GetCreatedBy());
            return user.GetName();
        }

        /**
         * 	Get Contact (may be not defined)
         *	@return Sales Rep User
         */
        public MVAFUserContact GetUser()
        {
            if (GetVAF_UserContact_ID() == 0)
                return null;
            if (_user != null && _user.GetVAF_UserContact_ID() != GetVAF_UserContact_ID())
                _user = null;
            if (_user == null)
                _user = new MVAFUserContact(GetCtx(), GetVAF_UserContact_ID(), Get_TrxName());
            return _user;
        }

        /**
         * 	Set Business Partner - Callout
         *	@param oldVAF_UserContact_ID old value
         *	@param newVAF_UserContact_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetVAF_UserContact_ID(String oldVAF_UserContact_ID, String newVAF_UserContact_ID, int windowNo)
        {
            if (newVAF_UserContact_ID == null || newVAF_UserContact_ID.Length == 0)
                return;
            int VAF_UserContact_ID = int.Parse(newVAF_UserContact_ID);
            base.SetVAF_UserContact_ID(VAF_UserContact_ID);
            if (VAF_UserContact_ID == 0)
                return;

            if (GetVAB_BusinessPartner_ID() == 0)
            {
                MVAFUserContact user = new MVAFUserContact(GetCtx(), VAF_UserContact_ID, null);
                SetVAB_BusinessPartner_ID(user.GetVAB_BusinessPartner_ID());
            }
        }

        /**
         * 	Get BPartner (may be not defined)
         *	@return Sales Rep User
         */
        public MVABBusinessPartner GetBPartner()
        {
            if (GetVAB_BusinessPartner_ID() == 0)
                return null;
            if (_partner != null && _partner.GetVAB_BusinessPartner_ID() != GetVAB_BusinessPartner_ID())
                _partner = null;
            if (_partner == null)
                _partner = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_TrxName());
            return _partner;
        }

        /**
         * 	Web Can Update Request
         *	@return true if Web can update
         */
        public Boolean IsWebCanUpdate()
        {
            if (IsProcessed())
                return false;
            if (GetVAR_Req_Status_ID() == 0)
                SetVAR_Req_Status_ID();
            if (GetVAR_Req_Status_ID() == 0)
                return false;
            MVARReqStatus status = MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
            if (status == null)
                return false;
            return status.IsWebCanUpdate();
        }

        /// <summary>
        /// Set Priority
        /// </summary>
        private void SetPriority()
        {
            if (GetPriorityUser() == null)
                SetPriorityUser(PRIORITYUSER_Low);
            //
            if (GetBPartner() != null)
            {
                MVABBPartCategory bpg = MVABBPartCategory.Get(GetCtx(), GetBPartner().GetVAB_BPart_Category_ID());
                String prioBase = bpg.GetPriorityBase();
                if (prioBase != null && !prioBase.Equals(X_VAB_BPart_Category.PRIORITYBASE_Same))
                {
                    char tarGetPrio = Convert.ToChar(GetPriorityUser().Substring(0, 1));
                    if (prioBase.Equals(X_VAB_BPart_Category.PRIORITYBASE_Lower))
                    {
                        tarGetPrio = Convert.ToChar((char.GetNumericValue(tarGetPrio) + 2).ToString());
                    }
                    else
                    {
                        tarGetPrio = Convert.ToChar((char.GetNumericValue(tarGetPrio) - 2).ToString());
                    }
                    if (tarGetPrio < Convert.ToChar(PRIORITY_High.Substring(0, 1)))	//	1
                    {
                        tarGetPrio = Convert.ToChar(PRIORITY_High.Substring(0, 1));
                    }
                    if (tarGetPrio > Convert.ToChar(PRIORITY_Low.Substring(0, 1)))	//	9
                    {
                        tarGetPrio = Convert.ToChar(PRIORITY_Low.Substring(0, 1));
                    }
                    if (GetPriority() == null)
                        SetPriority(tarGetPrio.ToString());
                    else	//	previous priority
                    {
                        if (tarGetPrio < Convert.ToChar(GetPriority().Substring(0, 1)))
                        {
                            SetPriority(tarGetPrio.ToString());
                        }
                    }
                }
            }
            //	Same if nothing else
            if (GetPriority() == null)
                SetPriority(GetPriorityUser());
        }

        /**
         * 	Set Confidential Type Entry
         *	@param ConfidentialTypeEntry confidentiality
         */
        public new void SetConfidentialTypeEntry(String ConfidentialTypeEntry)
        {
            if (ConfidentialTypeEntry == null)
                ConfidentialTypeEntry = GetConfidentialType();
            //
            if (CONFIDENTIALTYPE_Internal.Equals(GetConfidentialType()))
                base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_Internal);
            else if (CONFIDENTIALTYPE_PrivateInformation.Equals(GetConfidentialType()))
            {
                if (CONFIDENTIALTYPE_Internal.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PrivateInformation.Equals(ConfidentialTypeEntry))
                    base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
                else
                    base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_PrivateInformation);
            }
            else if (CONFIDENTIALTYPE_PartnerConfidential.Equals(GetConfidentialType()))
            {
                if (CONFIDENTIALTYPE_Internal.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PrivateInformation.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PartnerConfidential.Equals(ConfidentialTypeEntry))
                    base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
                else
                    base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_PartnerConfidential);
            }
            else if (CONFIDENTIALTYPE_PublicInformation.Equals(GetConfidentialType()))
                base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
        }

        /**
         * 	Web Update
         *	@param result result
         *	@return true if updated
         */
        public Boolean WebUpdate(String result)
        {
            MVARReqStatus status = MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
            if (!status.IsWebCanUpdate())
                return false;
            if (status.GetUpdate_Status_ID() > 0)
                SetVAR_Req_Status_ID(status.GetUpdate_Status_ID());
            SetResult(result);
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVARRequest[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo()).Append("]");
            return sb.ToString();
        }

        /**
         * 	Create PDF
         *	@return pdf or null
         */
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

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, GetVAB_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
        }

        /**************************************************************************
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Request Type
            GetRequestType();
            if (newRecord || Is_ValueChanged("VAR_Req_Type_ID"))
            {
                if (_requestType != null)
                {
                    if (IsInvoiced() != _requestType.IsInvoiced())
                        SetIsInvoiced(_requestType.IsInvoiced());
                    if (GetDateNextAction() == null && _requestType.GetAutoDueDateDays() > 0)
                        SetDateNextAction(TimeUtil.AddDays(DateTime.Now,
                            _requestType.GetAutoDueDateDays()));
                }
                //	Is Status Valid
                if (GetVAR_Req_Status_ID() != 0)
                {
                    MVARReqStatus sta = MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
                    MVARRequestType rt = MVARRequestType.Get(GetCtx(), GetVAR_Req_Type_ID());
                    if (sta.GetVAR_Req_StatusCategory_ID() != rt.GetVAR_Req_StatusCategory_ID())
                        SetVAR_Req_Status_ID();	//	Set to default
                }
            }
            // Start Plan Date And End Plan Date 
            if (GetCloseDate() < GetStartDate())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EnddategrtrthnStartdate"));
                return false;
            }
            if (GetDateCompletePlan() < GetDateStartPlan())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EndPlandategrtrthnStartdate"));
                return false;
            }
            //	Request Status
            if (GetVAR_Req_Status_ID() == 0)
                SetVAR_Req_Status_ID();
            //	Validate/Update Due Type
            SetDueType();
            MVARReqStatus status = MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
            //	Close/Open
            if (status != null)
            {
                if (status.IsOpen())
                {
                    if (GetStartDate() == null)
                        SetStartDate(DateTime.Now);
                    if (GetCloseDate() != null)
                        SetCloseDate(null);
                }
                if (status.IsClosed()
                    && GetCloseDate() == null)
                    SetCloseDate(DateTime.Now);
                if (status.IsFinalClose())
                    SetProcessed(true);
            }

            //	Confidential Info
            if (GetConfidentialType() == null)
            {
                GetRequestType();
                if (_requestType != null)
                {
                    String ct = _requestType.GetConfidentialType();
                    if (ct != null)
                        SetConfidentialType(ct);
                }
                if (GetConfidentialType() == null)
                    SetConfidentialType(CONFIDENTIALTYPEENTRY_PublicInformation);
            }
            if (GetConfidentialTypeEntry() == null)
                SetConfidentialTypeEntry(GetConfidentialType());
            else
                SetConfidentialTypeEntry(GetConfidentialTypeEntry());

            //	Importance / Priority
            SetPriority();

            //	New
            if (newRecord)
                return true;

            //	Change Log
            _changed = false;
            List<String> sendInfo = new List<String>();
            MVARReqHistory ra = new MVARReqHistory(this, false);
            //
            if (CheckChange(ra, "VAR_Req_Type_ID"))
                sendInfo.Add("VAR_Req_Type_ID");
            if (CheckChange(ra, "VAR_Group_ID"))
                sendInfo.Add("VAR_Group_ID");
            if (CheckChange(ra, "VAR_Category_ID"))
                sendInfo.Add("VAR_Category_ID");
            if (CheckChange(ra, "VAR_Req_Status_ID"))
                sendInfo.Add("VAR_Req_Status_ID");
            if (CheckChange(ra, "VAR_Resolution_ID"))
                sendInfo.Add("VAR_Resolution_ID");
            //
            if (CheckChange(ra, "SalesRep_ID"))
            {
                //	Sender
                int VAF_UserContact_ID = p_ctx.GetVAF_UserContact_ID();
                if (VAF_UserContact_ID == 0)
                    VAF_UserContact_ID = GetUpdatedBy();
                //	Old
                Object oo = Get_ValueOld("SalesRep_ID");
                int oldSalesRep_ID = 0;
                if (oo is int)
                {
                    oldSalesRep_ID = ((int)oo);
                }
                if (oldSalesRep_ID != 0)
                {
                    //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
                    Object[] args = new Object[] {GetDocumentNo(),
                        MVAFUserContact.GetNameOfUser(VAF_UserContact_ID),
                        MVAFUserContact.GetNameOfUser(oldSalesRep_ID),
                        MVAFUserContact.GetNameOfUser(GetSalesRep_ID())
                        };
                    String msg = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
                    AddToResult(msg);
                    sendInfo.Add("SalesRep_ID");
                }
            }
            CheckChange(ra, "VAF_Role_ID");
            //
            if (CheckChange(ra, "Priority"))
                sendInfo.Add("Priority");
            if (CheckChange(ra, "PriorityUser"))
                sendInfo.Add("PriorityUser");
            if (CheckChange(ra, "IsEscalated"))
                sendInfo.Add("IsEscalated");
            //
            CheckChange(ra, "ConfidentialType");
            if (CheckChange(ra, "Summary"))
                sendInfo.Add("Summary");
            CheckChange(ra, "IsSelfService");
            CheckChange(ra, "VAB_BusinessPartner_ID");
            CheckChange(ra, "VAF_UserContact_ID");
            CheckChange(ra, "VAB_Project_ID");
            CheckChange(ra, "VAA_Asset_ID");
            CheckChange(ra, "VAB_Order_ID");
            CheckChange(ra, "VAB_Invoice_ID");
            CheckChange(ra, "VAM_Product_ID");
            CheckChange(ra, "VAB_Payment_ID");
            CheckChange(ra, "VAM_Inv_InOut_ID");
            //	checkChange(ra, "VAB_Promotion_ID");
            //	checkChange(ra, "RequestAmt");
            CheckChange(ra, "IsInvoiced");
            CheckChange(ra, "VAB_BillingCode_ID");
            CheckChange(ra, "DateNextAction");
            CheckChange(ra, "VAM_ProductSpent_ID");
            CheckChange(ra, "QtySpent");
            CheckChange(ra, "QtyInvoiced");
            CheckChange(ra, "StartDate");
            CheckChange(ra, "CloseDate");
            CheckChange(ra, "TaskStatus");
            CheckChange(ra, "DateStartPlan");
            CheckChange(ra, "DateCompletePlan");
            //
            if (_changed)
            {
                if (sendInfo.Count > 0)
                {
                    // get the columns which were changed.
                    string colsChanged = getChangedString(sendInfo);
                    ra.SetChangedValues(colsChanged);
                }
                ra.Save();
            }

            //	Current Info
            MVARRequestUpdate update = new MVARRequestUpdate(this);
            if (update.IsNewInfo())
                update.Save();
            else
                update = null;
            //
            // check mail templates from request or request type.
            if (GetVAR_MailTemplate_ID() > 0)
            {
                mailText_ID = GetVAR_MailTemplate_ID();
            }
            if (mailText_ID == 0)
            {
                MVARRequestType reqType = new MVARRequestType(GetCtx(), GetVAR_Req_Type_ID(), null);
                if (reqType.GetVAR_MailTemplate_ID() > 0)
                {
                    mailText_ID = reqType.GetVAR_MailTemplate_ID();
                }

            }


            if (mailText_ID == 0)
            {
                _emailTo = new StringBuilder();
                if (update != null || sendInfo.Count > 0)
                {
                    prepareNotificMsg(sendInfo);
                    //	Update
                    if (ra != null)
                        SetDateLastAction(ra.GetCreated());
                    SetLastResult(GetResult());
                    SetDueType();
                    //	ReSet
                    SetConfidentialTypeEntry(GetConfidentialType());
                    SetStartDate(null);
                    SetEndTime(null);
                    SetVAR_Req_StandardReply_ID(0);
                    SetVAR_MailTemplate_ID(0);
                    SetResult(null);
                    //	SetQtySpent(null);
                    //	SetQtyInvoiced(null);




                }
            }
            else
            {
                // get message if mail template is found.
                prepareNotificMsg(sendInfo);
            }

            return true;
        }

        /// <summary>
        /// get string of the changed columns
        /// </summary>
        /// <param name="sendInfo">list of columns changed.</param>
        /// <returns>return the comma separated string.</returns>
        private string getChangedString(List<string> sendInfo)
        {
            StringBuilder colString = null;
            if (sendInfo.Count > 0)
            {
                colString = new StringBuilder();
                for (int i = 0; i < sendInfo.Count; i++)
                {
                    if (i == 0)
                    {
                        colString.Append(sendInfo[i]);
                    }
                    else
                    {
                        colString.Append(',').Append(sendInfo[i]);
                    }

                }
            }
            return colString.ToString();
        }


        /// <summary>
        /// Prepare the notification message before going to after save.
        /// </summary>
        /// <param name="list"></param>
        private void prepareNotificMsg(List<String> list)
        {
            if (mailText_ID == 0)
            {
                message = new StringBuilder();
                //		UpdatedBy: Joe
                int UpdatedBy = GetCtx().GetVAF_UserContact_ID();
                MVAFUserContact from = MVAFUserContact.Get(GetCtx(), UpdatedBy);
                if (from != null)
                    message.Append(Msg.Translate(GetCtx(), "UpdatedBy")).Append(": ")
                        .Append(from.GetName());
                //		LastAction/Created: ...	
                if (GetDateLastAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateLastAction"))
                        .Append(": ").Append(GetDateLastAction());
                else
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "Created"))
                        .Append(": ").Append(GetCreated());
                //	Changes
                for (int i = 0; i < list.Count; i++)
                {
                    String columnName = (String)list[i];
                    message.Append("\n").Append(Msg.GetElement(GetCtx(), columnName))
                        .Append(": ").Append(Get_DisplayValue(columnName, false))
                        .Append(" -> ").Append(Get_DisplayValue(columnName, true));
                }
                //	NextAction
                if (GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(GetDateNextAction());
                message.Append(SEPARATOR)
                    .Append(GetSummary());
                if (GetResult() != null)
                    message.Append("\n----------\n").Append(GetResult());
                message.Append(GetMailTrailer(null));
            }
            else
            {
                message = new StringBuilder();

                MVARRequest _req = new MVARRequest(GetCtx(), GetVAR_Request_ID(), null);
                MVARMailTemplate text = new MVARMailTemplate(GetCtx(), mailText_ID, null);
                text.SetPO(_req, true); //Set _Po Current value
                subject += GetDocumentNo() + ": " + text.GetMailHeader();

                message.Append(text.GetMailText(true));
                if (GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(GetDateNextAction());

                // message.Append(GetMailTrailer(null));
            }
        }



        /// <summary>
        /// Send notice to role
        /// </summary>
        /// <param name="list">change information</param>
        private List<int> SendRoleNotice()
        {
            List<int> _users = new List<int>();
            string sql = @"SELECT VAF_UserContact.VAF_UserContact_ID,
                         VAF_UserContact_Roles.VAF_Role_ID
                        FROM VAF_UserContact_Roles
                        INNER JOIN VAF_UserContact
                        ON (VAF_UserContact_Roles.VAF_UserContact_ID    =VAF_UserContact.VAF_UserContact_ID)
                        WHERE VAF_UserContact_Roles.VAF_Role_ID IN
                          (SELECT VAF_Role_ID
                          FROM VAR_Rtype_UpdatesAlert
                          WHERE VAF_Role_ID   IS NOT NULL
                          AND VAR_Req_Type_ID=" + GetVAR_Req_Type_ID() + @"
                          AND IsActive        ='Y'
                          )
                        AND VAF_UserContact_Roles.VAF_UserContact_ID NOT IN
                          (SELECT u.VAF_UserContact_ID
                          FROM RV_RequestUpdates_Only ru
                          INNER JOIN VAF_UserContact u
                          ON (ru.VAF_UserContact_ID=u.VAF_UserContact_ID)
                          LEFT OUTER JOIN VAF_UserContact_Roles r
                          ON (u.VAF_UserContact_ID     =r.VAF_UserContact_ID)
                          WHERE ru.VAR_Request_ID=" + GetVAR_Request_ID() + @"
                          )
                        AND VAF_UserContact.email IS NOT NULL";

            DataSet _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                _users = validateUsers(_ds);
            }
            return _users;

        }


        /// <summary>
        /// Validate the organization access of users according to the role.
        /// </summary>
        /// <param name="_ds"></param>
        /// <returns></returns>
        private List<int> validateUsers(DataSet _ds)
        {
            List<int> users = new List<int>();
            MVAFRole role = new MVAFRole(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[0]["VAF_Role_ID"]), null);
            bool isAllUser = false;
            // if access all organization
            if (role.IsAccessAllOrgs())
            {
                isAllUser = true;
            }
            // if not access user organization access.
            if (!isAllUser && !role.IsUseUserOrgAccess())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Org_ID) FROm VAF_Role_OrgRights WHERE IsActive='Y' AND  VAF_Role_ID=" + role.GetVAF_Role_ID() + " AND VAF_Org_ID IN (" + GetVAF_Org_ID() + ",0)")) > 0)
                {
                    isAllUser = true;
                }
                else
                {
                    return users;
                }
            }
            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {
                if (isAllUser)
                {
                    users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                }
                else
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_Org_ID) FROm VAF_UserContact_OrgRights WHERE VAF_UserContact_ID=" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]) + " AND  IsActive='Y' AND  VAF_Org_ID IN (" + GetVAF_Org_ID() + ",0)")) > 0)
                    {
                        users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["VAF_UserContact_ID"]));
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Send notifications to users
        /// </summary>
        /// <param name="newRec">if new record or not</param>
        private void SendNotifications(bool newRec)
        {
            if (newRec)
            {
                prepareNotificMsg(new List<String>());
                Thread thread = new Thread(new ThreadStart(() => SendNotices(new List<String>())));
                thread.Start();
            }
            else
            {
                _changed = false;
                List<String> sendInfo = new List<String>();
                MVARReqHistory ra = new MVARReqHistory(this, false);
                //
                if (CheckChange(ra, "VAR_Req_Type_ID"))
                    sendInfo.Add("VAR_Req_Type_ID");
                if (CheckChange(ra, "VAR_Group_ID"))
                    sendInfo.Add("VAR_Group_ID");
                if (CheckChange(ra, "VAR_Category_ID"))
                    sendInfo.Add("VAR_Category_ID");
                if (CheckChange(ra, "VAR_Req_Status_ID"))
                    sendInfo.Add("VAR_Req_Status_ID");
                if (CheckChange(ra, "VAR_Resolution_ID"))
                    sendInfo.Add("VAR_Resolution_ID");
                //
                if (CheckChange(ra, "SalesRep_ID"))
                {
                    //	Sender
                    int VAF_UserContact_ID = p_ctx.GetVAF_UserContact_ID();
                    if (VAF_UserContact_ID == 0)
                        VAF_UserContact_ID = GetUpdatedBy();
                    //	Old
                    Object oo = Get_ValueOld("SalesRep_ID");
                    int oldSalesRep_ID = 0;
                    if (oo is int)
                    {
                        oldSalesRep_ID = ((int)oo);
                    }
                    if (oldSalesRep_ID != 0)
                    {
                        //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
                        Object[] args = new Object[] {GetDocumentNo(),
                        MVAFUserContact.GetNameOfUser(VAF_UserContact_ID),
                        MVAFUserContact.GetNameOfUser(oldSalesRep_ID),
                        MVAFUserContact.GetNameOfUser(GetSalesRep_ID())
                        };
                        String msg = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
                        AddToResult(msg);
                        sendInfo.Add("SalesRep_ID");
                    }
                }
                CheckChange(ra, "VAF_Role_ID");
                //
                if (CheckChange(ra, "Priority"))
                    sendInfo.Add("Priority");
                if (CheckChange(ra, "PriorityUser"))
                    sendInfo.Add("PriorityUser");
                if (CheckChange(ra, "IsEscalated"))
                    sendInfo.Add("IsEscalated");
                //
                CheckChange(ra, "ConfidentialType");
                if (CheckChange(ra, "Summary"))
                    sendInfo.Add("Summary");
                CheckChange(ra, "IsSelfService");
                CheckChange(ra, "VAB_BusinessPartner_ID");
                CheckChange(ra, "VAF_UserContact_ID");
                CheckChange(ra, "VAB_Project_ID");
                CheckChange(ra, "VAA_Asset_ID");
                CheckChange(ra, "VAB_Order_ID");
                CheckChange(ra, "VAB_Invoice_ID");
                CheckChange(ra, "VAM_Product_ID");
                CheckChange(ra, "VAB_Payment_ID");
                CheckChange(ra, "VAM_Inv_InOut_ID");
                //	checkChange(ra, "VAB_Promotion_ID");
                //	checkChange(ra, "RequestAmt");
                CheckChange(ra, "IsInvoiced");
                CheckChange(ra, "VAB_BillingCode_ID");
                CheckChange(ra, "DateNextAction");
                CheckChange(ra, "VAM_ProductSpent_ID");
                CheckChange(ra, "QtySpent");
                CheckChange(ra, "QtyInvoiced");
                CheckChange(ra, "StartDate");
                CheckChange(ra, "CloseDate");
                CheckChange(ra, "TaskStatus");
                CheckChange(ra, "DateStartPlan");
                CheckChange(ra, "DateCompletePlan");
                //
                //if (_changed)
                //    ra.Save();

                //	Current Info
                MVARRequestUpdate update = new MVARRequestUpdate(this);
                if (update.IsNewInfo())
                    update.Save();
                else
                    update = null;
                //
                if (mailText_ID == 0)
                {
                    _emailTo = new StringBuilder();
                    if (update != null || sendInfo.Count > 0)
                    {

                        // For Role Changes
                        Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                        thread.Start();
                    }
                }
                else
                {
                    // For Role Changes
                    Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                    thread.Start();
                }
            }
        }



        /**
         * 	Check for changes
         *	@param ra request action
         *	@param columnName column
         *	@return true if changes
         */
        private Boolean CheckChange(MVARReqHistory ra, String columnName)
        {
            if (Is_ValueChanged(columnName))
            {
                Object value = Get_ValueOld(columnName);
                if (value == null)
                    ra.AddNullColumn(columnName);
                else
                    ra.Set_ValueNoCheck(columnName, value);
                _changed = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Get Column Value
        /// </summary>
        /// <param name="columnName"> Column name</param>
        /// <returns>Returns value of the column.</returns>
        public string getColumnValue(string columnName)
        {
            return Get_DisplayValue(columnName, true);
        }
        /**
         *  Check the ability to send email.
         *  @return VAF_Msg_Lable or null if no error
         */
        private String CheckEMail()
        {
            //  Mail Host
            MVAFClient client = MVAFClient.Get(GetCtx());
            if (client == null
                || client.GetSmtpHost() == null
                || client.GetSmtpHost().Length == 0)
                return "RequestActionEMailNoSMTP";

            //  Mail To
            MVAFUserContact to = new MVAFUserContact(GetCtx(), GetVAF_UserContact_ID(), Get_TrxName());
            if (to == null
                || to.GetEMail() == null
                || to.GetEMail().Length == 0)
                return "RequestActionEMailNoTo";

            //  Mail From real user
            MVAFUserContact from = MVAFUserContact.Get(GetCtx(), GetCtx().GetVAF_UserContact_ID());
            if (from == null
                || from.GetEMail() == null
                || from.GetEMail().Length == 0)
                return "RequestActionEMailNoFrom";

            //  Check that UI user is Request User
            //int realSalesRep_ID = Env.GetVAF_UserContact_ID(GetCtx());
            //if (realSalesRep_ID != GetSalesRep_ID())
            //    SetSalesRep_ID(realSalesRep_ID);

            //  RequestActionEMailInfo - EMail from {0} to {1}
            //Object[] args = new Object[] { emailFrom, emailTo };
            //String msg = Msg.GetMsg(GetCtx(),"RequestActionEMailInfo");
            //SetLastResult(msg);
            return null;
        }

        /**
         * 	Set SalesRep_ID
         *	@param SalesRep_ID id
         */
        public new void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID != 0)
                base.SetSalesRep_ID(SalesRep_ID);
            else if (GetSalesRep_ID() != 0)
            { }
            log.Warning("Ignored - Tried to Set SalesRep_ID to 0 from " + GetSalesRep_ID());
        }

        /**
         * 	Set MailText - Callout
         *	@param oldVAR_MailTemplate_ID old value
         *	@param newVAR_MailTemplate_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetVAR_MailTemplate_ID(String oldVAR_MailTemplate_ID, String newVAR_MailTemplate_ID, int windowNo)
        {
            if (newVAR_MailTemplate_ID == null || newVAR_MailTemplate_ID.Length == 0)
                return;
            int VAR_MailTemplate_ID = int.Parse(newVAR_MailTemplate_ID);
            base.SetVAR_MailTemplate_ID(VAR_MailTemplate_ID);
            if (VAR_MailTemplate_ID == 0)
                return;

            MVARMailTemplate mt = new MVARMailTemplate(GetCtx(), VAR_MailTemplate_ID, null);
            if (mt.Get_ID() == VAR_MailTemplate_ID)
            {
                String txt = mt.GetMailText();
                txt = Env.ParseContext(GetCtx(), windowNo, txt, false, true);
                SetResult(txt);
            }
        }

        /**
         * 	Set Standard Response - Callout
         *	@param oldVAR_Req_StandardReply_ID old value
         *	@param newVAR_Req_StandardReply_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetVAR_Req_StandardReply_ID(String oldVAR_Req_StandardReply_ID,
                String newVAR_Req_StandardReply_ID, int windowNo)
        {
            if (newVAR_Req_StandardReply_ID == null || newVAR_Req_StandardReply_ID.Length == 0)
                return;
            int VAR_Req_StandardReply_ID = int.Parse(newVAR_Req_StandardReply_ID);
            base.SetVAR_Req_StandardReply_ID(VAR_Req_StandardReply_ID);
            if (VAR_Req_StandardReply_ID == 0)
                return;

            MVARReqStandardReply sr = new MVARReqStandardReply(GetCtx(), VAR_Req_StandardReply_ID, null);
            if (sr.Get_ID() == VAR_Req_StandardReply_ID)
            {
                String txt = sr.GetResponseText();
                txt = Env.ParseContext(GetCtx(), windowNo, txt, false, true);
                SetResult(txt);
            }
        }

        /**
         * 	Set Request Type - Callout
         *	@param oldVAR_Req_Type_ID old value
         *	@param newVAR_Req_Type_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetVAR_Req_Type_ID(String oldVAR_Req_Type_ID,
                String newVAR_Req_Type_ID, int windowNo)
        {
            if (newVAR_Req_Type_ID == null || newVAR_Req_Type_ID.Length == 0)
                return;
            int VAR_Req_Type_ID = int.Parse(newVAR_Req_Type_ID);
            base.SetVAR_Req_Type_ID(VAR_Req_Type_ID);
            if (VAR_Req_Type_ID == 0)
                return;

            MVARRequestType rt = MVARRequestType.Get(GetCtx(), VAR_Req_Type_ID);
            int VAR_Req_Status_ID = rt.GetDefaultVAR_Req_Status_ID();
            SetVAR_Req_Status_ID(VAR_Req_Status_ID);
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
                return success;

            //	Create Update
            if (newRecord && GetResult() != null)
            {
                MVARRequestUpdate update = new MVARRequestUpdate(this);
                update.Save();
            }
            MVARRequestType reqType = new MVARRequestType(GetCtx(), GetVAR_Req_Type_ID(), null);
            //	Initial Mail
            if (reqType.Get_ID() > 0 && reqType.IsR_AllowSaveNotify())
            {
                SendNotifications(newRecord);
            }
            //SendNotices(new List<String>());

            //	ChangeRequest - created in Request Processor
            if (GetM_ChangeRequest_ID() != 0
                && Is_ValueChanged("VAR_Group_ID"))	//	different ECN assignment?
            {
                int oldID = Convert.ToInt32(Get_ValueOld("VAR_Group_ID"));
                if (GetR_Group_ID() == 0)
                    SetM_ChangeRequest_ID(0);	//	not effective as in afterSave
                else
                {
                    MVARGroup oldG = MVARGroup.Get(GetCtx(), oldID);
                    MVARGroup newG = MVARGroup.Get(GetCtx(), GetR_Group_ID());
                    if (oldG.GetVAM_BOM_ID() != newG.GetVAM_BOM_ID()
                        || oldG.GetM_ChangeNotice_ID() != newG.GetM_ChangeNotice_ID())
                    {
                        MVAMChangeRequest ecr = new MVAMChangeRequest(GetCtx(), GetM_ChangeRequest_ID(), Get_TrxName());
                        if (!ecr.IsProcessed()
                            || ecr.GetM_FixChangeNotice_ID() == 0)
                        {
                            ecr.SetVAM_BOM_ID(newG.GetVAM_BOM_ID());
                            ecr.SetM_ChangeNotice_ID(newG.GetM_ChangeNotice_ID());
                            ecr.Save();
                        }
                    }
                }
            }

            //if (_emailTo.Length > 0)
            // {
            //log.SaveInfo("RequestActionEMailOK", _emailTo.ToString());

            //}

            if (reqType.Get_ID() > 0 && reqType.IsR_AllowSaveNotify())
            {
                log.SaveInfo("R_EmailSentBackgrnd", "");
            }
            return success;
        }

        /// <summary>
        /// 	Send transfer Message
        /// </summary>
        private void SendTransferMessage()
        {
            //	Sender
            int VAF_UserContact_ID = p_ctx.GetVAF_UserContact_ID();
            if (VAF_UserContact_ID == 0)
                VAF_UserContact_ID = GetUpdatedBy();
            //	Old
            Object oo = Get_ValueOld("SalesRep_ID");
            int oldSalesRep_ID = 0;
            //if (oo.GetType() == typeof(int))
            if (oo is int)
            {
                oldSalesRep_ID = int.Parse(oo.ToString());
            }

            //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
            Object[] args = new Object[] {GetDocumentNo(),
                    MVAFUserContact.GetNameOfUser(VAF_UserContact_ID),
                    MVAFUserContact.GetNameOfUser(oldSalesRep_ID),
                    MVAFUserContact.GetNameOfUser(GetSalesRep_ID())
                    };
            String subject = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
            String message = subject + "\n" + GetSummary();
            MVAFClient client = MVAFClient.Get(GetCtx(), GetVAF_Client_ID());
            MVAFUserContact from = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
            MVAFUserContact to = MVAFUserContact.Get(GetCtx(), GetSalesRep_ID());
            //
            client.SendEMail(from, to, subject, message, CreatePDF());
        }

        /**
         * 	Send Update EMail/Notices
         * 	@param list list of changes
         */
        protected void SendNotices(List<String> list)
        {
            bool isEmailSent = false;
            StringBuilder finalMsg = new StringBuilder();
            finalMsg.Append(Msg.Translate(GetCtx(), "VAR_Request_ID") + ": " + GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NotificSent"));
            //	Subject
            if (mailText_ID == 0)
            {
                subject = Msg.Translate(GetCtx(), "VAR_Request_ID")
                   + " " + Msg.GetMsg(GetCtx(), "Updated", true) + ": " + GetDocumentNo() + " (●" + MVAFTableView.Get_Table_ID(Table_Name) + "-" + GetVAR_Request_ID() + "●) " + Msg.GetMsg(GetCtx(), "DoNotChange");
            }
            //	Message

            //		UpdatedBy: Joe
            int UpdatedBy = GetCtx().GetVAF_UserContact_ID();
            MVAFUserContact from = MVAFUserContact.Get(GetCtx(), UpdatedBy);

            FileInfo pdf = CreatePDF();
            log.Finer(message.ToString());

            //	Prepare sending Notice/Mail
            MVAFClient client = MVAFClient.Get(GetCtx(), GetVAF_Client_ID());
            //	ReSet from if external
            if (from.GetEMailUser() == null || from.GetEMailUserPW() == null)
                from = null;
            _success = 0;
            _failure = 0;
            _notices = 0;

            /** List of users - aviod duplicates	*/
            List<int> userList = new List<int>();
            String sql = "SELECT u.VAF_UserContact_ID, u.NotificationType, u.EMail, u.Name, MAX(r.VAF_Role_ID) "
                + "FROM RV_RequestUpdates_Only ru"
                + " INNER JOIN VAF_UserContact u ON (ru.VAF_UserContact_ID=u.VAF_UserContact_ID)"
                + " LEFT OUTER JOIN VAF_UserContact_Roles r ON (u.VAF_UserContact_ID=r.VAF_UserContact_ID) "
                + "WHERE ru.VAR_Request_ID= " + GetVAR_Request_ID()
                + " GROUP BY u.VAF_UserContact_ID, u.NotificationType, u.EMail, u.Name";

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int VAF_UserContact_ID = Utility.Util.GetValueOfInt(idr[0]);
                    String NotificationType = Util.GetValueOfString(idr[1]); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_EMail;
                    String email = Util.GetValueOfString(idr[2]);// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = Util.GetValueOfString(idr[3]);//idr.GetString(3);
                    //	Role
                    int VAF_Role_ID = Utility.Util.GetValueOfInt(idr[4]);
                    if (idr == null)
                    {
                        VAF_Role_ID = -1;
                    }

                    //	Don't send mail to oneself
                    //		if (VAF_UserContact_ID == UpdatedBy)
                    //			continue;

                    //	No confidential to externals
                    if (VAF_Role_ID == -1
                        && (GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPE_Internal)
                            || GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPE_PrivateInformation)))
                        continue;

                    if (X_VAF_UserContact.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }
                    if ((X_VAF_UserContact.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                        || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
                        && (email == null || email.Length == 0))
                    {
                        if (VAF_Role_ID >= 0)
                            NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_Notice;
                        else
                        {
                            log.Config("No EMail: " + Name);
                            continue;
                        }
                    }
                    if (X_VAF_UserContact.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                        && VAF_Role_ID >= 0)
                    {
                        log.Config("No internal User: " + Name);
                        continue;
                    }

                    //	Check duplicate receivers
                    int ii = VAF_UserContact_ID;
                    if (userList.Contains(ii))
                        continue;
                    userList.Add(ii);

                    // check the user roles for organization access.
                    MVAFUserContact user = new MVAFUserContact(GetCtx(), VAF_UserContact_ID, null);
                    MVAFRole[] role = user.GetRoles(GetVAF_Org_ID());
                    if (role.Length == 0)
                        continue;


                    //
                    SendNoticeNow(VAF_UserContact_ID, NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                idr.Close();
                // Notification For Role
                List<int> _users = SendRoleNotice();
                for (int i = 0; i < _users.Count; i++)
                {
                    MVAFUserContact user = new MVAFUserContact(GetCtx(), _users[i], null);
                    int VAF_UserContact_ID = user.GetVAF_UserContact_ID();
                    String NotificationType = user.GetNotificationType(); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_EMail;
                    String email = user.GetEMail();// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = user.GetName();//idr.GetString(3);
                                                 //	Role                  

                    if (X_VAF_UserContact.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }

                    //
                    SendNoticeNow(_users[i], NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                if (!isEmailSent)
                {
                    finalMsg.Clear();
                    finalMsg.Append(Msg.Translate(GetCtx(), "VAR_Request_ID") + ": " + GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NoNotificationSent"));
                }

                int VAF_Msg_Lable_ID = 834;
                MVAFNotice note = new MVAFNotice(GetCtx(), VAF_Msg_Lable_ID, GetCtx().GetVAF_UserContact_ID(),
                    X_VAR_Request.Table_ID, GetVAR_Request_ID(),
                    subject, finalMsg.ToString(), Get_TrxName());
                if (note.Save())
                    log.Log(Level.INFO, "ProcessFinished", "");
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }


            //	New Sales Rep (may happen if sent from beforeSave
            if (!userList.Contains(GetSalesRep_ID()))
                SendNoticeNow(GetSalesRep_ID(), null,
                    client, from, subject, message.ToString(), pdf);

            log.Info("EMail Success=" + _success + ", Failure=" + _failure
                + " - Notices=" + _notices);
        }

        /**
         * 	Send Notice Now
         *	@param VAF_UserContact_ID	recipient
         *	@param NotificationType	optional notification type
         *	@param client client
         *	@param from sender
         *	@param subject subject
         *	@param message message
         *	@param pdf optional attachment
         */
        private void SendNoticeNow(int VAF_UserContact_ID, String NotificationType,
            MVAFClient client, MVAFUserContact from, String subject, String message, FileInfo pdf)
        {
            MVAFUserContact to = MVAFUserContact.Get(GetCtx(), VAF_UserContact_ID);
            if (NotificationType == null)
                NotificationType = to.GetNotificationType();
            //	Send Mail
            if (X_VAF_UserContact.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(GetCtx(), 0, null);
                _mAttachment.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                _mAttachment.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                _mAttachment.SetVAF_TableView_ID(MVAFTableView.Get_Table_ID(Table_Name));
                _mAttachment.IsActive();
                _mAttachment.SetMailAddress("");
                _mAttachment.SetAttachmentType("M");
                _mAttachment.SetRecord_ID(GetVAR_Request_ID());
                _mAttachment.SetTextMsg(message);
                _mAttachment.SetTitle(subject);
                _mAttachment.SetMailAddress(to.GetEMail());

                if (from != null && !string.IsNullOrEmpty(from.GetEMail()))
                {
                    _mAttachment.SetMailAddressFrom(from.GetEMail());
                }
                else
                {
                    _mAttachment.SetMailAddressFrom(client.GetRequestEMail());
                }

                _mAttachment.NewRecord();

                if (client.SendEMail(from, to, subject, message.ToString(), pdf))
                {
                    _success++;
                    if (_emailTo.Length > 0)
                        _emailTo.Append(", ");
                    _emailTo.Append(to.GetEMail());
                    _mAttachment.SetIsMailSent(true);
                }
                else
                {
                    log.Warning("Failed: " + to);
                    _failure++;
                    NotificationType = X_VAF_UserContact.NOTIFICATIONTYPE_Notice;
                    _mAttachment.SetIsMailSent(false);
                }

                _mAttachment.Save();
            }

            //	Send Note
            if (X_VAF_UserContact.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                || X_VAF_UserContact.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                int VAF_Msg_Lable_ID = 834;
                MVAFNotice note = new MVAFNotice(GetCtx(), VAF_Msg_Lable_ID, VAF_UserContact_ID,
                    X_VAR_Request.Table_ID, GetVAR_Request_ID(),
                    subject, message.ToString(), Get_TrxName());
                if (note.Save())
                    _notices++;
            }

        }

        /*****
         * 	Get MailID
         * 	@param serverAddress server address
         *	@return Mail Trailer
         */
        public String GetMailTrailer(String serverAddress)
        {
            StringBuilder sb = new StringBuilder("\n").Append(SEPARATOR)
                .Append(Msg.Translate(GetCtx(), "VAR_Request_ID"))
                .Append(": ").Append(GetDocumentNo())
                .Append("  ").Append(GetMailTag())
                .Append("\nSent by ViennaMail");
            if (serverAddress != null)
                sb.Append(" from ").Append(serverAddress);
            return sb.ToString();
        }


        /**
         * 	Get Mail Tag
         *	@return [Req@{id}@]
         */
        public String GetMailTag()
        {
            return TAG_START + Get_ID() + TAG_END;
        }

        /**
         * 	(Soft) Close request.
         * 	Must be called after webUpdate
         */
        public void DoClose()
        {
            MVARReqStatus status = MVARReqStatus.Get(GetCtx(), GetVAR_Req_Status_ID());
            if (!status.IsClosed())
            {
                MVARReqStatus[] closed = MVARReqStatus.GetClosed(GetCtx());
                MVARReqStatus newStatus = null;
                for (int i = 0; i < closed.Length; i++)
                {
                    if (!closed[i].IsFinalClose())
                    {
                        newStatus = closed[i];
                        break;
                    }
                }
                if (newStatus == null && closed.Length > 0)
                    newStatus = closed[0];
                if (newStatus != null)
                    SetVAR_Req_Status_ID(newStatus.GetVAR_Req_Status_ID());
            }
        }

        /**
         * 	Escalate request
         * 	@param user true if user escalated - otherwise system
         */
        public void DoEscalate(Boolean user)
        {
            if (user)
            {
                String Importance = GetPriorityUser();
                if (PRIORITYUSER_Urgent.Equals(Importance))
                {; }	//	high as it goes
                else if (PRIORITYUSER_High.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Urgent);
                else if (PRIORITYUSER_Medium.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_High);
                else if (PRIORITYUSER_Low.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Medium);
                else if (PRIORITYUSER_Minor.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Low);
            }
            else
            {
                String Importance = GetPriority();
                if (PRIORITY_Urgent.Equals(Importance))
                {; }	//	high as it goes
                else if (PRIORITY_High.Equals(Importance))
                    SetPriority(PRIORITY_Urgent);
                else if (PRIORITY_Medium.Equals(Importance))
                    SetPriority(PRIORITY_High);
                else if (PRIORITY_Low.Equals(Importance))
                    SetPriority(PRIORITY_Medium);
                else if (PRIORITY_Minor.Equals(Importance))
                    SetPriority(PRIORITY_Low);
            }
        }
    }
}