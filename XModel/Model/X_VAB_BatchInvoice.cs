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
/** Generated Model for VAB_BatchInvoice
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_BatchInvoice : PO
{
public X_VAB_BatchInvoice (Context ctx, int VAB_BatchInvoice_ID, Trx trxName) : base (ctx, VAB_BatchInvoice_ID, trxName)
{
/** if (VAB_BatchInvoice_ID == 0)
{
SetVAB_Currency_ID (0);	// @$VAB_Currency_ID@
SetVAB_BatchInvoice_ID (0);
SetControlAmt (0.0);	// 0
SetDateDoc (DateTime.Now);	// @#Date@
SetDocumentAmt (0.0);
SetDocumentNo (null);
SetIsSOTrx (false);	// N
SetProcessed (false);	// N
SetSalesRep_ID (0);
}
 */
}
public X_VAB_BatchInvoice (Ctx ctx, int VAB_BatchInvoice_ID, Trx trxName) : base (ctx, VAB_BatchInvoice_ID, trxName)
{
/** if (VAB_BatchInvoice_ID == 0)
{
SetVAB_Currency_ID (0);	// @$VAB_Currency_ID@
SetVAB_BatchInvoice_ID (0);
SetControlAmt (0.0);	// 0
SetDateDoc (DateTime.Now);	// @#Date@
SetDocumentAmt (0.0);
SetDocumentNo (null);
SetIsSOTrx (false);	// N
SetProcessed (false);	// N
SetSalesRep_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BatchInvoice (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BatchInvoice (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BatchInvoice (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_BatchInvoice()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372375L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055586L;
/** VAF_TableView_ID=767 */
public static int Table_ID;
 // =767;

/** TableName=VAB_BatchInvoice */
public static String Table_Name="VAB_BatchInvoice";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_BatchInvoice[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Currency Type.
@param VAB_CurrencyType_ID Currency Conversion Rate Type */
public void SetVAB_CurrencyType_ID (int VAB_CurrencyType_ID)
{
if (VAB_CurrencyType_ID <= 0) Set_Value ("VAB_CurrencyType_ID", null);
else
Set_Value ("VAB_CurrencyType_ID", VAB_CurrencyType_ID);
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
public void SetVAB_Currency_ID (int VAB_Currency_ID)
{
if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");
Set_Value ("VAB_Currency_ID", VAB_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() 
{
Object ii = Get_Value("VAB_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Batch.
@param VAB_BatchInvoice_ID Expense Invoice Batch Header */
public void SetVAB_BatchInvoice_ID (int VAB_BatchInvoice_ID)
{
if (VAB_BatchInvoice_ID < 1) throw new ArgumentException ("VAB_BatchInvoice_ID is mandatory.");
Set_ValueNoCheck ("VAB_BatchInvoice_ID", VAB_BatchInvoice_ID);
}
/** Get Invoice Batch.
@return Expense Invoice Batch Header */
public int GetVAB_BatchInvoice_ID() 
{
Object ii = Get_Value("VAB_BatchInvoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Control Amount.
@param ControlAmt If not zero, the Debit amount of the document must be equal this amount */
public void SetControlAmt (Decimal? ControlAmt)
{
if (ControlAmt == null) throw new ArgumentException ("ControlAmt is mandatory.");
Set_Value ("ControlAmt", (Decimal?)ControlAmt);
}
/** Get Control Amount.
@return If not zero, the Debit amount of the document must be equal this amount */
public Decimal GetControlAmt() 
{
Object bd =Get_Value("ControlAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Document Date.
@param DateDoc Date of the Document */
public void SetDateDoc (DateTime? DateDoc)
{
if (DateDoc == null) throw new ArgumentException ("DateDoc is mandatory.");
Set_Value ("DateDoc", (DateTime?)DateDoc);
}
/** Get Document Date.
@return Date of the Document */
public DateTime? GetDateDoc() 
{
return (DateTime?)Get_Value("DateDoc");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Document Amt.
@param DocumentAmt Document Amount */
public void SetDocumentAmt (Decimal? DocumentAmt)
{
if (DocumentAmt == null) throw new ArgumentException ("DocumentAmt is mandatory.");
Set_ValueNoCheck ("DocumentAmt", (Decimal?)DocumentAmt);
}
/** Get Document Amt.
@return Document Amount */
public Decimal GetDocumentAmt() 
{
Object bd =Get_Value("DocumentAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Document No.
@param DocumentNo Document sequence number of the document */
public void SetDocumentNo (String DocumentNo)
{
if (DocumentNo == null) throw new ArgumentException ("DocumentNo is mandatory.");
if (DocumentNo.Length > 30)
{
log.Warning("Length > 30 - truncated");
DocumentNo = DocumentNo.Substring(0,30);
}
Set_Value ("DocumentNo", DocumentNo);
}
/** Get Document No.
@return Document sequence number of the document */
public String GetDocumentNo() 
{
return (String)Get_Value("DocumentNo");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDocumentNo());
}
/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx)
{
Set_Value ("IsSOTrx", IsSOTrx);
}
/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() 
{
Object oo = Get_Value("IsSOTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** SalesRep_ID VAF_Control_Ref_ID=190 */
public static int SALESREP_ID_VAF_Control_Ref_ID=190;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID < 1) throw new ArgumentException ("SalesRep_ID is mandatory.");
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}