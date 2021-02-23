﻿/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_ExpectedCost
 * Chronological Development
 * Amit Bansal     04-Dec-2019
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABExpectedCost : X_VAB_ExpectedCost
    {

        #region Variable
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABExpectedCost).FullName);
        //
        private ValueNamePair pp = null;
        #endregion


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_ExpectedCost_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABExpectedCost(Ctx ctx, int VAB_ExpectedCost_ID, Trx trxName)
           : base(ctx, VAB_ExpectedCost_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVABExpectedCost(Ctx ctx, DataRow dr, Trx trxName)
           : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Implement Before Save 
        /// </summary>
        /// <param name="newRecord">true, when new record</param>
        /// <returns>true, when all condition set or passed</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // amount not be ZERO
            if(GetAmt() == 0)
            {
                log.SaveError("AmtCantbeZero", "");
                return false;
            }

            // check Unique constraints basedon Org + Order + cost element + landed cost distribution
            String sql = "SELECT COUNT(VAB_ExpectedCost_ID) FROM VAB_ExpectedCost WHERE IsActive = 'Y' AND VAF_Org_ID = " + GetVAF_Org_ID() +
                @" AND VAB_Order_ID = " + GetVAB_Order_ID() + @" AND VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() +
                @" AND LandedCostDistribution = '" + GetLandedCostDistribution() + "'";
            if (!newRecord)
            {
                sql += " AND VAB_ExpectedCost_ID <> " + GetVAB_ExpectedCost_ID();
            }
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (count > 0)
            {
                log.SaveError("VIS_DuplicateRecord", "");
                return false;
            }
            // end 

            return true;
        }

        /// <summary>
        /// Get Expected Landed Cost Lines
        /// </summary>
        /// <returns>lines</returns>
        public static MVABExpectedCost[] GetLines(Ctx ctx, int VAB_Order_ID, Trx trxName)
        {
            //	Lines					
            MVABExpectedCost[] _lines = null;
            List<MVABExpectedCost> list = new List<MVABExpectedCost>();
            String sql = "SELECT * FROM VAB_ExpectedCost WHERE IsActive = 'Y' AND VAB_Order_ID=" + VAB_Order_ID;
            DataSet ds = null;
            DataRow dr = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    list.Add(new MVABExpectedCost(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
                list = null;
            }
            ds = null;
            //
            if (list == null)
                return null;
            _lines = new MVABExpectedCost[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Order Lines having only product whihc is of Item type
        /// </summary>
        /// <param name="VAB_Order_ID">order id</param>
        /// <returns>order lines</returns>
        public MVABOrderLine[] GetLinesIteMVAMProduct(int VAB_Order_ID)
        {
            List<MVABOrderLine> list = new List<MVABOrderLine>();
            StringBuilder sql = new StringBuilder(@"SELECT * FROM VAB_OrderLine ol
                                                        INNER JOIN VAM_Product p ON p.VAM_Product_id = ol.VAM_Product_id
                                                        WHERE ol.VAB_Order_ID =" + VAB_Order_ID + @" AND ol.isactive = 'Y' 
                                                        AND  p.ProductType  = '" + MVAMProduct.PRODUCTTYPE_Item + "'");
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    MVABOrderLine ol = new MVABOrderLine(GetCtx(), dr, Get_TrxName());
                    list.Add(ol);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            //
            MVABOrderLine[] lines = new MVABOrderLine[list.Count]; ;
            lines = list.ToArray();
            return lines;
        }

        /// <summary>
        /// Distribute Cost
        /// </summary>
        /// <returns>if success then empty string else message</returns>
        public String DistributeLandedCost()
        {
            // default distribution line 
            MVABExpectedCostDis[] _expectedDistributionlines = null;

            // Delete expected distribution lines
            String sql = "DELETE FROM VAB_ExpectedCostDis WHERE  VAB_ExpectedCost_ID = " + GetVAB_ExpectedCost_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no != 0)
            {
                _log.Info(" VAB_ExpectedCostDis - Deleted #" + no);
            }

            // get order lines having only product
            MVABOrderLine[] orderLines = GetLinesIteMVAMProduct(GetVAB_Order_ID());

            // create expected cost distribution lines
            if (orderLines != null && orderLines.Length > 0)
            {
                List<MVABExpectedCostDis> list = new List<MVABExpectedCostDis>();
                for (int i = 0; i < orderLines.Length; i++)
                {
                    MVABExpectedCostDis allocation = new MVABExpectedCostDis(GetCtx(), 0, Get_Trx());
                    allocation.SetVAB_ExpectedCost_ID(GetVAB_ExpectedCost_ID());
                    allocation.SetVAB_OrderLine_ID(orderLines[i].GetVAB_OrderLine_ID());
                    allocation.SetClientOrg(GetVAF_Client_ID(), GetVAF_Org_ID());
                    allocation.SetAmt(Env.ZERO);
                    allocation.SetBase(Env.ZERO);
                    allocation.SetQty(Env.ZERO);
                    if (!allocation.Save())
                    {
                        pp = VLogger.RetrieveError();
                        if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                            return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved") + ", " + pp.GetName();
                        else
                            return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved");
                    }
                    else
                    {
                        list.Add(allocation);
                    }
                }
                if (list.Count > 0)
                {
                    _expectedDistributionlines = new MVABExpectedCostDis[list.Count];
                    _expectedDistributionlines = list.ToArray();
                }
            }

            if (_expectedDistributionlines != null && _expectedDistributionlines.Length == 1)
            {
                Decimal baseValue = orderLines[0].GetBase(GetLandedCostDistribution());
                if (baseValue == 0)
                {
                    StringBuilder msgreturn = new StringBuilder("Total of Base values is 0 - ").Append(GetLandedCostDistribution());
                    return msgreturn.ToString();
                }
                _expectedDistributionlines[0].SetBase(baseValue);
                _expectedDistributionlines[0].SetQty(orderLines[0].GetQtyOrdered());
                _expectedDistributionlines[0].SetAmt(GetAmt() , orderLines[0].GetPrecision());
                if (!_expectedDistributionlines[0].Save())
                {
                    pp = VLogger.RetrieveError();
                    if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                        return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved") + ", " + pp.GetName();
                    else
                        return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved");
                }
            }
            else if (_expectedDistributionlines != null && _expectedDistributionlines.Length > 1)
            {
                // get total
                Decimal total = Env.ZERO;
                for (int i = 0; i < orderLines.Length; i++)
                {
                    total = Decimal.Add(total, orderLines[i].GetBase(GetLandedCostDistribution()));
                }

                if (total == 0)
                {
                    StringBuilder msgreturn = new StringBuilder("Total of Base values is 0 - ").Append(GetLandedCostDistribution());
                    return msgreturn.ToString();
                }

                //	Create Allocations
                for (int i = 0; i < _expectedDistributionlines.Length; i++)
                {
                    MVABOrderLine orderLine = new MVABOrderLine(GetCtx(), _expectedDistributionlines[i].GetVAB_OrderLine_ID(), Get_Trx());
                    Decimal baseValue = orderLine.GetBase(GetLandedCostDistribution());
                    _expectedDistributionlines[i].SetBase(baseValue);
                    _expectedDistributionlines[i].SetQty(orderLine.GetQtyOrdered());
                    if (baseValue != 0)
                    {
                        Decimal result = Decimal.Multiply(GetAmt(), baseValue);
                        result = Decimal.Round(Decimal.Divide(result, total), orderLine.GetPrecision(), MidpointRounding.AwayFromZero);
                        _expectedDistributionlines[i].SetAmt(result);
                    }
                    if (!_expectedDistributionlines[i].Save())
                    {
                        pp = VLogger.RetrieveError();
                        if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                            return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved") + ", " + pp.GetName();
                        else
                            return Msg.GetMsg(GetCtx(), "ExpectedAllocationNotSaved");
                    }
                }
            }
            AllocateLandedCostRounding(_expectedDistributionlines);
            return "";
        }

        /// <summary>
        /// Allocate Landed Cost - Enforce Rounding
        /// </summary>
        /// <param name="_expectedDistributionlines">expected allocation lines</param>
        private void AllocateLandedCostRounding(MVABExpectedCostDis[] _expectedDistributionlines)
        {
            if (_expectedDistributionlines == null)
                return;
            MVABExpectedCostDis largestAmtAllocation = null;
            Decimal allocationAmt = Env.ZERO;
            for (int i = 0; i < _expectedDistributionlines.Length; i++)
            {
                MVABExpectedCostDis allocation = _expectedDistributionlines[i];
                if (largestAmtAllocation == null
                    || allocation.GetAmt().CompareTo(largestAmtAllocation.GetAmt()) > 0)
                    largestAmtAllocation = allocation;
                allocationAmt = Decimal.Add(allocationAmt, allocation.GetAmt());
            }
            Decimal difference = Decimal.Subtract(GetAmt(), allocationAmt);
            if (Env.Signum(difference) != 0)
            {
                largestAmtAllocation.SetAmt(Decimal.Add(largestAmtAllocation.GetAmt(), difference));
                largestAmtAllocation.Save();
                log.Config("Difference=" + difference
                    + ", VAB_ExpectedCostDis_ID=" + largestAmtAllocation.GetVAB_ExpectedCostDis_ID()
                    + ", Amt" + largestAmtAllocation.GetAmt());
            }
        }

    }
}