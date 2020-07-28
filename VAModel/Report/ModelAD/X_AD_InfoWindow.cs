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
/** Generated Model for AD_InfoWindow
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_InfoWindow : PO
{
public X_AD_InfoWindow (Context ctx, int AD_InfoWindow_ID, Trx trxName) : base (ctx, AD_InfoWindow_ID, trxName)
{
/** if (AD_InfoWindow_ID == 0)
{
SetAD_InfoWindow_ID (0);
SetEntityType (null);	// U
SetFromClause (null);
SetName (null);
}
 */
}
public X_AD_InfoWindow (Ctx ctx, int AD_InfoWindow_ID, Trx trxName) : base (ctx, AD_InfoWindow_ID, trxName)
{
/** if (AD_InfoWindow_ID == 0)
{
SetAD_InfoWindow_ID (0);
SetEntityType (null);	// U
SetFromClause (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InfoWindow (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InfoWindow (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_InfoWindow (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_InfoWindow()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361812L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045023L;
/** AD_Table_ID=895 */
public static int Table_ID;
 // =895;

/** TableName=AD_InfoWindow */
public static String Table_Name="AD_InfoWindow";

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
StringBuilder sb = new StringBuilder ("X_AD_InfoWindow[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Info Window.
@param AD_InfoWindow_ID Info and search/select Window */
public void SetAD_InfoWindow_ID (int AD_InfoWindow_ID)
{
if (AD_InfoWindow_ID < 1) throw new ArgumentException ("AD_InfoWindow_ID is mandatory.");
Set_ValueNoCheck ("AD_InfoWindow_ID", AD_InfoWindow_ID);
}
/** Get Info Window.
@return Info and search/select Window */
public int GetAD_InfoWindow_ID() 
{
Object ii = Get_Value("AD_InfoWindow_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Sql FROM.
@param FromClause SQL FROM clause */
public void SetFromClause (String FromClause)
{
if (FromClause == null) throw new ArgumentException ("FromClause is mandatory.");
if (FromClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
FromClause = FromClause.Substring(0,2000);
}
Set_Value ("FromClause", FromClause);
}
/** Get Sql FROM.
@return SQL FROM clause */
public String GetFromClause() 
{
return (String)Get_Value("FromClause");
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
/** Set Customization Default.
@param IsCustomDefault Default Customization */
public void SetIsCustomDefault (Boolean IsCustomDefault)
{
Set_Value ("IsCustomDefault", IsCustomDefault);
}
/** Get Customization Default.
@return Default Customization */
public Boolean IsCustomDefault() 
{
Object oo = Get_Value("IsCustomDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Other SQL Clause.
@param OtherClause Other SQL Clause */
public void SetOtherClause (String OtherClause)
{
if (OtherClause != null && OtherClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OtherClause = OtherClause.Substring(0,2000);
}
Set_Value ("OtherClause", OtherClause);
}
/** Get Other SQL Clause.
@return Other SQL Clause */
public String GetOtherClause() 
{
return (String)Get_Value("OtherClause");
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
}

}
