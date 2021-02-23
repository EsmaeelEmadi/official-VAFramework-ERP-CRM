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
/** Generated Model for VAB_Project_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Project_Acct : PO
{
public X_VAB_Project_Acct (Context ctx, int VAB_Project_Acct_ID, Trx trxName) : base (ctx, VAB_Project_Acct_ID, trxName)
{
/** if (VAB_Project_Acct_ID == 0)
{
SetVAB_AccountBook_ID (0);
SetVAB_Project_ID (0);
SetPJ_Asset_Acct (0);
SetPJ_WIP_Acct (0);
}
 */
}
public X_VAB_Project_Acct (Ctx ctx, int VAB_Project_Acct_ID, Trx trxName) : base (ctx, VAB_Project_Acct_ID, trxName)
{
/** if (VAB_Project_Acct_ID == 0)
{
SetVAB_AccountBook_ID (0);
SetVAB_Project_ID (0);
SetPJ_Asset_Acct (0);
SetPJ_WIP_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Project_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Project_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Project_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Project_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374397L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057608L;
/** VAF_TableView_ID=204 */
public static int Table_ID;
 // =204;

/** TableName=VAB_Project_Acct */
public static String Table_Name="VAB_Project_Acct";

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
StringBuilder sb = new StringBuilder ("X_VAB_Project_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
public void SetVAB_AccountBook_ID (int VAB_AccountBook_ID)
{
if (VAB_AccountBook_ID < 1) throw new ArgumentException ("VAB_AccountBook_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountBook_ID", VAB_AccountBook_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetVAB_AccountBook_ID() 
{
Object ii = Get_Value("VAB_AccountBook_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_AccountBook_ID().ToString());
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID < 1) throw new ArgumentException ("VAB_Project_ID is mandatory.");
Set_ValueNoCheck ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Asset.
@param PJ_Asset_Acct Project Asset Account */
public void SetPJ_Asset_Acct (int PJ_Asset_Acct)
{
Set_Value ("PJ_Asset_Acct", PJ_Asset_Acct);
}
/** Get Project Asset.
@return Project Asset Account */
public int GetPJ_Asset_Acct() 
{
Object ii = Get_Value("PJ_Asset_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Work In Progress.
@param PJ_WIP_Acct Account for Work in Progress */
public void SetPJ_WIP_Acct (int PJ_WIP_Acct)
{
Set_Value ("PJ_WIP_Acct", PJ_WIP_Acct);
}
/** Get Work In Progress.
@return Account for Work in Progress */
public int GetPJ_WIP_Acct() 
{
Object ii = Get_Value("PJ_WIP_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}