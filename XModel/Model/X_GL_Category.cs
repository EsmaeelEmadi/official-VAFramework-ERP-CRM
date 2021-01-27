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
/** Generated Model for VAGL_Group
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAGL_Group : PO
{
public X_VAGL_Group (Context ctx, int VAGL_Group_ID, Trx trxName) : base (ctx, VAGL_Group_ID, trxName)
{
/** if (VAGL_Group_ID == 0)
{
SetCategoryType (null);	// M
SetVAGL_Group_ID (0);
SetIsDefault (false);
SetName (null);
}
 */
}
public X_VAGL_Group (Ctx ctx, int VAGL_Group_ID, Trx trxName) : base (ctx, VAGL_Group_ID, trxName)
{
/** if (VAGL_Group_ID == 0)
{
SetCategoryType (null);	// M
SetVAGL_Group_ID (0);
SetIsDefault (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Group (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Group (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAGL_Group (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAGL_Group()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376340L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059551L;
/** VAF_TableView_ID=218 */
public static int Table_ID;
 // =218;

/** TableName=VAGL_Group */
public static String Table_Name="VAGL_Group";

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
StringBuilder sb = new StringBuilder ("X_VAGL_Group[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** CategoryType VAF_Control_Ref_ID=207 */
public static int CATEGORYTYPE_VAF_Control_Ref_ID=207;
/** Document = D */
public static String CATEGORYTYPE_Document = "D";
/** Import = I */
public static String CATEGORYTYPE_Import = "I";
/** Manual = M */
public static String CATEGORYTYPE_Manual = "M";
/** System generated = S */
public static String CATEGORYTYPE_SystemGenerated = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCategoryTypeValid (String test)
{
return test.Equals("D") || test.Equals("I") || test.Equals("M") || test.Equals("S");
}
/** Set Category Type.
@param CategoryType Source of the Journal with this category */
public void SetCategoryType (String CategoryType)
{
if (CategoryType == null) throw new ArgumentException ("CategoryType is mandatory");
if (!IsCategoryTypeValid(CategoryType))
throw new ArgumentException ("CategoryType Invalid value - " + CategoryType + " - Reference_ID=207 - D - I - M - S");
if (CategoryType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CategoryType = CategoryType.Substring(0,1);
}
Set_Value ("CategoryType", CategoryType);
}
/** Get Category Type.
@return Source of the Journal with this category */
public String GetCategoryType() 
{
return (String)Get_Value("CategoryType");
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
/** Set GL Category.
@param VAGL_Group_ID General Ledger Category */
public void SetVAGL_Group_ID (int VAGL_Group_ID)
{
if (VAGL_Group_ID < 1) throw new ArgumentException ("VAGL_Group_ID is mandatory.");
Set_ValueNoCheck ("VAGL_Group_ID", VAGL_Group_ID);
}
/** Get GL Category.
@return General Ledger Category */
public int GetVAGL_Group_ID() 
{
Object ii = Get_Value("VAGL_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
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
}

}
