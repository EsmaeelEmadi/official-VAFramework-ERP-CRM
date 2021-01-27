namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    //using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for VAB_Contract
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_Contract : PO
    {
        public X_VAB_Contract(Context ctx, int VAB_Contract_ID, Trx trxName)
            : base(ctx, VAB_Contract_ID, trxName)
        {
            /** if (VAB_Contract_ID == 0)
            {
            SetVAB_BusinessPartner_ID (0);
            SetVAB_Contract_ID (0);
            SetVAB_Currency_ID (0);	// @VAB_Currency_ID@
            SetVAB_Frequency_ID (0);
            SetVAB_PaymentTerm_ID (0);
            SetVAB_TaxRate_ID (0);
            SetVAB_UOM_ID (0);	// @#VAB_UOM_ID@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetEndDate (DateTime.Now);
            SetGrandTotal (0.0);
            SetLineNetAmt (0.0);
            SetM_PriceList_ID (0);
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceList (0.0);
            SetQtyEntered (0.0);	// 1
            SetStartDate (DateTime.Now);
            SetTaxAmt (0.0);
            }
             */
        }
        public X_VAB_Contract(Ctx ctx, int VAB_Contract_ID, Trx trxName)
            : base(ctx, VAB_Contract_ID, trxName)
        {
            /** if (VAB_Contract_ID == 0)
            {
            SetVAB_BusinessPartner_ID (0);
            SetVAB_Contract_ID (0);
            SetVAB_Currency_ID (0);	// @VAB_Currency_ID@
            SetVAB_Frequency_ID (0);
            SetVAB_PaymentTerm_ID (0);
            SetVAB_TaxRate_ID (0);
            SetVAB_UOM_ID (0);	// @#VAB_UOM_ID@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetEndDate (DateTime.Now);
            SetGrandTotal (0.0);
            SetLineNetAmt (0.0);
            SetM_PriceList_ID (0);
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceList (0.0);
            SetQtyEntered (0.0);	// 1
            SetStartDate (DateTime.Now);
            SetTaxAmt (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Contract(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Contract(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Contract(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_Contract()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27610984547573L;
        /** Last Updated Timestamp 2/10/2012 1:03:51 PM */
        public static long updatedMS = 1328859230784L;
        /** VAF_TableView_ID=1000256 */
        public static int Table_ID;
        // =1000256;

        /** TableName=VAB_Contract */
        public static String Table_Name = "VAB_Contract";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAB_Contract[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** Bill_BPartner_ID VAF_Control_Ref_ID=138 */
        public static int BILL_BPARTNER_ID_VAF_Control_Ref_ID = 138;
        /** Set Bill To.
        @param Bill_BPartner_ID Business Partner to be invoiced */
        public void SetBill_BPartner_ID(int Bill_BPartner_ID)
        {
            if (Bill_BPartner_ID <= 0) Set_Value("Bill_BPartner_ID", null);
            else
                Set_Value("Bill_BPartner_ID", Bill_BPartner_ID);
        }
        /** Get Bill To.
        @return Business Partner to be invoiced */
        public int GetBill_BPartner_ID()
        {
            Object ii = Get_Value("Bill_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Bill_Location_ID VAF_Control_Ref_ID=159 */
        public static int BILL_LOCATION_ID_VAF_Control_Ref_ID = 159;
        /** Set Bill To Location.
        @param Bill_Location_ID Business Partner Location for invoicing */
        public void SetBill_Location_ID(int Bill_Location_ID)
        {
            if (Bill_Location_ID <= 0) Set_Value("Bill_Location_ID", null);
            else
                Set_Value("Bill_Location_ID", Bill_Location_ID);
        }
        /** Get Bill To Location.
        @return Business Partner Location for invoicing */
        public int GetBill_Location_ID()
        {
            Object ii = Get_Value("Bill_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Bill_User_ID VAF_Control_Ref_ID=110 */
        public static int BILL_USER_ID_VAF_Control_Ref_ID = 110;
        /** Set Invoice Contact.
        @param Bill_User_ID Business Partner Contact for invoicing */
        public void SetBill_User_ID(int Bill_User_ID)
        {
            if (Bill_User_ID <= 0) Set_Value("Bill_User_ID", null);
            else
                Set_Value("Bill_User_ID", Bill_User_ID);
        }
        /** Get Invoice Contact.
        @return Business Partner Contact for invoicing */
        public int GetBill_User_ID()
        {
            Object ii = Get_Value("Bill_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param VAB_BusinessPartner_ID Identifies a Business Partner */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID < 1) throw new ArgumentException("VAB_BusinessPartner_ID is mandatory.");
            Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetVAB_BusinessPartner_ID()
        {
            Object ii = Get_Value("VAB_BusinessPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param VAB_Promotion_ID Marketing Campaign */
        public void SetVAB_Promotion_ID(int VAB_Promotion_ID)
        {
            if (VAB_Promotion_ID <= 0) Set_Value("VAB_Promotion_ID", null);
            else
                Set_Value("VAB_Promotion_ID", VAB_Promotion_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetVAB_Promotion_ID()
        {
            Object ii = Get_Value("VAB_Promotion_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VAB_Contract_ID.
        @param VAB_Contract_ID VAB_Contract_ID */
        public void SetVAB_Contract_ID(int VAB_Contract_ID)
        {
            if (VAB_Contract_ID < 1) throw new ArgumentException("VAB_Contract_ID is mandatory.");
            Set_ValueNoCheck("VAB_Contract_ID", VAB_Contract_ID);
        }
        /** Get VAB_Contract_ID.
        @return VAB_Contract_ID */
        public int GetVAB_Contract_ID()
        {
            Object ii = Get_Value("VAB_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Type.
        @param VAB_CurrencyType_ID Currency Conversion Rate Type */
        public void SetVAB_CurrencyType_ID(int VAB_CurrencyType_ID)
        {
            if (VAB_CurrencyType_ID <= 0) Set_Value("VAB_CurrencyType_ID", null);
            else
                Set_Value("VAB_CurrencyType_ID", VAB_CurrencyType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetVAB_CurrencyType_ID()
        {
            Object ii = Get_Value("VAB_CurrencyType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID < 1) throw new ArgumentException("VAB_Currency_ID is mandatory.");
            Set_ValueNoCheck("VAB_Currency_ID", VAB_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVAB_Currency_ID()
        {
            Object ii = Get_Value("VAB_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Billing Frequency.
        @param VAB_Frequency_ID Billing Frequency */
        public void SetVAB_Frequency_ID(int VAB_Frequency_ID)
        {
            if (VAB_Frequency_ID < 1) throw new ArgumentException("VAB_Frequency_ID is mandatory.");
            Set_Value("VAB_Frequency_ID", VAB_Frequency_ID);
        }
        /** Get Billing Frequency.
        @return Billing Frequency */
        public int GetVAB_Frequency_ID()
        {
            Object ii = Get_Value("VAB_Frequency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Line.
        @param VAB_OrderLine_ID Order Line */
        public void SetVAB_OrderLine_ID(int VAB_OrderLine_ID)
        {
            if (VAB_OrderLine_ID <= 0) Set_Value("VAB_OrderLine_ID", null);
            else
                Set_Value("VAB_OrderLine_ID", VAB_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetVAB_OrderLine_ID()
        {
            Object ii = Get_Value("VAB_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param VAB_Order_ID Order */
        public void SetVAB_Order_ID(int VAB_Order_ID)
        {
            if (VAB_Order_ID <= 0) Set_Value("VAB_Order_ID", null);
            else
                Set_Value("VAB_Order_ID", VAB_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetVAB_Order_ID()
        {
            Object ii = Get_Value("VAB_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Term.
        @param VAB_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetVAB_PaymentTerm_ID(int VAB_PaymentTerm_ID)
        {
            if (VAB_PaymentTerm_ID < 1) throw new ArgumentException("VAB_PaymentTerm_ID is mandatory.");
            Set_Value("VAB_PaymentTerm_ID", VAB_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetVAB_PaymentTerm_ID()
        {
            Object ii = Get_Value("VAB_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Opporutnity.
        @param VAB_Project_ID Financial Project */
        public void SetVAB_Project_ID(int VAB_Project_ID)
        {
            if (VAB_Project_ID <= 0) Set_Value("VAB_Project_ID", null);
            else
                Set_Value("VAB_Project_ID", VAB_Project_ID);
        }
        /** Get Opporutnity.
        @return Financial Project */
        public int GetVAB_Project_ID()
        {
            Object ii = Get_Value("VAB_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set C_Renewal_ID.
        @param C_Renewal_ID C_Renewal_ID */
        public void SetC_Renewal_ID(int C_Renewal_ID)
        {
            if (C_Renewal_ID <= 0) Set_Value("C_Renewal_ID", null);
            else
                Set_Value("C_Renewal_ID", C_Renewal_ID);
        }
        /** Get C_Renewal_ID.
        @return C_Renewal_ID */
        public int GetC_Renewal_ID()
        {
            Object ii = Get_Value("C_Renewal_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax.
        @param VAB_TaxRate_ID Tax identifier */
        public void SetVAB_TaxRate_ID(int VAB_TaxRate_ID)
        {
            if (VAB_TaxRate_ID < 1) throw new ArgumentException("VAB_TaxRate_ID is mandatory.");
            Set_Value("VAB_TaxRate_ID", VAB_TaxRate_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetVAB_TaxRate_ID()
        {
            Object ii = Get_Value("VAB_TaxRate_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param VAB_UOM_ID Unit of Measure */
        public void SetVAB_UOM_ID(int VAB_UOM_ID)
        {
            if (VAB_UOM_ID < 1) throw new ArgumentException("VAB_UOM_ID is mandatory.");
            Set_ValueNoCheck("VAB_UOM_ID", VAB_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetVAB_UOM_ID()
        {
            Object ii = Get_Value("VAB_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cancel Before Days.
        @param CancelBeforeDays Cancel Before Days */
        public void SetCancelBeforeDays(int CancelBeforeDays)
        {
            Set_Value("CancelBeforeDays", CancelBeforeDays);
        }
        /** Get Cancel Before Days.
        @return Cancel Before Days */
        public int GetCancelBeforeDays()
        {
            Object ii = Get_Value("CancelBeforeDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cancellation Date.
        @param CancellationDate Cancellation Date */
        public void SetCancellationDate(DateTime? CancellationDate)
        {
            Set_Value("CancellationDate", (DateTime?)CancellationDate);
        }
        /** Get Cancellation Date.
        @return Cancellation Date */
        public DateTime? GetCancellationDate()
        {
            return (DateTime?)Get_Value("CancellationDate");
        }
        /** Set Cancellation Remarks.
        @param CancellationRemarks Cancellation Remarks */
        public void SetCancellationRemarks(String CancellationRemarks)
        {
            if (CancellationRemarks != null && CancellationRemarks.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CancellationRemarks = CancellationRemarks.Substring(0, 50);
            }
            Set_Value("CancellationRemarks", CancellationRemarks);
        }
        /** Get Cancellation Remarks.
        @return Cancellation Remarks */
        public String GetCancellationRemarks()
        {
            return (String)Get_Value("CancellationRemarks");
        }
        /** Set No of Cycles.
        @param Cycles No of Cycles */
        public void SetCycles(int Cycles)
        {
            Set_Value("Cycles", Cycles);
        }
        /** Get No of Cycles.
        @return No of Cycles */
        public int GetCycles()
        {
            Object ii = Get_Value("Cycles");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Discount %.
        @param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount)
        {
            Set_Value("Discount", (Decimal?)Discount);
        }
        /** Get Discount %.
        @return Discount in percent */
        public Decimal GetDiscount()
        {
            Object bd = Get_Value("Discount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** DocAction VAF_Control_Ref_ID=135 */
        public static int DOCACTION_VAF_Control_Ref_ID = 135;
        /** <None> = -- */
        public static String DOCACTION_None = "--";
        /** Approve = AP */
        public static String DOCACTION_Approve = "AP";
        /** Close = CL */
        public static String DOCACTION_Close = "CL";
        /** Complete = CO */
        public static String DOCACTION_Complete = "CO";
        /** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";
        /** Post = PO */
        public static String DOCACTION_Post = "PO";
        /** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";
        /** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";
        /** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";
        /** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";
        /** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";
        /** Void = VO */
        public static String DOCACTION_Void = "VO";
        /** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";
        /** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocActionValid(String test)
        {
            return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (DocAction == null) throw new ArgumentException("DocAction is mandatory");
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                DocAction = DocAction.Substring(0, 20);
            }
            Set_Value("DocAction", DocAction);
        }
        /** Get Document Action.
        @return The targeted status of the document */
        public String GetDocAction()
        {
            return (String)Get_Value("DocAction");
        }

        /** DocStatus VAF_Control_Ref_ID=131 */
        public static int DOCSTATUS_VAF_Control_Ref_ID = 131;
        /** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocStatusValid(String test)
        {
            return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory");
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo != null && DocumentNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                DocumentNo = DocumentNo.Substring(0, 50);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Set End Date.
        @param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate)
        {
            if (EndDate == null) throw new ArgumentException("EndDate is mandatory.");
            Set_Value("EndDate", (DateTime?)EndDate);
        }
        /** Get End Date.
        @return Last effective date (inclusive) */
        public DateTime? GetEndDate()
        {
            return (DateTime?)Get_Value("EndDate");
        }
        /** Set Generate Invoice.
        @param GenerateInvoice Generate Invoice */
        public void SetGenerateInvoice(String GenerateInvoice)
        {
            if (GenerateInvoice != null && GenerateInvoice.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                GenerateInvoice = GenerateInvoice.Substring(0, 20);
            }
            Set_Value("GenerateInvoice", GenerateInvoice);
        }
        /** Get Generate Invoice.
        @return Generate Invoice */
        public String GetGenerateInvoice()
        {
            return (String)Get_Value("GenerateInvoice");
        }
        /** Set Grand Total.
        @param GrandTotal Total amount of document */
        public void SetGrandTotal(Decimal? GrandTotal)
        {
            if (GrandTotal == null) throw new ArgumentException("GrandTotal is mandatory.");
            Set_ValueNoCheck("GrandTotal", (Decimal?)GrandTotal);
        }
        /** Get Grand Total.
        @return Total amount of document */
        public Decimal GetGrandTotal()
        {
            Object bd = Get_Value("GrandTotal");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Invoices Generated.
        @param InvoicesGenerated Invoices Generated */
        public void SetInvoicesGenerated(int InvoicesGenerated)
        {
            Set_Value("InvoicesGenerated", InvoicesGenerated);
        }
        /** Get Invoices Generated.
        @return Invoices Generated */
        public int GetInvoicesGenerated()
        {
            Object ii = Get_Value("InvoicesGenerated");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set IsCancel.
        @param IsCancel IsCancel */
        public void SetIsCancel(Boolean IsCancel)
        {
            Set_Value("IsCancel", IsCancel);
        }
        /** Get IsCancel.
        @return IsCancel */
        public Boolean IsCancel()
        {
            Object oo = Get_Value("IsCancel");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Return Transaction.
        @param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx)
        {
            Set_Value("IsReturnTrx", IsReturnTrx);
        }
        /** Get Return Transaction.
        @return This is a return transaction */
        public Boolean IsReturnTrx()
        {
            Object oo = Get_Value("IsReturnTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line Amount.
        @param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt)
        {
            if (LineNetAmt == null) throw new ArgumentException("LineNetAmt is mandatory.");
            Set_Value("LineNetAmt", (Decimal?)LineNetAmt);
        }
        /** Get Line Amount.
        @return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt()
        {
            Object bd = Get_Value("LineNetAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price List.
        @param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID < 1) throw new ArgumentException("M_PriceList_ID is mandatory.");
            Set_Value("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unit Price.
        @param PriceActual Actual Price  */
        public void SetPriceActual(Decimal? PriceActual)
        {
            if (PriceActual == null) throw new ArgumentException("PriceActual is mandatory.");
            Set_Value("PriceActual", (Decimal?)PriceActual);
        }
        /** Get Unit Price.
        @return Actual Price  */
        public Decimal GetPriceActual()
        {
            Object bd = Get_Value("PriceActual");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price.
        @param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered)
        {
            if (PriceEntered == null) throw new ArgumentException("PriceEntered is mandatory.");
            Set_Value("PriceEntered", (Decimal?)PriceEntered);
        }
        /** Get Price.
        @return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered()
        {
            Object bd = Get_Value("PriceEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set List Price.
        @param PriceList List Price */
        public void SetPriceList(Decimal? PriceList)
        {
            if (PriceList == null) throw new ArgumentException("PriceList is mandatory.");
            Set_Value("PriceList", (Decimal?)PriceList);
        }
        /** Get List Price.
        @return List Price */
        public Decimal GetPriceList()
        {
            Object bd = Get_Value("PriceList");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** PriceListID VAF_Control_Ref_ID=166 */
        public static int PRICELISTID_VAF_Control_Ref_ID = 166;
        /** Set Price List.
        @param PriceListID Price List */
        public void SetPriceListID(int PriceListID)
        {
            Set_Value("PriceListID", PriceListID);
        }
        /** Get Price List.
        @return Price List */
        public int GetPriceListID()
        {
            Object ii = Get_Value("PriceListID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Process Contract.
        @param ProcessContract Process Contract */
        public void SetProcessContract(String ProcessContract)
        {
            if (ProcessContract != null && ProcessContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ProcessContract = ProcessContract.Substring(0, 50);
            }
            Set_Value("ProcessContract", ProcessContract);
        }
        /** Get Process Contract.
        @return Process Contract */
        public String GetProcessContract()
        {
            return (String)Get_Value("ProcessContract");
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Quantity.
        @param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            if (QtyEntered == null) throw new ArgumentException("QtyEntered is mandatory.");
            Set_Value("QtyEntered", (Decimal?)QtyEntered);
        }
        /** Get Quantity.
        @return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered()
        {
            Object bd = Get_Value("QtyEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Contract Reference.
        @param RefContract Contract Reference */
        public void SetRefContract(String RefContract)
        {
            if (RefContract != null && RefContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                RefContract = RefContract.Substring(0, 50);
            }
            Set_Value("RefContract", RefContract);
        }
        /** Get Contract Reference.
        @return Contract Reference */
        public String GetRefContract()
        {
            return (String)Get_Value("RefContract");
        }

        /** Ref_Contract_ID VAF_Control_Ref_ID=1000096 */
        public static int REF_CONTRACT_ID_VAF_Control_Ref_ID = 1000096;
        /** Set Contract Ref..
        @param Ref_Contract_ID Contract Ref. */
        public void SetRef_Contract_ID(int Ref_Contract_ID)
        {
            if (Ref_Contract_ID <= 0) Set_Value("Ref_Contract_ID", null);
            else
                Set_Value("Ref_Contract_ID", Ref_Contract_ID);
        }
        /** Get Contract Ref..
        @return Contract Ref. */
        public int GetRef_Contract_ID()
        {
            Object ii = Get_Value("Ref_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Renew Contract.
        @param RenewContract Renew Contract */
        public void SetRenewContract(String RenewContract)
        {
            if (RenewContract != null && RenewContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                RenewContract = RenewContract.Substring(0, 50);
            }
            Set_Value("RenewContract", RenewContract);
        }
        /** Get Renew Contract.
        @return Renew Contract */
        public String GetRenewContract()
        {
            return (String)Get_Value("RenewContract");
        }

        /** RenewalType VAF_Control_Ref_ID=1000095 */
        public static int RENEWALTYPE_VAF_Control_Ref_ID = 1000095;
        /** Automatic = A */
        public static String RENEWALTYPE_Automatic = "A";
        /** Manual = M */
        public static String RENEWALTYPE_Manual = "M";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsRenewalTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("M");
        }
        /** Set Renewal Type.
        @param RenewalType Renewal Type */
        public void SetRenewalType(String RenewalType)
        {
            if (!IsRenewalTypeValid(RenewalType))
                throw new ArgumentException("RenewalType Invalid value - " + RenewalType + " - Reference_ID=1000095 - A - M");
            if (RenewalType != null && RenewalType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                RenewalType = RenewalType.Substring(0, 1);
            }
            Set_Value("RenewalType", RenewalType);
        }
        /** Get Renewal Type.
        @return Renewal Type */
        public String GetRenewalType()
        {
            return (String)Get_Value("RenewalType");
        }

        /** SalesRep_ID VAF_Control_Ref_ID=190 */
        public static int SALESREP_ID_VAF_Control_Ref_ID = 190;
        /** Set Owner.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Owner.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Schedule Contract.
        @param ScheduleContract Schedule Contract */
        public void SetScheduleContract(String ScheduleContract)
        {
            if (ScheduleContract != null && ScheduleContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ScheduleContract = ScheduleContract.Substring(0, 50);
            }
            Set_Value("ScheduleContract", ScheduleContract);
        }
        /** Get Schedule Contract.
        @return Schedule Contract */
        public String GetScheduleContract()
        {
            return (String)Get_Value("ScheduleContract");
        }
        /** Set Start Date.
        @param StartDate First effective day (inclusive) */
        public void SetStartDate(DateTime? StartDate)
        {
            if (StartDate == null) throw new ArgumentException("StartDate is mandatory.");
            Set_Value("StartDate", (DateTime?)StartDate);
        }
        /** Get Start Date.
        @return First effective day (inclusive) */
        public DateTime? GetStartDate()
        {
            return (DateTime?)Get_Value("StartDate");
        }
        /// <summary>
        /// Set Surcharge Amount.
        /// </summary>
        /// <param name="SurchargeAmt">Surcharge Amount for a document</param>        
        public void SetSurchargeAmt(Decimal? SurchargeAmt)
        {
            Set_Value("SurchargeAmt", (Decimal?)SurchargeAmt);
        }
        /// <summary>
        /// Get Surcharge Amount.
        /// </summary>
        /// <returns>Surcharge Amount for a document</returns>
        public Decimal GetSurchargeAmt()
        {
            Object bd = Get_Value("SurchargeAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            if (TaxAmt == null) throw new ArgumentException("TaxAmt is mandatory.");
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Total Invoice.
        @param TotalInvoice Total Invoice */
        public void SetTotalInvoice(int TotalInvoice)
        {
            Set_Value("TotalInvoice", TotalInvoice);
        }
        /** Get Total Invoice.
        @return Total Invoice */
        public int GetTotalInvoice()
        {
            Object ii = Get_Value("TotalInvoice");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Ref_PriceList_ID VAF_Control_Ref_ID=166 */
        public static int Ref_PriceList_ID_VAF_Control_Ref_ID = 166;
        /** Set Price List.
        @param Ref_PriceList_ID Price List */
        public void SetRef_PriceList_ID(int Ref_PriceList_ID)
        {
            Set_Value("Ref_PriceList_ID", Ref_PriceList_ID);
        }
        /** Get Price List.
        @return Price List */
        public int GetRef_PriceList_ID()
        {
            Object ii = Get_Value("Ref_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bill Start Date.
        @param BillStartDate BillStartDate */
        public void SetBillStartDate(DateTime? BillStartDate)
        {
            Set_Value("BillStartDate", (DateTime?)BillStartDate);
        }
        /** Get BillStartDate.
        @return BillStartDate */
        public DateTime? GetBillStartDate()
        {
            return (DateTime?)Get_Value("BillStartDate");
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
    }

}
