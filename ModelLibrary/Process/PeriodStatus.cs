﻿/********************************************************
* Project Name   : VAdvantage
* Class Name     : PeriodStatus
* Purpose        : 
* Class Used     : 
* Chronological    Development
* Raghunandan     25-Jun-2009
 ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class PeriodStatus : ProcessEngine.SvrProcess
    {
        //Period					
        private int _VAB_YearPeriod_ID = 0;
        //Action					
        private String _PeriodAction = null;
        //Organization					
        private string _VAF_Org_ID = null;
        // Document BaseType
        private string _docBaseType = null;

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {

                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    _VAF_Org_ID = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("DocBaseType"))
                {
                    _docBaseType = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("PeriodAction"))
                {
                    _PeriodAction = (String)para[i].GetParameter();
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAB_YearPeriod_ID = GetRecord_ID();
        }

        /**
         * 	Process
         *	@return message
         *	@throws Exception
         */
        protected override String DoIt()
        {
            //log.info("VAB_YearPeriod_ID=" + _VAB_YearPeriod_ID + ", PeriodAction=" + _PeriodAction);
            MPeriod period = new MPeriod(GetCtx(), _VAB_YearPeriod_ID, Get_TrxName());
            if (period.Get_ID() == 0)
                throw new Exception("@NotFound@  @VAB_YearPeriod_ID@=" + _VAB_YearPeriod_ID);

            StringBuilder sql = new StringBuilder("UPDATE VAB_YearPeriodControl ");
            sql.Append("SET PeriodStatus='");
            //	Open
            if (MPeriodControl.PERIODACTION_OpenPeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_Open);
            //	Close
            else if (MPeriodControl.PERIODACTION_ClosePeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_Closed);
            //	Close Permanently
            else if (MPeriodControl.PERIODACTION_PermanentlyClosePeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_PermanentlyClosed);
            else
                return "-";
            //
            sql.Append("', PeriodAction='N', Updated=SysDate,UpdatedBy=").Append(GetVAF_UserContact_ID());
            //	WHERE
            sql.Append(" WHERE VAB_YearPeriod_ID=").Append(period.GetVAB_YearPeriod_ID())
                .Append(" AND PeriodStatus<>'P'")
                .Append(" AND PeriodStatus<>'").Append(_PeriodAction).Append("'");

            // if organization is selected then update period control of selected organizations
            if (!String.IsNullOrEmpty(_VAF_Org_ID))
            {
                sql.Append(" AND VAF_Org_ID IN (" + _VAF_Org_ID + ")");
            }

            // if Document BaseType is selected then update period control for selected Document BaseType
            if (!String.IsNullOrEmpty(_docBaseType))
            {
                sql.Append(" AND DocBaseType IN (SELECT DocBaseType FROM VAB_MasterDocType WHERE VAB_MasterDocType_ID IN (" + _docBaseType + "))");
            }

            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());

            // Lokesh Chauhan 17-Dec-2013
            /* Change For setting date From in Balance Aggregation window
              For Fact Account Balance updation */
            if (no >= 0)
            {
                if (MPeriodControl.PERIODACTION_ClosePeriod.Equals(_PeriodAction))
                {
                    try
                    {
                        string sqlSchID = "SELECT VAB_AccountBook_ID FROM VAB_AccountBook WHERE IsActive = 'Y' AND VAF_Client_ID = " + GetCtx().GetVAF_Client_ID();
                        DataSet ds = DB.ExecuteDataset(sqlSchID);

                        if (ds != null)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int VAB_AccountBook_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]);
                                string sqlUpd = "UPDATE Actual_Accumulation SET DateFrom = " + DB.TO_DATE(period.GetStartDate().Value.AddDays(-1)) + " WHERE IsActive = 'Y' AND VAF_Client_ID = " + GetCtx().GetVAF_Client_ID();
                                no = DB.ExecuteQuery(sqlUpd, null, Get_TrxName());
                                if (Get_Trx().Commit())
                                {
                                    VAdvantage.Report.FinBalance.UpdateBalance(GetCtx(), VAB_AccountBook_ID, period.GetStartDate().Value.AddDays(-1), Get_TrxName(), 0, this);
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }

            CacheMgt.Get().Reset("VAB_YearPeriodControl", 0);
            CacheMgt.Get().Reset("VAB_YearPeriod", _VAB_YearPeriod_ID);
            return "@Period Updated@ #" + no;
        }
    }
}
