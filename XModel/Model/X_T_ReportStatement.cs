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
/** Generated Model for T_ReportStatement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_ReportStatement : PO
{
public X_T_ReportStatement (Context ctx, int T_ReportStatement_ID, Trx trxName) : base (ctx, T_ReportStatement_ID, trxName)
{
/** if (T_ReportStatement_ID == 0)
{
SetVAF_JInstance_ID (0);
SetDateAcct (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetLevelNo (0);
}
 */
}
public X_T_ReportStatement (Ctx ctx, int T_ReportStatement_ID, Trx trxName) : base (ctx, T_ReportStatement_ID, trxName)
{
/** if (T_ReportStatement_ID == 0)
{
SetVAF_JInstance_ID (0);
SetDateAcct (DateTime.Now);
SetActual_Acct_Detail_ID (0);
SetLevelNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ReportStatement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ReportStatement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_ReportStatement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_ReportStatement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384396L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067607L;
/** VAF_TableView_ID=545 */
public static int Table_ID;
 // =545;

/** TableName=T_ReportStatement */
public static String Table_Name="T_ReportStatement";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_T_ReportStatement[").Append(Get_ID()).Append("]");
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
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
Set_ValueNoCheck ("AmtAcctCr", (Decimal?)AmtAcctCr);
}
/** Get Accounted Credit.
@return Accounted Credit Amount */
public Decimal GetAmtAcctCr() 
{
Object bd =Get_Value("AmtAcctCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounted Debit.
@param AmtAcctDr Accounted Debit Amount */
public void SetAmtAcctDr (Decimal? AmtAcctDr)
{
Set_ValueNoCheck ("AmtAcctDr", (Decimal?)AmtAcctDr);
}
/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() 
{
Object bd =Get_Value("AmtAcctDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Balance.
@param Balance Balance */
public void SetBalance (Decimal? Balance)
{
Set_ValueNoCheck ("Balance", (Decimal?)Balance);
}
/** Get Balance.
@return Balance */
public Decimal GetBalance() 
{
Object bd =Get_Value("Balance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");
Set_ValueNoCheck ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
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
Set_ValueNoCheck ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
/** Set Level no.
@param LevelNo Level no */
public void SetLevelNo (int LevelNo)
{
Set_ValueNoCheck ("LevelNo", LevelNo);
}
/** Get Level no.
@return Level no */
public int GetLevelNo() 
{
Object ii = Get_Value("LevelNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_ValueNoCheck ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_ValueNoCheck ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
