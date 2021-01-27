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
/** Generated Model for VAB_EmployeeCompensation
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_EmployeeCompensation : PO
{
public X_VAB_EmployeeCompensation (Context ctx, int VAB_EmployeeCompensation_ID, Trx trxName) : base (ctx, VAB_EmployeeCompensation_ID, trxName)
{
/** if (VAB_EmployeeCompensation_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_Compensation_ID (0);
SetVAB_EmployeeCompensation_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_VAB_EmployeeCompensation (Ctx ctx, int VAB_EmployeeCompensation_ID, Trx trxName) : base (ctx, VAB_EmployeeCompensation_ID, trxName)
{
/** if (VAB_EmployeeCompensation_ID == 0)
{
SetVAF_UserContact_ID (0);
SetVAB_Compensation_ID (0);
SetVAB_EmployeeCompensation_ID (0);
SetGrossRAmt (0.0);
SetGrossRCost (0.0);
SetOvertimeAmt (0.0);
SetOvertimeCost (0.0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_EmployeeCompensation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_EmployeeCompensation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_EmployeeCompensation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_EmployeeCompensation()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375823L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059034L;
/** VAF_TableView_ID=794 */
public static int Table_ID;
 // =794;

/** TableName=VAB_EmployeeCompensation */
public static String Table_Name="VAB_EmployeeCompensation";

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
StringBuilder sb = new StringBuilder ("X_VAB_EmployeeCompensation[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID < 1) throw new ArgumentException ("VAF_UserContact_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_UserContact_ID().ToString());
}
/** Set Remuneration.
@param VAB_Compensation_ID Wage or Salary */
public void SetVAB_Compensation_ID (int VAB_Compensation_ID)
{
if (VAB_Compensation_ID < 1) throw new ArgumentException ("VAB_Compensation_ID is mandatory.");
Set_ValueNoCheck ("VAB_Compensation_ID", VAB_Compensation_ID);
}
/** Get Remuneration.
@return Wage or Salary */
public int GetVAB_Compensation_ID() 
{
Object ii = Get_Value("VAB_Compensation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Employee Remuneration.
@param VAB_EmployeeCompensation_ID Employee Wage or Salary Overwrite */
public void SetVAB_EmployeeCompensation_ID (int VAB_EmployeeCompensation_ID)
{
if (VAB_EmployeeCompensation_ID < 1) throw new ArgumentException ("VAB_EmployeeCompensation_ID is mandatory.");
Set_ValueNoCheck ("VAB_EmployeeCompensation_ID", VAB_EmployeeCompensation_ID);
}
/** Get Employee Remuneration.
@return Employee Wage or Salary Overwrite */
public int GetVAB_EmployeeCompensation_ID() 
{
Object ii = Get_Value("VAB_EmployeeCompensation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Gross Amount.
@param GrossRAmt Gross Remuneration Amount */
public void SetGrossRAmt (Decimal? GrossRAmt)
{
if (GrossRAmt == null) throw new ArgumentException ("GrossRAmt is mandatory.");
Set_Value ("GrossRAmt", (Decimal?)GrossRAmt);
}
/** Get Gross Amount.
@return Gross Remuneration Amount */
public Decimal GetGrossRAmt() 
{
Object bd =Get_Value("GrossRAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Gross Cost.
@param GrossRCost Gross Remuneration Costs */
public void SetGrossRCost (Decimal? GrossRCost)
{
if (GrossRCost == null) throw new ArgumentException ("GrossRCost is mandatory.");
Set_Value ("GrossRCost", (Decimal?)GrossRCost);
}
/** Get Gross Cost.
@return Gross Remuneration Costs */
public Decimal GetGrossRCost() 
{
Object bd =Get_Value("GrossRCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Overtime Amount.
@param OvertimeAmt Hourly Overtime Rate */
public void SetOvertimeAmt (Decimal? OvertimeAmt)
{
if (OvertimeAmt == null) throw new ArgumentException ("OvertimeAmt is mandatory.");
Set_Value ("OvertimeAmt", (Decimal?)OvertimeAmt);
}
/** Get Overtime Amount.
@return Hourly Overtime Rate */
public Decimal GetOvertimeAmt() 
{
Object bd =Get_Value("OvertimeAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Overtime Cost.
@param OvertimeCost Hourly Overtime Cost */
public void SetOvertimeCost (Decimal? OvertimeCost)
{
if (OvertimeCost == null) throw new ArgumentException ("OvertimeCost is mandatory.");
Set_Value ("OvertimeCost", (Decimal?)OvertimeCost);
}
/** Get Overtime Cost.
@return Hourly Overtime Cost */
public Decimal GetOvertimeCost() 
{
Object bd =Get_Value("OvertimeCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
if (ValidFrom == null) throw new ArgumentException ("ValidFrom is mandatory.");
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}
