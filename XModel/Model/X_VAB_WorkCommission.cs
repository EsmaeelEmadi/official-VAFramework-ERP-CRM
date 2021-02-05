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
/** Generated Model for VAB_WorkCommission
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_WorkCommission : PO
{
public X_VAB_WorkCommission (Context ctx, int VAB_WorkCommission_ID, Trx trxName) : base (ctx, VAB_WorkCommission_ID, trxName)
{
/** if (VAB_WorkCommission_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_Charge_ID (0);
SetVAB_WorkCommission_ID (0);
SetVAB_Currency_ID (0);
SetDocBasisType (null);	// I
SetFrequencyType (null);	// M
SetListDetails (false);
SetName (null);
}
 */
}
public X_VAB_WorkCommission (Ctx ctx, int VAB_WorkCommission_ID, Trx trxName) : base (ctx, VAB_WorkCommission_ID, trxName)
{
/** if (VAB_WorkCommission_ID == 0)
{
SetVAB_BusinessPartner_ID (0);
SetVAB_Charge_ID (0);
SetVAB_WorkCommission_ID (0);
SetVAB_Currency_ID (0);
SetDocBasisType (null);	// I
SetFrequencyType (null);	// M
SetListDetails (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_WorkCommission (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_WorkCommission()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371263L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054474L;
/** VAF_TableView_ID=429 */
public static int Table_ID;
 // =429;

/** TableName=VAB_WorkCommission */
public static String Table_Name="VAB_WorkCommission";

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
StringBuilder sb = new StringBuilder ("X_VAB_WorkCommission[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAB_BusinessPartner_ID VAF_Control_Ref_ID=232 */
public static int VAB_BUSINESSPARTNER_ID_VAF_Control_Ref_ID=232;
/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Business Partner */
public void SetVAB_BusinessPartner_ID (int VAB_BusinessPartner_ID)
{
if (VAB_BusinessPartner_ID < 1) throw new ArgumentException ("VAB_BusinessPartner_ID is mandatory.");
Set_Value ("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetVAB_BusinessPartner_ID() 
{
Object ii = Get_Value("VAB_BusinessPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge.
@param VAB_Charge_ID Additional document charges */
public void SetVAB_Charge_ID (int VAB_Charge_ID)
{
if (VAB_Charge_ID < 1) throw new ArgumentException ("VAB_Charge_ID is mandatory.");
Set_Value ("VAB_Charge_ID", VAB_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetVAB_Charge_ID() 
{
Object ii = Get_Value("VAB_Charge_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commission.
@param VAB_WorkCommission_ID Commission */
public void SetVAB_WorkCommission_ID (int VAB_WorkCommission_ID)
{
if (VAB_WorkCommission_ID < 1) throw new ArgumentException ("VAB_WorkCommission_ID is mandatory.");
Set_ValueNoCheck ("VAB_WorkCommission_ID", VAB_WorkCommission_ID);
}
/** Get Commission.
@return Commission */
public int GetVAB_WorkCommission_ID() 
{
Object ii = Get_Value("VAB_WorkCommission_ID");
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
/** Set Create lines from.
@param CreateFrom Process which will generate a new document lines based on an existing document */
public void SetCreateFrom (String CreateFrom)
{
if (CreateFrom != null && CreateFrom.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateFrom = CreateFrom.Substring(0,1);
}
Set_Value ("CreateFrom", CreateFrom);
}
/** Get Create lines from.
@return Process which will generate a new document lines based on an existing document */
public String GetCreateFrom() 
{
return (String)Get_Value("CreateFrom");
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_ValueNoCheck ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
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

/** DocBasisType VAF_Control_Ref_ID=224 */
public static int DOCBASISTYPE_VAF_Control_Ref_ID=224;
/** Invoice = I */
public static String DOCBASISTYPE_Invoice = "I";
/** Order = O */
public static String DOCBASISTYPE_Order = "O";
/** Receipt = R */
public static String DOCBASISTYPE_Receipt = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDocBasisTypeValid (String test)
{
return test.Equals("I") || test.Equals("O") || test.Equals("R");
}
/** Set Calculation Basis.
@param DocBasisType Basis for the calculation the commission */
public void SetDocBasisType (String DocBasisType)
{
if (DocBasisType == null) throw new ArgumentException ("DocBasisType is mandatory");
if (!IsDocBasisTypeValid(DocBasisType))
throw new ArgumentException ("DocBasisType Invalid value - " + DocBasisType + " - Reference_ID=224 - I - O - R");
if (DocBasisType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DocBasisType = DocBasisType.Substring(0,1);
}
Set_Value ("DocBasisType", DocBasisType);
}
/** Get Calculation Basis.
@return Basis for the calculation the commission */
public String GetDocBasisType() 
{
return (String)Get_Value("DocBasisType");
}

/** FrequencyType VAF_Control_Ref_ID=225 */
public static int FREQUENCYTYPE_VAF_Control_Ref_ID=225;
/** Monthly = M */
public static String FREQUENCYTYPE_Monthly = "M";
/** Quarterly = Q */
public static String FREQUENCYTYPE_Quarterly = "Q";
/** Weekly = W */
public static String FREQUENCYTYPE_Weekly = "W";
/** Yearly = Y */
public static String FREQUENCYTYPE_Yearly = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFrequencyTypeValid (String test)
{
return test.Equals("M") || test.Equals("Q") || test.Equals("W") || test.Equals("Y");
}
/** Set Frequency Type.
@param FrequencyType Frequency of event */
public void SetFrequencyType (String FrequencyType)
{
if (FrequencyType == null) throw new ArgumentException ("FrequencyType is mandatory");
if (!IsFrequencyTypeValid(FrequencyType))
throw new ArgumentException ("FrequencyType Invalid value - " + FrequencyType + " - Reference_ID=225 - M - Q - W - Y");
if (FrequencyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
FrequencyType = FrequencyType.Substring(0,1);
}
Set_Value ("FrequencyType", FrequencyType);
}
/** Get Frequency Type.
@return Frequency of event */
public String GetFrequencyType() 
{
return (String)Get_Value("FrequencyType");
}
/** Set List Details.
@param ListDetails List document details */
public void SetListDetails (Boolean ListDetails)
{
Set_Value ("ListDetails", ListDetails);
}
/** Get List Details.
@return List document details */
public Boolean IsListDetails() 
{
Object oo = Get_Value("ListDetails");
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