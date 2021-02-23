namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for C_CurrCrossRate
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_CurrCrossRate : PO{public X_C_CurrCrossRate (Context ctx, int C_CurrCrossRate_ID, Trx trxName) : base (ctx, C_CurrCrossRate_ID, trxName){/** if (C_CurrCrossRate_ID == 0){SetVAB_CurrencyType_ID (0);SetC_CurrCrossRate_ID (0);SetVAB_Currency_From_ID (0);SetVAB_Currency_ID (0);SetVAB_Currency_To_ID (0);} */
}public X_C_CurrCrossRate (Ctx ctx, int C_CurrCrossRate_ID, Trx trxName) : base (ctx, C_CurrCrossRate_ID, trxName){/** if (C_CurrCrossRate_ID == 0){SetVAB_CurrencyType_ID (0);SetC_CurrCrossRate_ID (0);SetVAB_Currency_From_ID (0);SetVAB_Currency_ID (0);SetVAB_Currency_To_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CurrCrossRate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CurrCrossRate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_CurrCrossRate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_CurrCrossRate(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27881088188083L;/** Last Updated Timestamp 9/1/2020 12:21:11 PM */
public static long updatedMS = 1598962871294L;/** VAF_TableView_ID=1000545 */
public static int Table_ID; // =1000545;
/** TableName=C_CurrCrossRate */
public static String Table_Name="C_CurrCrossRate";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_C_CurrCrossRate[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Currency Rate Type.
@param VAB_CurrencyType_ID Currency Conversion Rate Type */
public void SetVAB_CurrencyType_ID (int VAB_CurrencyType_ID){if (VAB_CurrencyType_ID < 1) throw new ArgumentException ("VAB_CurrencyType_ID is mandatory.");Set_Value ("VAB_CurrencyType_ID", VAB_CurrencyType_ID);}/** Get Currency Rate Type.
@return Currency Conversion Rate Type */
public int GetVAB_CurrencyType_ID() {Object ii = Get_Value("VAB_CurrencyType_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cross Rate Setting.
@param C_CurrCrossRate_ID Cross rates are the relation of two currencies against each other */
public void SetC_CurrCrossRate_ID (int C_CurrCrossRate_ID){if (C_CurrCrossRate_ID < 1) throw new ArgumentException ("C_CurrCrossRate_ID is mandatory.");Set_ValueNoCheck ("C_CurrCrossRate_ID", C_CurrCrossRate_ID);}/** Get Cross Rate Setting.
@return Cross rates are the relation of two currencies against each other */
public int GetC_CurrCrossRate_ID() {Object ii = Get_Value("C_CurrCrossRate_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** VAB_Currency_From_ID VAF_Control_Ref_ID=112 */
public static int VAB_CURRENCY_FROM_ID_VAF_Control_Ref_ID=112;/** Set From Currency.
@param VAB_Currency_From_ID From Currency */
public void SetVAB_Currency_From_ID (int VAB_Currency_From_ID){if (VAB_Currency_From_ID < 1) throw new ArgumentException ("VAB_Currency_From_ID is mandatory.");Set_Value ("VAB_Currency_From_ID", VAB_Currency_From_ID);}/** Get From Currency.
@return From Currency */
public int GetVAB_Currency_From_ID() {Object ii = Get_Value("VAB_Currency_From_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Currency.
@param VAB_Currency_ID The Currency for this record */
public void SetVAB_Currency_ID (int VAB_Currency_ID){if (VAB_Currency_ID < 1) throw new ArgumentException ("VAB_Currency_ID is mandatory.");Set_Value ("VAB_Currency_ID", VAB_Currency_ID);}/** Get Currency.
@return The Currency for this record */
public int GetVAB_Currency_ID() {Object ii = Get_Value("VAB_Currency_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}
/** VAB_Currency_To_ID VAF_Control_Ref_ID=112 */
public static int VAB_CURRENCY_TO_ID_VAF_Control_Ref_ID=112;/** Set Currency To.
@param VAB_Currency_To_ID Target currency */
public void SetVAB_Currency_To_ID (int VAB_Currency_To_ID){if (VAB_Currency_To_ID < 1) throw new ArgumentException ("VAB_Currency_To_ID is mandatory.");Set_Value ("VAB_Currency_To_ID", VAB_Currency_To_ID);}/** Get Currency To.
@return Target currency */
public int GetVAB_Currency_To_ID() {Object ii = Get_Value("VAB_Currency_To_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name != null && Name.Length > 40){log.Warning("Length > 40 - truncated");Name = Name.Substring(0,40);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}}
}