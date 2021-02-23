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
/** Generated Model for VAB_ForecastLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ForecastLine : PO
{
public X_VAB_ForecastLine (Context ctx, int VAB_ForecastLine_ID, Trx trxName) : base (ctx, VAB_ForecastLine_ID, trxName)
{
/** if (VAB_ForecastLine_ID == 0)
{
SetVAB_ForecastLine_ID (0);
SetVAB_Forecast_ID (0);
}
 */
}
public X_VAB_ForecastLine (Ctx ctx, int VAB_ForecastLine_ID, Trx trxName) : base (ctx, VAB_ForecastLine_ID, trxName)
{
/** if (VAB_ForecastLine_ID == 0)
{
SetVAB_ForecastLine_ID (0);
SetVAB_Forecast_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ForecastLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ForecastLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ForecastLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ForecastLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27609451562317L;
/** Last Updated Timestamp 1/23/2012 7:14:05 PM */
public static long updatedMS = 1327326245528L;
/** VAF_TableView_ID=1000245 */
public static int Table_ID;
 // =1000245;

/** TableName=VAB_ForecastLine */
public static String Table_Name="VAB_ForecastLine";

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
StringBuilder sb = new StringBuilder ("X_VAB_ForecastLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_ForecastLine_ID.
@param VAB_ForecastLine_ID VAB_ForecastLine_ID */
public void SetVAB_ForecastLine_ID (int VAB_ForecastLine_ID)
{
if (VAB_ForecastLine_ID < 1) throw new ArgumentException ("VAB_ForecastLine_ID is mandatory.");
Set_ValueNoCheck ("VAB_ForecastLine_ID", VAB_ForecastLine_ID);
}
/** Get VAB_ForecastLine_ID.
@return VAB_ForecastLine_ID */
public int GetVAB_ForecastLine_ID() 
{
Object ii = Get_Value("VAB_ForecastLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Forecast.
@param VAB_Forecast_ID Forecast */
public void SetVAB_Forecast_ID (int VAB_Forecast_ID)
{
if (VAB_Forecast_ID < 1) throw new ArgumentException ("VAB_Forecast_ID is mandatory.");
Set_ValueNoCheck ("VAB_Forecast_ID", VAB_Forecast_ID);
}
/** Get Forecast.
@return Forecast */
public int GetVAB_Forecast_ID() 
{
Object ii = Get_Value("VAB_Forecast_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param VAB_UOM_ID Unit of Measure */
public void SetVAB_UOM_ID (int VAB_UOM_ID)
{
if (VAB_UOM_ID <= 0) Set_Value ("VAB_UOM_ID", null);
else
Set_Value ("VAB_UOM_ID", VAB_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetVAB_UOM_ID() 
{
Object ii = Get_Value("VAB_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAM_Product_ID VAF_Control_Ref_ID=162 */
public static int VAM_Product_ID_VAF_Control_Ref_ID=162;
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID <= 0) Set_Value ("VAM_Product_ID", null);
else
Set_Value ("VAM_Product_ID", VAM_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetVAM_Product_ID() 
{
Object ii = Get_Value("VAM_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Standard Price.
@param PriceStd Standard Price */
public void SetPriceStd (Decimal? PriceStd)
{
Set_Value ("PriceStd", (Decimal?)PriceStd);
}
/** Get Standard Price.
@return Standard Price */
public Decimal GetPriceStd() 
{
Object bd =Get_Value("PriceStd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
public void SetQtyEntered (Decimal? QtyEntered)
{
Set_Value ("QtyEntered", (Decimal?)QtyEntered);
}
/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
public Decimal GetQtyEntered() 
{
Object bd =Get_Value("QtyEntered");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}