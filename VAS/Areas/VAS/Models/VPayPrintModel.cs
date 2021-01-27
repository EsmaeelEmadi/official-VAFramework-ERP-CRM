﻿/********************************************************
 * Module Name    : VIS
 * Purpose        : Print payment Selection
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     18 May 2015
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class VPayPrintModel
    {
        String[] bp = null;
        String[] bpba = null;

        private static long serialVersionUID = 1L;
        //Window No	
        private int m_WindowNo = 0;
        private bool _printCheck = false;
        private int _VAB_PaymentOption_ID = 0;
        //Used Bank Account	
        private int _VAB_Bank_Acct_ID = 0;
        //Logger
        private static VLogger log = VLogger.GetVLogger(typeof(VPayPrintModel));
        //Payment Information
        public VAdvantage.Model.MPaySelectionCheck[] m_checks = null;
        //Payment Batch	
        public VAdvantage.Model.MPaymentBatch m_batch = null;
        public List<int> payment_ID = null;
        //VAdvantage.DSProcessWorkflow.ReportInfo rep = null;


        /** BPartner Info Index for Value       */
        private const int BP_VALUE = 0;
        /** BPartner Info Index for Name        */
        private const int BP_NAME = 1;
        /** BPartner Info Index for Contact Name    */
        private const int BP_CONTACT = 2;
        /** BPartner Info Index for Address 1   */
        private const int BP_ADDR1 = 3;
        /** BPartner Info Index for Address 2   */
        private const int BP_ADDR2 = 4;
        /** BPartner Info Index for City        */
        private const int BP_CITY = 5;
        /** BPartner Info Index for Region      */
        private const int BP_REGION = 6;
        /** BPartner Info Index for Postal Code */
        private const int BP_POSTAL = 7;
        /** BPartner Info Index for Country     */
        private const int BP_COUNTRY = 8;
        /** BPartner Info Index for Reference No    */
        private const int BP_REFNO = 9;


        /** BankAccount Info Index for RoutingNo       */
        private const int BPBA_RoutingNo = 0;
        /** BankAccount Info Index for AccountNo       */
        private const int BPBA_AccountNo = 1;
        /** BankAccount Info Index for AccountName     */
        private const int BPBA_AName = 2;
        /** BankAccount Info Index for AccountCity     */
        private const int BPBA_ACity = 3;
        /** BankAccount Info Index for BBAN            */
        private const int BPBA_BBAN = 4;
        /** BankAccount Info Index for IBAN            */
        private const int BPBA_IBAN = 5;
        /** BankAccount Info Index for Bank Name       */
        private const int BA_Name = 6;
        /** BankAccount Info Index for Bank RoutingNo  */
        private const int BA_RoutingNo = 7;
        /** BankAccount Info Index for Bank SwiftCode  */
        private const int BA_SwitftCode = 8;
        /// <summary>
        /// Get Detail for Payment Print Form
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public PaymentPrintDetail GetDetail(Ctx ctx, bool printCheck, int VAB_PaymentOption_ID, bool isFirstTime)
        {
            _printCheck = printCheck;
            _VAB_PaymentOption_ID = VAB_PaymentOption_ID;
            PaymentPrintDetail objPPrintDetail = new PaymentPrintDetail();
            VPaySelectModel objPSelectModel = new VPaySelectModel();
            objPPrintDetail.PaymentSelection = GetPaymentSelection(ctx, isFirstTime);
            objPPrintDetail.PSelectInfo = GetPSelectInfo(ctx);
            objPPrintDetail.PaymentMethod = objPSelectModel.GetPaymentMethod(ctx, _VAB_Bank_Acct_ID);
            return objPPrintDetail;
        }
        /// <summary>
        /// GetPaymentSelection
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="isFirstTime"></param>
        /// <returns></returns>
        private List<PaymentSelection> GetPaymentSelection(Ctx ctx, bool isFirstTime)
        {
            List<PaymentSelection> objPSelection = new List<PaymentSelection>();
            StringBuilder sql = new StringBuilder("SELECT VAB_PaymentOption_ID, Name || ' - ' || " + DB.TO_CHAR("TotalAmt",
                      DisplayType.Number, ctx.GetVAF_Language()) + " AS NAME FROM VAB_PaymentOption "
                      + "WHERE VAF_Client_ID=" + ctx.GetVAF_Client_ID() + " AND Processed='Y' AND IsActive='Y'");
            if (!_printCheck)
            {
                sql.Append(" AND EXISTS (SELECT * FROM VAB_PaymentOptionCheck psc" +
                           " WHERE psc.VAB_PaymentOption_ID = VAB_PaymentOption.VAB_PaymentOption_ID	AND psc.IsPrinted = 'N') ");
            }
            sql.Append("ORDER BY PayDate DESC, VAB_PaymentOption_ID DESC");
            DataSet ds = new DataSet();
            ds = DB.ExecuteDataset(sql.ToString(), null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (isFirstTime)
                    {
                        _VAB_PaymentOption_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_PaymentOption_ID"]);
                    }
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        objPSelection.Add(new PaymentSelection()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_PaymentOption_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                        });
                    }
                }
            }
            return objPSelection;
        }
        /// <summary>
        /// Get Information like BankAccount,CurrentBalance,Currency etc
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private List<PSelectInfo> GetPSelectInfo(Ctx ctx)
        {
            List<PSelectInfo> objPSelectInfo = new List<PSelectInfo>();
            String sql = "SELECT ps.VAB_Bank_Acct_ID, b.Name || ' ' || ba.AccountNo AS NAME,"	//	1..2
               + " c.ISO_Code, CurrentBalance "										//	3..4
               + "FROM VAB_PaymentOption ps"
               + " INNER JOIN VAB_Bank_Acct ba ON (ps.VAB_Bank_Acct_ID=ba.VAB_Bank_Acct_ID)"
               + " INNER JOIN C_Bank b ON (ba.C_Bank_ID=b.C_Bank_ID)"
               + " INNER JOIN VAB_Currency c ON (ba.VAB_Currency_ID=c.VAB_Currency_ID) "
               + "WHERE ps.VAB_PaymentOption_ID=" + _VAB_PaymentOption_ID + " AND ps.Processed='Y' AND ba.IsActive='Y'";
            DataSet ds = new DataSet();
            ds = DB.ExecuteDataset(sql.ToString(), null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    _VAB_Bank_Acct_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["VAB_Bank_Acct_ID"]);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        objPSelectInfo.Add(new PSelectInfo()
                        {
                            BankAccount_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_Bank_Acct_ID"]),
                            BankAccount = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                            Currency = Convert.ToString(ds.Tables[0].Rows[i]["ISO_Code"]),
                            CurrentBalance = Convert.ToString(ds.Tables[0].Rows[i]["CurrentBalance"])
                        });
                    }
                }
            }
            return objPSelectInfo;
        }

        /// <summary>
        /// PaymentRule changed - load DocumentNo, NoPayments,
        ///  enable/disable EFT, Print
        /// </summary>
        public PInfo LoadPaymentRuleInfo(Ctx ctx, string paymentMethod_ID, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule)
        {

            PInfo objPInfo = new PInfo();


            String sql = "SELECT COUNT(*) "
                + "FROM VAB_PaymentOptionCheck "
                + "WHERE VAB_PaymentOption_ID=" + VAB_PaymentOption_ID;

            //  DocumentNo
            String sql1 = "SELECT CurrentNext "
                + "FROM VAB_Bank_AcctDoc "
                + "WHERE VAB_Bank_Acct_ID=" + m_VAB_Bank_Acct_ID + " AND PaymentRule  = '" + paymentMethod_ID + "' AND IsActive='Y'";


            int count = DB.GetSQLValue(null, sql);

            int next = DB.GetSQLValue(null, sql1);
            objPInfo.NoOfPayments = Util.GetValueOfString(count);
            // btnEFT.IsEnabled = PaymentRule.Equals("T");

            if (next != 0)
            {
                objPInfo.CheckNo = next.ToString();
            }
            else
            {
                objPInfo.Msg = "VPayPrintNoDoc";
            }


            return objPInfo;
        }

        /// <summary>
        /// Print Checks and/or Remittance
        /// </summary>
        public Cmd_Print Cmd_Print(Ctx ctx, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule, int startDocumentNo)
        {

            Cmd_Print objCmdPrint = new Cmd_Print();
            VAdvantage.ProcessEngine.ProcessCtl pctrl = null;
            bool res = false;
            log.Info(PaymentRule);


            log.Config("VAB_PaymentOption_ID=" + VAB_PaymentOption_ID + ", PaymentRule=" + PaymentRule + ", DocumentNo=" + startDocumentNo);


            m_checks = VAdvantage.Model.MPaySelectionCheck.Get(VAB_PaymentOption_ID, PaymentRule, startDocumentNo, null);

            if (m_checks == null || m_checks.Length == 0)
            {
                //ShowMessage.Error("VPayPrintNoRecords", null, "(" + Msg.Translate(Envs.GetCtx(), "VAB_PaymentOptionLine_ID") + " #0");
                res = false;
                //return false;
            }
            else
            {
                res = true;
            }
            m_batch = VAdvantage.Model.MPaymentBatch.GetForPaySelection(ctx, VAB_PaymentOption_ID, null);
            if (!m_batch.Save())
            {

            }
            //return true;
            if (!res)
            {
                return null;
            }

            bool somethingPrinted = false;
            bool directPrint = !Ini.IsPropertyBool(Ini.P_PRINTPREVIEW);
            Dictionary<VAdvantage.Model.MPaySelectionCheck, String> oldCheckValues = new Dictionary<VAdvantage.Model.MPaySelectionCheck, string>();
            int checkID = 0;
            int VAB_Payment_ID = 0;
            List<int> check_ID = new List<int>();
            foreach (VAdvantage.Model.MPaySelectionCheck check in m_checks)
            {
                //	ReportCtrl will check BankAccountDoc for PrintFormat
                oldCheckValues.Add(check, Util.GetValueOfString(check.Get_ValueOld("CheckNo")));
                check.Save();
                checkID = Util.GetValueOfInt(check.Get_ID());
                check_ID.Add(checkID);
                if (!somethingPrinted)
                {
                    somethingPrinted = true;
                }
            }

            int table_ID = 0;
            int VAF_Job_ID = 0;
            string sql = "";
            int paymentTable_ID = 0;
            int paymentVAF_Job_ID = 0;
            try
            {

                sql = "select vaf_tableview_id from vaf_tableview where tablename = 'VAB_PaymentOptionCheck'";
                table_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                //sql = "select VAF_Job_id from VAF_Job where value = 'CheckPrint'";
                sql = "select VAF_Job_id from VAF_Job where VAF_Print_Rpt_Layout_id = (select check_printformat_id from VAB_Bank_Acctdoc where VAB_Bank_Acct_id = (select VAB_Bank_Acct_id from VAB_Payment where VAB_Payment_id = (select VAB_Payment_id from VAB_PaymentOptioncheck where VAB_PaymentOptioncheck_id = " + checkID + ")) and VAB_Bank_Acctdoc.isactive = 'Y' and rownum =1)";
                VAF_Job_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                sql = "select vaf_tableview_id from vaf_tableview where tablename = 'VAB_Payment'";
                paymentTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                sql = "select VAF_Job_id from VAF_Job where value = 'PaymentPrintFormat'";
                paymentVAF_Job_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                // int paymentVAF_Job_ID = 313;
            }
            catch
            {

            }

            //for (int j = 0; j < check_ID.Count; j++)
            //{

            //    int record_ID = Util.GetValueOfInt(checkID);
            //  VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo(null, VAF_Job_ID, table_ID, Util.GetValueOfInt(check_ID[j]));
            //    pi.SetVAF_UserContact_ID(ctx.GetVAF_UserContact_ID());
            //    pi.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
            //    byte[] reportData = null;
            //    string result="";
            //    pctrl = new VAdvantage.ProcessEngine.ProcessCtl(ctx,null, pi, null);
            //    pctrl.Process(pi, ctx,out reportData,out result);

            //}
            objCmdPrint.check_ID = check_ID;
            objCmdPrint.m_batch = m_batch.GetVAB_PaymentBatch_ID();
            return objCmdPrint;

        }
        /// <summary>
        /// ContinueCheckPrint
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_PaymentOption_ID"></param>
        /// <param name="m_VAB_Bank_Acct_ID"></param>
        /// <param name="PaymentRule"></param>
        /// <param name="startDocumentNo"></param>
        /// <param name="check_ID"></param>
        /// <param name="m_checks"></param>
        /// <param name="m_batch"></param>
        /// <returns></returns>
        public List<int> ContinueCheckPrint(Ctx ctx, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule, string startDocumentNo, List<int> check_ID,
                                       VAdvantage.Model.MPaySelectionCheck[] m_checks, VAdvantage.Model.MPaymentBatch m_batch)
        {
            int VAB_Payment_ID = 0;
            string sql = string.Empty;
            // int lastDocumentNo = MPaySelectionCheck.ConfirmPrint(m_checks, m_batch);
            int lastDocumentNo = VAdvantage.Model.MPaySelectionCheck.ConfirmPrint(m_checks, m_batch);
            if (lastDocumentNo != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE VAB_Bank_AcctDoc SET CurrentNext= " + (++lastDocumentNo))
                    .Append(" WHERE VAB_Bank_Acct_ID= " + m_VAB_Bank_Acct_ID)
                    .Append(" AND PaymentRule=  '" + PaymentRule + "'");

                int result = DB.ExecuteQuery(sb.ToString());
            }

            payment_ID = new List<int>();
            for (int k = 0; k < check_ID.Count; k++)
            {
                sql = "select VAB_Payment_id from VAB_PaymentOptionCheck where VAB_PaymentOptionCheck_id = " + Util.GetValueOfInt(check_ID[k]);
                VAB_Payment_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                payment_ID.Add(VAB_Payment_ID);
            }


            return payment_ID;


        }
        /// <summary>
        /// VPayPrintRemittance
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="payment_ID"></param>
        /// <returns></returns>
        public string VPayPrintRemittance(Ctx ctx, List<int> payment_ID)
        {
            int table_ID = 0;
            int VAF_Job_ID = 0;
            string sql = "";
            int paymentTable_ID = 0;
            int paymentVAF_Job_ID = 0;
            VAdvantage.ProcessEngine.ProcessCtl pctrl = null;
            try
            {
                sql = "select vaf_tableview_id from vaf_tableview where tablename = 'VAB_Payment'";
                paymentTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                sql = "select VAF_Job_id from VAF_Job where value = 'PaymentPrintFormat'";
                paymentVAF_Job_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

            }
            catch
            {

            }
            //for (int l = 0; l < payment_ID.Count; l++)
            //{
            //    byte[] reportData = null;
            //    string result = "";
            //    VAdvantage.ProcessEngine.ProcessInfo pin = new VAdvantage.ProcessEngine.ProcessInfo(null, paymentVAF_Job_ID, paymentTable_ID, Util.GetValueOfInt(payment_ID[l]));
            //    pin.SetVAF_UserContact_ID(ctx.GetVAF_UserContact_ID());
            //    pin.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
            //    pctrl = new VAdvantage.ProcessEngine.ProcessCtl(ctx,null, pin, null);
            //    pctrl.Process(pin, ctx,out reportData, out result);              
            //}
            return "";
        }


        public Cmd_Export Cmd_Export(Ctx ctx, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule, int startDocumentNo)
        {
            Cmd_Export objCmdExport = new Cmd_Export();
            bool res = false;

            log.Info(PaymentRule);
            log.Config("VAB_PaymentOption_ID=" + VAB_PaymentOption_ID + ", PaymentRule=" + PaymentRule + ", DocumentNo=" + startDocumentNo);

            string expFile = "Export.txt";


            m_checks = VAdvantage.Model.MPaySelectionCheck.Get(VAB_PaymentOption_ID, PaymentRule, startDocumentNo, null);

            //
            if (m_checks == null || m_checks.Length == 0)
            {
                // ShowMessage.Error("VPayPrintNoRecords", null, "(" + Msg.Translate(Envs.GetCtx(), "VAB_PaymentOptionLine_ID") + " #0");
                res = false;
                //return false;
            }
            else
            {
                res = true;
            }
            m_batch = VAdvantage.Model.MPaymentBatch.GetForPaySelection(ctx, VAB_PaymentOption_ID, null);
            //return true;
            if (!res)
            {
                return null;
            }
            //  Get File Info
            Dictionary<VAdvantage.Model.MPaySelectionCheck, String> oldCheckValues = new Dictionary<VAdvantage.Model.MPaySelectionCheck, string>();

            //  Create File
            List<int> check_ID = new List<int>();
            foreach (VAdvantage.Model.MPaySelectionCheck check in m_checks)
            {
                //oldCheckValues.put(check,(String)check.get_ValueOld("DocumentNo"));
                oldCheckValues.Add(check, Util.GetValueOfString(check.Get_ValueOld("DocumentNo")));
                check_ID.Add(Util.GetValueOfInt(check.Get_ID()));
            }

            List<String[]> busPart = new List<String[]>();
            List<String[]> bankAcc = new List<String[]>();
            List<String> comm = new List<String>();
            List<String> DocumentNo = new List<String>();
            List<String> PayDate = new List<String>();
            List<String> ISOCode = new List<String>();
            List<String> PayAmt = new List<String>();

            ////  write lines
            for (int i = 0; i < m_checks.Length; i++)
            {
                VAdvantage.Model.MPaySelectionCheck mpp = m_checks[i];
                if (mpp == null)
                    continue;
                //  BPartner Info
                bp = GetBPartnerInfo(mpp.GetVAB_BusinessPartner_ID());
                busPart.Add(bp);
                //  TarGet BankAccount Info
                bpba = GetBPBankAccountInfo(mpp.GetVAB_BPart_Bank_Acct_ID());
                bankAcc.Add(bpba);
                //  Comment - list of invoice document no
                StringBuilder comment = new StringBuilder();
                VAdvantage.Model.MPaySelectionLine[] psls = mpp.GetPaySelectionLines(false);
                for (int l = 0; l < psls.Length; l++)
                {
                    if (l > 0)
                        comment.Append(", ");
                    comment.Append(psls[l].GetInvoice().GetDocumentNo());
                }
                comm.Add(comment.ToString());
                DocumentNo.Add(mpp.GetDocumentNo());
                PayDate.Add(mpp.GetParent().GetPayDate().ToString());
                ISOCode.Add(VAdvantage.Model.MCurrency.GetISO_Code(ctx, mpp.GetParent().GetVAB_Currency_ID()));
                PayAmt.Add(mpp.GetPayAmt().ToString());
            }


            string filepath = Export(m_checks, expFile, comm, busPart, bankAcc, DocumentNo, PayDate, ISOCode, PayAmt);

            objCmdExport.check_ID = check_ID;
            objCmdExport.m_batch = m_batch.GetVAB_PaymentBatch_ID();
            objCmdExport.filepath = filepath;
            return objCmdExport;
        }

        private string Export(VAdvantage.Model.MPaySelectionCheck[] m_checks, string expFile, List<String> comment, List<String[]> Partners, List<String[]> account, List<String> docNo, List<String> payDate, List<String> ISOCode, List<String> payAmt)
        {
            //StreamWriter sw = null;
            int noLines = 0;
            //using (sw = new StreamWriter(filepath))
            //{
            // sw.Write("abc");
            char x = '"';      //  ease
            StringBuilder fileContent = new StringBuilder();
            StringBuilder line = null;
            //  write header
            line = new StringBuilder();
            line.Append(x).Append("Value").Append(x).Append(",")
                .Append(x).Append("Name").Append(x).Append(",")
                .Append(x).Append("Contact").Append(x).Append(",")
                .Append(x).Append("Addr1").Append(x).Append(",")
                .Append(x).Append("Addr2").Append(x).Append(",")
                .Append(x).Append("City").Append(x).Append(",")
                .Append(x).Append("State").Append(x).Append(",")
                .Append(x).Append("ZIP").Append(x).Append(",")
                .Append(x).Append("Country").Append(x).Append(",")
                .Append(x).Append("ReferenceNo").Append(x).Append(",")
                .Append(x).Append("BPRoutingNo").Append(x).Append(",")
                .Append(x).Append("BPAccountNo").Append(x).Append(",")
                .Append(x).Append("BPAName").Append(x).Append(",")
                .Append(x).Append("BPACity").Append(x).Append(",")
                .Append(x).Append("BPBBAN").Append(x).Append(",")
                .Append(x).Append("BPIBAN").Append(x).Append(",")
                .Append(x).Append("BAName").Append(x).Append(",")
                .Append(x).Append("BARoutingNo").Append(x).Append(",")
                .Append(x).Append("BASwiftCode").Append(x).Append(",")
                .Append(x).Append("DocumentNo").Append(x).Append(",")
                .Append(x).Append("PayDate").Append(x).Append(",")
                .Append(x).Append("Currency").Append(x).Append(",")
                .Append(x).Append("PayAmount").Append(x).Append(",")
                .Append(x).Append("Comment").Append(x)
                .Append(Env.NL);
            //sw.Write(line.ToString());
            noLines++;
            fileContent.Append(line);
            for (int i = 0; i < Partners.Count; i++)
            {
                line = new StringBuilder();
                line.Append(x).Append(bp[BP_VALUE]).Append(x).Append(",")   // Value
                    .Append(x).Append(Partners[i][BP_NAME]).Append(x).Append(",")    // Name
                    .Append(x).Append(Partners[i][BP_CONTACT]).Append(x).Append(",") // Contact
                    .Append(x).Append(Partners[i][BP_ADDR1]).Append(x).Append(",")   // Addr1
                    .Append(x).Append(Partners[i][BP_ADDR2]).Append(x).Append(",")   // Addr2
                    .Append(x).Append(Partners[i][BP_CITY]).Append(x).Append(",")    // City
                    .Append(x).Append(Partners[i][BP_REGION]).Append(x).Append(",")  // State
                    .Append(x).Append(Partners[i][BP_POSTAL]).Append(x).Append(",")  // ZIP
                    .Append(x).Append(Partners[i][BP_COUNTRY]).Append(x).Append(",") // Country
                    .Append(x).Append(Partners[i][BP_REFNO]).Append(x).Append(",")   // ReferenceNo
                    .Append(x).Append(account[i][BPBA_RoutingNo]).Append(x).Append(",")   // Routing No (as of BPBankAccount
                    .Append(x).Append(account[i][BPBA_AccountNo]).Append(x).Append(",")   // AccountNo
                    .Append(x).Append(account[i][BPBA_AName]).Append(x).Append(",")       // Account Name
                    .Append(x).Append(account[i][BPBA_ACity]).Append(x).Append(",")       // Account City
                    .Append(x).Append(account[i][BPBA_BBAN]).Append(x).Append(",")        // BBAN
                    .Append(x).Append(account[i][BPBA_IBAN]).Append(x).Append(",")        // IBAN
                    .Append(x).Append(account[i][BA_Name]).Append(x).Append(",")          // Bank Name
                    .Append(x).Append(account[i][BA_RoutingNo]).Append(x).Append(",")     // Bank RoutingNo
                    .Append(x).Append(account[i][BA_SwitftCode]).Append(x).Append(",")    // SwiftCode
                    //  Payment Info
                    .Append(x).Append(docNo[i]).Append(x).Append(",")    // DocumentNo
                    .Append(payDate[i]).Append(",")               // PayDate
                    .Append(x).Append(ISOCode[i]).Append(x).Append(",")    // Currency
                    .Append(payAmt[i]).Append(",")                // PayAmount
                    .Append(x).Append(comment[i]).Append(x)     // Comment
                    .Append(Env.NL);
                //sw.Write(line.ToString());
                noLines++;
            }
            fileContent.Append(line);
            string filename = "export" + DateTime.Now.Ticks + ".txt";
            string filepath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\" + filename;
            FileStream fs1 = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            writer.Write(fileContent.ToString());
            writer.Close();
            return filename;
        }
        public string VPayPrintSuccess(Ctx ctx, VAdvantage.Model.MPaySelectionCheck[] m_checks, VAdvantage.Model.MPaymentBatch m_batch)
        {
            if (true)
            {
                VAdvantage.Model.MPaySelectionCheck.ConfirmPrint(m_checks, m_batch);

            }
            else
            {
                //foreach (VAdvantage.Model.MPaySelectionCheck check in m_checks)
                //{
                //    check.SetCheckNo(oldCheckValues[check]);
                //    check.Save();
                //}
            }
            return "";
        }
        /// <summary>
        /// Get Checks
        /// </summary>
        /// <param name="PaymentRule">Payment Rule</param>
        /// <returns> true if payments were created</returns>
        private bool GetChecks(Ctx ctx, String PaymentRule, int VAB_PaymentOption_ID, int startDocumentNo)
        {

            log.Config("VAB_PaymentOption_ID=" + VAB_PaymentOption_ID + ", PaymentRule=" + PaymentRule + ", DocumentNo=" + startDocumentNo);
            //
            //this.setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));

            //	get Selections

            bool result = false;
            m_checks = VAdvantage.Model.MPaySelectionCheck.Get(VAB_PaymentOption_ID, PaymentRule, startDocumentNo, null);

            //this.setCursor(Cursor.getDefaultCursor());
            //
            if (m_checks == null || m_checks.Length == 0)
            {
                // ShowMessage.Error("VPayPrintNoRecords", null, "(" + Msg.Translate(ctx, "VAB_PaymentOptionLine_ID") + " #0");
                result = false;
                //return false;
            }
            else
            {
                result = true;
            }
            m_batch = VAdvantage.Model.MPaymentBatch.GetForPaySelection(ctx, VAB_PaymentOption_ID, null);
            //return true;


            return true;
        }
        /**
     *  Get Customer/Vendor Info.
     *  Based on BP_ static variables
     *  @param VAB_BusinessPartner_ID BPartner
     *  @return info array
     */
        private static String[] GetBPartnerInfo(int VAB_BusinessPartner_ID)
        {
            String[] bp = new String[10];

            String sql = "SELECT bp.Value, bp.Name, c.Name AS Contact, "
                + "a.Address1, a.Address2, a.City, r.Name AS Region, a.Postal, "
                + "cc.Name AS Country, bp.ReferenceNo "
                /*//jz use SQL standard outer join
                + "FROM VAB_BusinessPartner bp, VAF_UserContact c, VAB_BPart_Location l, VAB_Address a, VAB_RegionState r, VAB_Country cc "
                + "WHERE bp.VAB_BusinessPartner_ID=?"        // #1
                + " AND bp.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID(+)"
                + " AND bp.VAB_BusinessPartner_ID=l.VAB_BusinessPartner_ID"
                + " AND l.VAB_Address_ID=a.VAB_Address_ID"
                + " AND a.VAB_RegionState_ID=r.VAB_RegionState_ID(+)"
                + " AND a.VAB_Country_ID=cc.VAB_Country_ID "
                */
                + "FROM VAB_BusinessPartner bp "
                + "LEFT OUTER JOIN VAF_UserContact c ON (bp.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID) "
                + "INNER JOIN VAB_BPart_Location l ON (bp.VAB_BusinessPartner_ID=l.VAB_BusinessPartner_ID) "
                + "INNER JOIN VAB_Address a ON (l.VAB_Address_ID=a.VAB_Address_ID) "
                + "LEFT OUTER JOIN VAB_RegionState r ON (a.VAB_RegionState_ID=r.VAB_RegionState_ID) "
                + "INNER JOIN VAB_Country cc ON (a.VAB_Country_ID=cc.VAB_Country_ID) "
                + "WHERE bp.VAB_BusinessPartner_ID= " + VAB_BusinessPartner_ID          // #1
                + " ORDER BY l.IsBillTo DESC";

            IDataReader idr = null;
            try
            {
                //
                idr = DB.ExecuteReader(sql, null, null);

                if (idr.Read())
                {
                    bp[BP_VALUE] = idr.GetString(0);
                    if (bp[BP_VALUE] == null)
                        bp[BP_VALUE] = "";
                    bp[BP_NAME] = idr.GetString(1);
                    if (bp[BP_NAME] == null)
                        bp[BP_NAME] = "";
                    bp[BP_CONTACT] = idr.GetString(2);
                    if (bp[BP_CONTACT] == null)
                        bp[BP_CONTACT] = "";
                    bp[BP_ADDR1] = idr.GetString(3);
                    if (bp[BP_ADDR1] == null)
                        bp[BP_ADDR1] = "";
                    bp[BP_ADDR2] = idr.GetString(4);
                    if (bp[BP_ADDR2] == null)
                        bp[BP_ADDR2] = "";
                    bp[BP_CITY] = idr.GetString(5);
                    if (bp[BP_CITY] == null)
                        bp[BP_CITY] = "";
                    bp[BP_REGION] = idr.GetString(6);
                    if (bp[BP_REGION] == null)
                        bp[BP_REGION] = "";
                    bp[BP_POSTAL] = idr.GetString(7);
                    if (bp[BP_POSTAL] == null)
                        bp[BP_POSTAL] = "";
                    bp[BP_COUNTRY] = idr.GetString(8);
                    if (bp[BP_COUNTRY] == null)
                        bp[BP_COUNTRY] = "";
                    bp[BP_REFNO] = idr.GetString(9);
                    if (bp[BP_REFNO] == null)
                        bp[BP_REFNO] = "";
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            return bp;
        }

        /**
         *  Get Bank Account Info for tarGet Accpimt.
         *  Based on BP_ static variables
         *  @param VAB_BusinessPartner_ID BPartner
         *  @return info array
         */
        private static String[] GetBPBankAccountInfo(int VAB_BPart_Bank_Acct_ID)
        {
            String[] bp = new String[10];

            String sql = "SELECT bpba.RoutingNo, bpba.AccountNo, bpba.A_Name, bpba.A_City, bpba.BBAN, "
                + "bpba.IBAN, ba.Name, ba.RoutingNo, ba.SwiftCode "
                /*//jz use SQL standard outer join
                + "FROM VAB_BusinessPartner bp, VAF_UserContact c, VAB_BPart_Location l, VAB_Address a, VAB_RegionState r, VAB_Country cc "
                + "WHERE bp.VAB_BusinessPartner_ID=?"        // #1
                + " AND bp.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID(+)"
                + " AND bp.VAB_BusinessPartner_ID=l.VAB_BusinessPartner_ID"
                + " AND l.VAB_Address_ID=a.VAB_Address_ID"
                + " AND a.VAB_RegionState_ID=r.VAB_RegionState_ID(+)"
                + " AND a.VAB_Country_ID=cc.VAB_Country_ID "
                */
                + "FROM VAB_BPart_Bank_Acct bpba "
                + "LEFT OUTER JOIN C_Bank ba ON (bpba.C_Bank_ID = ba.C_Bank_ID) "
                + "WHERE bpba.VAB_BPart_Bank_Acct_ID=" + VAB_BPart_Bank_Acct_ID;        // #1

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    bp[BPBA_RoutingNo] = idr.GetString(1);
                    if (bp[BPBA_RoutingNo] == null)
                        bp[BPBA_RoutingNo] = "";
                    bp[BPBA_AccountNo] = idr.GetString(2);
                    if (bp[BPBA_AccountNo] == null)
                        bp[BPBA_AccountNo] = "";
                    bp[BPBA_AName] = idr.GetString(3);
                    if (bp[BPBA_AName] == null)
                        bp[BPBA_AName] = "";
                    bp[BPBA_ACity] = idr.GetString(4);
                    if (bp[BPBA_ACity] == null)
                        bp[BPBA_ACity] = "";
                    bp[BPBA_BBAN] = idr.GetString(5);
                    if (bp[BPBA_BBAN] == null)
                        bp[BPBA_BBAN] = "";
                    bp[BPBA_IBAN] = idr.GetString(6);
                    if (bp[BPBA_IBAN] == null)
                        bp[BPBA_IBAN] = "";
                    bp[BA_Name] = idr.GetString(7);
                    if (bp[BA_Name] == null)
                        bp[BA_Name] = "";
                    bp[BA_RoutingNo] = idr.GetString(8);
                    if (bp[BA_RoutingNo] == null)
                        bp[BA_RoutingNo] = "";
                    bp[BA_SwitftCode] = idr.GetString(9);
                    if (bp[BA_SwitftCode] == null)
                        bp[BA_SwitftCode] = "";
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            return bp;
        }

    }
    //*******************
    //Properties Classes for VPayPrint
    //*******************
    public class PaymentPrintDetail
    {
        public List<PaymentSelection> PaymentSelection { get; set; }
        public List<PaymentMethod> PaymentMethod { get; set; }
        public List<PSelectInfo> PSelectInfo { get; set; }
    }
    public class PSelectInfo
    {
        public int BankAccount_ID { get; set; }
        public string BankAccount { get; set; }
        public string CurrentBalance { get; set; }
        public string Currency { get; set; }
        public string CheckNo { get; set; }
        public string NoOfPayments { get; set; }
    }
    public class PInfo
    {
        public string CheckNo { get; set; }
        public string NoOfPayments { get; set; }
        public string Msg { get; set; }
    }
    public class Cmd_Print
    {
        public List<int> m_checks { get; set; }
        public int m_batch { get; set; }
        public List<int> check_ID { get; set; }
    }
    public class Cmd_Export
    {
        public List<int> m_checks { get; set; }
        public int m_batch { get; set; }
        public List<int> check_ID { get; set; }
        public string filepath { get; set; }
    }
}