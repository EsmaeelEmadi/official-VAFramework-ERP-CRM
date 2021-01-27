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
/** Generated Model for VAGL_Fund
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_Fund : PO
{
public X_VAGL_Fund (Context ctx, int VAGL_Fund_ID, Trx trxName) : base (ctx, VAGL_Fund_ID, trxName)
{
/** if (VAGL_Fund_ID == 0)
{
SetAmt (0.0);
SetVAB_AccountBook_ID (0);
SetVAGL_Fund_ID (0);
SetName (null);
}
 */
}
public X_VAGL_Fund (Ctx ctx, int VAGL_Fund_ID, Trx trxName) : base (ctx, VAGL_Fund_ID, trxName)
{
/** if (VAGL_Fund_ID == 0)
{
SetAmt (0.0);
SetVAB_AccountBook_ID (0);
SetVAGL_Fund_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Fund (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Fund (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Fund (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_Fund()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376513L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059724L;
/** VAF_TableView_ID=823 */
public static int Table_ID;
 // =823;

/** TableName=VAGL_Fund */
public static String Table_Name="VAGL_Fund";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_VAGL_Fund[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Amount.
@param Amt Amount */
public void SetAmt (Decimal? Amt)
{
if (Amt == null) throw new ArgumentException ("Amt is mandatory.");
Set_Value ("Amt", (Decimal?)Amt);
}
/** Get Amount.
@return Amount */
public Decimal GetAmt() 
{
Object bd =Get_Value("Amt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_Value ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date From.
@param DateFrom Starting date for a range */
public void SetDateFrom (DateTime? DateFrom)
{
Set_Value ("DateFrom", (DateTime?)DateFrom);
}
/** Get Date From.
@return Starting date for a range */
public DateTime? GetDateFrom() 
{
return (DateTime?)Get_Value("DateFrom");
}
/** Set Date To.
@param DateTo End date of a date range */
public void SetDateTo (DateTime? DateTo)
{
Set_Value ("DateTo", (DateTime?)DateTo);
}
/** Get Date To.
@return End date of a date range */
public DateTime? GetDateTo() 
{
return (DateTime?)Get_Value("DateTo");
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
/** Set GL Fund.
@param VAGL_Fund_ID General Ledger Funds Control */
public void SetVAGL_Fund_ID (int VAGL_Fund_ID)
{
if (VAGL_Fund_ID < 1) throw new ArgumentException ("VAGL_Fund_ID is mandatory.");
Set_ValueNoCheck ("VAGL_Fund_ID", VAGL_Fund_ID);
}
/** Get GL Fund.
@return General Ledger Funds Control */
public int GetVAGL_Fund_ID() 
{
Object ii = Get_Value("VAGL_Fund_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
}

}
