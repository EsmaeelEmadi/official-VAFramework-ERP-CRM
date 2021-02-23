﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRevenueRecognition
 * Purpose        : Revenue Recognition Model
 * Class Used     : X_VAB_Rev_Recognition
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Model
{
    /// <summary>
    /// Revenue Recognition Model
    /// </summary>
    public class MVABRevRecognition : X_VAB_Rev_Recognition
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABRevRecognition).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_Rev_Recognition_ID"></param>
        /// <param name="trxName"></param>
        public MVABRevRecognition(Ctx ctx, int VAB_Rev_Recognition_ID, Trx trxName)
            : base(ctx, VAB_Rev_Recognition_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVABRevRecognition(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRevRecognition(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// After Save Logic
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            // create default Account
            StringBuilder _sql = new StringBuilder();
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_RevenueRecognition_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Revenue Recognition'");
                var relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                PO assetGroupAcct = null;
                _sql.Clear();
                _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where IsActive = 'Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID());
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID() + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
                        DataSet ds = new DataSet();
                        ds = DB.ExecuteDataset(_sql.ToString(), null, Get_Trx());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (!_relatedTo.Equals("") && _relatedTo.Equals(relatedtoProduct))
                                {
                                    _sql.Clear();
                                    _sql.Append(@"Select count(*) From VAB_Rev_Recognition Bp
                                                       Left Join FRPT_RevenueRecognition_Acct  ca On Bp.VAB_Rev_Recognition_ID=ca.VAB_Rev_Recognition_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                                   + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + GetVAF_Client_ID() +
                                                   " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) +
                                                   " AND Bp.VAB_Rev_Recognition_ID = " + GetVAB_Rev_Recognition_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        assetGroupAcct = MVAFTableView.GetPO(GetCtx(), "FRPT_RevenueRecognition_Acct", 0, null);
                                        assetGroupAcct.Set_ValueNoCheck("VAF_Org_ID", 0);
                                        assetGroupAcct.Set_ValueNoCheck("VAB_Rev_Recognition_ID", Util.GetValueOfInt(GetVAB_Rev_Recognition_ID()));
                                        assetGroupAcct.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        assetGroupAcct.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                        assetGroupAcct.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
                                        if (!assetGroupAcct.Save())
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            log.Log(Level.SEVERE, "Could Not create FRPT_Asset_Groip_Acct. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// This Function is used to get RevenueRecognition Records
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="trx">trx</param>
        /// <returns>Array of MRevenueRecognition</returns>
        public static MVABRevRecognition[] GetRecognitions(Ctx ctx, Trx trx)
        {
            List<MVABRevRecognition> list = new List<MVABRevRecognition>();
            string sql = "Select * From VAB_Rev_Recognition Where VAF_Client_ID=" + ctx.GetVAF_Client_ID();

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABRevRecognition(ctx, dr, trx));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MVABRevRecognition[] retValue = new MVABRevRecognition[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// This function is used to create Recognition plan and run
        /// </summary>
        /// <param name="VAB_InvoiceLine_ID">invoice line</param>
        /// <param name="VAB_Rev_Recognition_ID">Revenue Recognition</param>
        /// <param name="Invoice">Invoice</param>
        /// <returns>true, when success</returns>
        public static bool CreateRevenueRecognitionPlan(int VAB_InvoiceLine_ID, int VAB_Rev_Recognition_ID, MVABInvoice Invoice)
        {
            try
            {
                MVABRevRecognitionRun revenueRecognitionRun = null;
                DateTime? RecognizationDate = null;
                int NoofMonths = 0;
                MVABRevRecognition revenueRecognition = new MVABRevRecognition(Invoice.GetCtx(), VAB_Rev_Recognition_ID, Invoice.Get_Trx());
                int defaultAccSchemaOrg_ID = GetDefaultActSchema(Invoice.GetCtx(), Invoice.GetVAF_Client_ID(), Invoice.GetVAF_Org_ID());
                int ToCurrency = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Currency_ID FROM VAB_AccountBook WHERE VAB_AccountBook_ID=" + defaultAccSchemaOrg_ID));

                MVABInvoiceLine invoiceLine = new MVABInvoiceLine(Invoice.GetCtx(), VAB_InvoiceLine_ID, Invoice.Get_Trx());
                RecognizationDate = Util.GetValueOfDateTime(invoiceLine.Get_Value("RevenueStartDate"));

                // precision to be handle based on std precision defined on acct schema
                string sql = "SELECT C.StdPrecision FROM VAB_AccountBook a INNER JOIN VAB_Currency c ON c.VAB_Currency_ID= a.VAB_Currency_ID WHERE a.VAB_AccountBook_ID=" + defaultAccSchemaOrg_ID;
                int stdPrecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));


                if (revenueRecognition.GetRecognitionFrequency().Equals(RECOGNITIONFREQUENCY_Month) && RecognizationDate.Value.Day != 1 && revenueRecognition.GetNoMonths() > 0)
                {
                    //if startdate is in between day of month
                    NoofMonths = revenueRecognition.GetNoMonths() + 1;
                }
                else
                {
                    //if start date is the first day of the month
                    NoofMonths = revenueRecognition.GetNoMonths();
                }
                MVABRevRecognitionStrtgy revenueRecognitionPlan = new MVABRevRecognitionStrtgy(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                revenueRecognitionPlan.SetRecognitionPlan(invoiceLine, Invoice, VAB_Rev_Recognition_ID, ToCurrency);
                revenueRecognitionPlan.SetVAB_AccountBook_ID(defaultAccSchemaOrg_ID);
                revenueRecognitionPlan.SetRecognizedAmt(0);
                if (!revenueRecognitionPlan.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    string error = pp != null ? pp.GetValue() : "";
                    if (pp != null && string.IsNullOrEmpty(error))
                    {
                        error = pp.GetName();
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        _log.Log(Level.SEVERE, error);
                        return false;
                    }
                }
                else
                {
                    if (!revenueRecognition.IsTimeBased())
                    {

                    }
                    else
                    {
                        if (revenueRecognition.GetRecognitionFrequency().Equals(RECOGNITIONFREQUENCY_Month))
                        {

                            decimal totaldays = Util.GetValueOfDecimal((RecognizationDate.Value.AddMonths(revenueRecognition.GetNoMonths()) - RecognizationDate.Value.Date).TotalDays);

                            decimal perdayAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / (totaldays > 0 ? totaldays : 1), 12);
                            decimal recognizedAmt = 0;
                            DateTime? lastdate = null;
                            int days = 0;
                            for (int i = 0; i < NoofMonths; i++)
                            {
                                if (i == 0)
                                {
                                    if (RecognizationDate.Value.Month == 12)
                                    {
                                        //last date of the month 
                                        lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month, 1).AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        //last date of the month
                                        lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month + 1, 1).AddDays(-1);
                                    }
                                    days = Util.GetValueOfInt((lastdate.Value.Date - RecognizationDate.Value.Date).TotalDays);
                                    days += 1;
                                }
                                else if (i == (revenueRecognition.GetNoMonths()))
                                {
                                    //last date of the month would the day before  the recoganizationdate
                                    lastdate = RecognizationDate.Value.AddMonths(i).AddDays(-1);
                                    DateTime startDate = new DateTime(lastdate.Value.Year, lastdate.Value.Month, 1);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - startDate.Date).TotalDays);
                                    days += 1;
                                }
                                else
                                {
                                    DateTime startDate = lastdate.Value.AddDays(1);
                                    days = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                                    lastdate = startDate.AddDays(days - 1);
                                }
                                recognizedAmt = Math.Round(days * perdayAmt, stdPrecision);
                                revenueRecognitionRun = new MVABRevRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(lastdate);
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                                recognizedAmt = 0;
                            }
                        }
                        else if (revenueRecognition.GetRecognitionFrequency().Equals(RECOGNITIONFREQUENCY_Day))
                        {
                            Decimal recognizedAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / revenueRecognition.GetNoMonths(), stdPrecision);
                            int days = 0;
                            for (int i = 0; i < revenueRecognition.GetNoMonths(); i++)
                            {
                                revenueRecognitionRun = new MVABRevRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(RecognizationDate.Value.AddDays(days));
                                days += 1;
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (revenueRecognition.GetRecognitionFrequency().Equals(RECOGNITIONFREQUENCY_Year))
                        {
                            DateTime? fstartDate = null;
                            DateTime? fendDate = null;
                            int calendar_ID = 0;
                            DataSet ds = new DataSet();

                            calendar_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Calender_ID FROM VAF_OrgDetail WHERE vaf_org_id = " + Invoice.GetVAF_Org_ID()));
                            if (calendar_ID == 0)
                            {
                                calendar_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAB_Calender_ID FROM VAF_ClientDetail WHERE vaf_client_id = " + Invoice.GetVAF_Client_ID()));
                            }
                            sql = "SELECT startdate , enddate FROM VAB_YearPeriod WHERE " +
                                "VAB_Year_id = (SELECT VAB_Year.VAB_Year_id FROM VAB_Year INNER JOIN VAB_YearPeriod ON VAB_Year.VAB_Year_id = VAB_YearPeriod.VAB_Year_id " +
                                "WHERE  VAB_Year.VAB_Calender_id =" + calendar_ID + " AND " + GlobalVariable.TO_DATE(RecognizationDate, true) + " BETWEEN VAB_YearPeriod.startdate AND VAB_YearPeriod.enddate) AND periodno IN (1, 12)";
                            ds = DB.ExecuteDataset(sql);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                fstartDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["startdate"]);
                                fendDate = Convert.ToDateTime(ds.Tables[0].Rows[1]["enddate"]);
                            }
                            if (fstartDate != RecognizationDate)
                            {
                                //RecognizationDate  is not same as financial year's start date
                                NoofMonths += 1;
                            }
                            decimal totaldays = Util.GetValueOfDecimal((RecognizationDate.Value.AddYears(revenueRecognition.GetNoMonths()) - RecognizationDate.Value.Date).TotalDays);

                            decimal perdayAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / (totaldays > 0 ? totaldays : 1), 12);
                            decimal recognizedAmt = 0;
                            DateTime? lastdate = null;
                            int days = 0;
                            for (int i = 0; i < NoofMonths; i++)
                            {
                                if (i == 0)
                                {
                                    //last date will always be financial year's end date 
                                    lastdate = fendDate;
                                    days = Util.GetValueOfInt((lastdate.Value.Date - RecognizationDate.Value.Date).TotalDays);
                                    days += 1;
                                }
                                else if (i == revenueRecognition.GetNoMonths())
                                {
                                    //last date of the year would the day before the recoganizationdate
                                    lastdate = RecognizationDate.Value.AddYears(i).AddDays(-1);
                                    DateTime startDate = fstartDate.Value.AddYears(i);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - startDate.Date).TotalDays);
                                    days += 1;
                                }
                                else
                                {
                                    lastdate = fendDate.Value.AddYears(i);
                                    DateTime _startDate = fstartDate.Value.AddYears(i);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - _startDate.Date).TotalDays);
                                    days += 1;
                                }
                                recognizedAmt = Math.Round(days * perdayAmt, stdPrecision);
                                revenueRecognitionRun = new MVABRevRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(lastdate);
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                                recognizedAmt = 0;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Severe("Exception during creation of Recognition Plan and Run. " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function is used to get Accounting Schema either binded on Organization or Primary Accounting SChema
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="vaf_client_ID">VAF_Client_ID</param>
        /// <param name="VAF_Org_ID">Org ID</param>
        /// <returns>VAB_AccountBook ID</returns>
        public static int GetDefaultActSchema(Ctx ctx, int vaf_client_ID, int VAF_Org_ID)
        {
            MVABAccountBook acctSchema = null;
            if (VAF_Org_ID > 0)
            {
                acctSchema = MVAFOrg.Get(ctx, VAF_Org_ID).GetAcctSchema();
            }
            if (acctSchema == null)
            {
                acctSchema = MVAFClient.Get(ctx, vaf_client_ID).GetAcctSchema();
            }
            return acctSchema.GetVAB_AccountBook_ID();
        }

    }
}