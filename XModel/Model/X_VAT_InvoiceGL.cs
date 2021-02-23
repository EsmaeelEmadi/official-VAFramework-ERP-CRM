namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for VAT_InvoiceGL
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_InvoiceGL : PO
{
public X_VAT_InvoiceGL (Context ctx, int VAT_InvoiceGL_ID, Trx trxName) : base (ctx, VAT_InvoiceGL_ID, trxName)
{
/** if (VAT_InvoiceGL_ID == 0)
{
SetVAF_JInstance_ID (0);
SetAmtAcctBalance (0.0);
SetAmtRevalCr (0.0);
SetAmtRevalCrDiff (0.0);
SetAmtRevalDr (0.0);
SetAmtRevalDrDiff (0.0);
SetAmtSourceBalance (0.0);
SetVAB_CurrencyTypeReval_ID (0);
SetVAB_Invoice_ID (0);
SetDateReval (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetGrandTotal (0.0);
SetIsAllCurrencies (false);
SetOpenAmt (0.0);
}
 */
}
public X_VAT_InvoiceGL (Ctx ctx, int VAT_InvoiceGL_ID, Trx trxName) : base (ctx, VAT_InvoiceGL_ID, trxName)
{
/** if (VAT_InvoiceGL_ID == 0)
{
SetVAF_JInstance_ID (0);
SetAmtAcctBalance (0.0);
SetAmtRevalCr (0.0);
SetAmtRevalCrDiff (0.0);
SetAmtRevalDr (0.0);
SetAmtRevalDrDiff (0.0);
SetAmtSourceBalance (0.0);
SetVAB_CurrencyTypeReval_ID (0);
SetVAB_Invoice_ID (0);
SetDateReval (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetGrandTotal (0.0);
SetIsAllCurrencies (false);
SetOpenAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_InvoiceGL (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_InvoiceGL (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_InvoiceGL (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_InvoiceGL()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384271L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067482L;
/** VAF_TableView_ID=803 */
public static int Table_ID;
 // =803;

/** TableName=VAT_InvoiceGL */
public static String Table_Name="VAT_InvoiceGL";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAT_InvoiceGL[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param VAF_JInstance_ID Instance of the process */
public void SetVAF_JInstance_ID (int VAF_JInstance_ID)
{
if (VAF_JInstance_ID < 1) throw new ArgumentException ("VAF_JInstance_ID is mandatory.");
Set_ValueNoCheck ("VAF_JInstance_ID", VAF_JInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetVAF_JInstance_ID() 
{
Object ii = Get_Value("VAF_JInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_JInstance_ID().ToString());
}

/** APAR VAF_Control_Ref_ID=332 */
public static int APAR_VAF_Control_Ref_ID=332;
/** Receivables & Payables = A */
public static String APAR_ReceivablesPayables = "A";
/** Payables only = P */
public static String APAR_PayablesOnly = "P";
/** Receivables only = R */
public static String APAR_ReceivablesOnly = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAPARValid (String test)
{
return test == null || test.Equals("A") || test.Equals("P") || test.Equals("R");
}
/** Set AP - AR.
@param APAR Include Receivables and/or Payables transactions */
public void SetAPAR (String APAR)
{
if (!IsAPARValid(APAR))
throw new ArgumentException ("APAR Invalid value - " + APAR + " - Reference_ID=332 - A - P - R");
if (APAR != null && APAR.Length > 1)
{
log.Warning("Length > 1 - truncated");
APAR = APAR.Substring(0,1);
}
Set_Value ("APAR", APAR);
}
/** Get AP - AR.
@return Include Receivables and/or Payables transactions */
public String GetAPAR() 
{
return (String)Get_Value("APAR");
}
/** Set Accounted Balance.
@param AmtAcctBalance Accounted Balance Amount */
public void SetAmtAcctBalance (Decimal? AmtAcctBalance)
{
if (AmtAcctBalance == null) throw new ArgumentException ("AmtAcctBalance is mandatory.");
Set_Value ("AmtAcctBalance", (Decimal?)AmtAcctBalance);
}
/** Get Accounted Balance.
@return Accounted Balance Amount */
public Decimal GetAmtAcctBalance() 
{
Object bd =Get_Value("AmtAcctBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Revaluated Amount Cr.
@param AmtRevalCr Revaluated Cr Amount */
public void SetAmtRevalCr (Decimal? AmtRevalCr)
{
if (AmtRevalCr == null) throw new ArgumentException ("AmtRevalCr is mandatory.");
Set_Value ("AmtRevalCr", (Decimal?)AmtRevalCr);
}
/** Get Revaluated Amount Cr.
@return Revaluated Cr Amount */
public Decimal GetAmtRevalCr() 
{
Object bd =Get_Value("AmtRevalCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Revaluated Difference Cr.
@param AmtRevalCrDiff Revaluated Cr Amount Difference */
public void SetAmtRevalCrDiff (Decimal? AmtRevalCrDiff)
{
if (AmtRevalCrDiff == null) throw new ArgumentException ("AmtRevalCrDiff is mandatory.");
Set_Value ("AmtRevalCrDiff", (Decimal?)AmtRevalCrDiff);
}
/** Get Revaluated Difference Cr.
@return Revaluated Cr Amount Difference */
public Decimal GetAmtRevalCrDiff() 
{
Object bd =Get_Value("AmtRevalCrDiff");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Revaluated Amount Dr.
@param AmtRevalDr Revaluated Dr Amount */
public void SetAmtRevalDr (Decimal? AmtRevalDr)
{
if (AmtRevalDr == null) throw new ArgumentException ("AmtRevalDr is mandatory.");
Set_Value ("AmtRevalDr", (Decimal?)AmtRevalDr);
}
/** Get Revaluated Amount Dr.
@return Revaluated Dr Amount */
public Decimal GetAmtRevalDr() 
{
Object bd =Get_Value("AmtRevalDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Revaluated Difference Dr.
@param AmtRevalDrDiff Revaluated Dr Amount Difference */
public void SetAmtRevalDrDiff (Decimal? AmtRevalDrDiff)
{
if (AmtRevalDrDiff == null) throw new ArgumentException ("AmtRevalDrDiff is mandatory.");
Set_Value ("AmtRevalDrDiff", (Decimal?)AmtRevalDrDiff);
}
/** Get Revaluated Difference Dr.
@return Revaluated Dr Amount Difference */
public Decimal GetAmtRevalDrDiff() 
{
Object bd =Get_Value("AmtRevalDrDiff");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Balance.
@param AmtSourceBalance Source Balance Amount */
public void SetAmtSourceBalance (Decimal? AmtSourceBalance)
{
if (AmtSourceBalance == null) throw new ArgumentException ("AmtSourceBalance is mandatory.");
Set_Value ("AmtSourceBalance", (Decimal?)AmtSourceBalance);
}
/** Get Source Balance.
@return Source Balance Amount */
public Decimal GetAmtSourceBalance() 
{
Object bd =Get_Value("AmtSourceBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** VAB_CurrencyTypeReval_ID VAF_Control_Ref_ID=352 */
public static int VAB_CurrencyTypeREVAL_ID_VAF_Control_Ref_ID=352;
/** Set Revaluation Conversion Type.
@param VAB_CurrencyTypeReval_ID Revaluation Currency Conversion Type */
public void SetVAB_CurrencyTypeReval_ID (int VAB_CurrencyTypeReval_ID)
{
if (VAB_CurrencyTypeReval_ID < 1) throw new ArgumentException ("VAB_CurrencyTypeReval_ID is mandatory.");
Set_Value ("VAB_CurrencyTypeReval_ID", VAB_CurrencyTypeReval_ID);
}
/** Get Revaluation Conversion Type.
@return Revaluation Currency Conversion Type */
public int GetVAB_CurrencyTypeReval_ID() 
{
Object ii = Get_Value("VAB_CurrencyTypeReval_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAB_DocTypesReval_ID VAF_Control_Ref_ID=170 */
public static int VAB_DocTypesREVAL_ID_VAF_Control_Ref_ID=170;
/** Set Revaluation Document Type.
@param VAB_DocTypesReval_ID Document Type for Revaluation Journal */
public void SetVAB_DocTypesReval_ID (int VAB_DocTypesReval_ID)
{
if (VAB_DocTypesReval_ID <= 0) Set_Value ("VAB_DocTypesReval_ID", null);
else
Set_Value ("VAB_DocTypesReval_ID", VAB_DocTypesReval_ID);
}
/** Get Revaluation Document Type.
@return Document Type for Revaluation Journal */
public int GetVAB_DocTypesReval_ID() 
{
Object ii = Get_Value("VAB_DocTypesReval_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
public void SetVAB_Invoice_ID (int VAB_Invoice_ID)
{
if (VAB_Invoice_ID < 1) throw new ArgumentException ("VAB_Invoice_ID is mandatory.");
Set_ValueNoCheck ("VAB_Invoice_ID", VAB_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetVAB_Invoice_ID() 
{
Object ii = Get_Value("VAB_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Revaluation Date.
@param DateReval Date of Revaluation */
public void SetDateReval (DateTime? DateReval)
{
if (DateReval == null) throw new ArgumentException ("DateReval is mandatory.");
Set_Value ("DateReval", (DateTime?)DateReval);
}
/** Get Revaluation Date.
@return Date of Revaluation */
public DateTime? GetDateReval() 
{
return (DateTime?)Get_Value("DateReval");
}
/** Set Accounting Fact.
@param Actual_Acct_Detail_ID Accounting Fact */
public void SetActual_Acct_Detail_ID (int Actual_Acct_Detail_ID)
{
if (Actual_Acct_Detail_ID < 1) throw new ArgumentException ("Actual_Acct_Detail_ID is mandatory.");
Set_ValueNoCheck ("Actual_Acct_Detail_ID", Actual_Acct_Detail_ID);
}
/** Get Accounting Fact.
@return Accounting Fact */
public int GetActual_Acct_Detail_ID() 
{
Object ii = Get_Value("Actual_Acct_Detail_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Grand Total.
@param GrandTotal Total amount of document */
public void SetGrandTotal (Decimal? GrandTotal)
{
if (GrandTotal == null) throw new ArgumentException ("GrandTotal is mandatory.");
Set_Value ("GrandTotal", (Decimal?)GrandTotal);
}
/** Get Grand Total.
@return Total amount of document */
public Decimal GetGrandTotal() 
{
Object bd =Get_Value("GrandTotal");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Include All Currencies.
@param IsAllCurrencies Report not just foreign currency Invoices */
public void SetIsAllCurrencies (Boolean IsAllCurrencies)
{
Set_Value ("IsAllCurrencies", IsAllCurrencies);
}
/** Get Include All Currencies.
@return Report not just foreign currency Invoices */
public Boolean IsAllCurrencies() 
{
Object oo = Get_Value("IsAllCurrencies");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Open Amount.
@param OpenAmt Open item amount */
public void SetOpenAmt (Decimal? OpenAmt)
{
if (OpenAmt == null) throw new ArgumentException ("OpenAmt is mandatory.");
Set_Value ("OpenAmt", (Decimal?)OpenAmt);
}
/** Get Open Amount.
@return Open item amount */
public Decimal GetOpenAmt() 
{
Object bd =Get_Value("OpenAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Percent.
@param PercentGL Gain / Loss Percentage */
public void SetPercentGL (Decimal? PercentGL)
{
Set_Value ("PercentGL", (Decimal?)PercentGL);
}
/** Get Percent.
@return Gain / Loss Percentage */
public Decimal GetPercentGL() 
{
Object bd =Get_Value("PercentGL");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}