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
/** Generated Model for VAT_Transaction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAT_Transaction : PO
{
public X_VAT_Transaction (Context ctx, int VAT_Transaction_ID, Trx trxName) : base (ctx, VAT_Transaction_ID, trxName)
{
/** if (VAT_Transaction_ID == 0)
{
SetVAF_JInstance_ID (0);
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_Locator_ID (0);
SetVAM_Product_ID (0);
SetVAM_Inv_Trx_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetMovementType (null);
}
 */
}
public X_VAT_Transaction (Ctx ctx, int VAT_Transaction_ID, Trx trxName) : base (ctx, VAT_Transaction_ID, trxName)
{
/** if (VAT_Transaction_ID == 0)
{
SetVAF_JInstance_ID (0);
SetVAM_PFeature_SetInstance_ID (0);
SetVAM_Locator_ID (0);
SetVAM_Product_ID (0);
SetVAM_Inv_Trx_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetMovementType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Transaction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Transaction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAT_Transaction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAT_Transaction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384569L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067780L;
/** VAF_TableView_ID=758 */
public static int Table_ID;
 // =758;

/** TableName=VAT_Transaction */
public static String Table_Name="VAT_Transaction";

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
StringBuilder sb = new StringBuilder ("X_VAT_Transaction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param VAF_JInstance_ID Instance of the process */
public void SetVAF_JInstance_ID (int VAF_JInstance_ID)
{
if (VAF_JInstance_ID < 1) throw new ArgumentException ("VAF_JInstance_ID is mandatory.");
Set_Value ("VAF_JInstance_ID", VAF_JInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetVAF_JInstance_ID() 
{
Object ii = Get_Value("VAF_JInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Issue.
@param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
public void SetVAB_ProjectSupply_ID (int VAB_ProjectSupply_ID)
{
if (VAB_ProjectSupply_ID <= 0) Set_Value ("VAB_ProjectSupply_ID", null);
else
Set_Value ("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetVAB_ProjectSupply_ID() 
{
Object ii = Get_Value("VAB_ProjectSupply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param VAB_Project_ID Financial Project */
public void SetVAB_Project_ID (int VAB_Project_ID)
{
if (VAB_Project_ID <= 0) Set_Value ("VAB_Project_ID", null);
else
Set_Value ("VAB_Project_ID", VAB_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetVAB_Project_ID() 
{
Object ii = Get_Value("VAB_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set Instance.
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
public void SetVAM_PFeature_SetInstance_ID (int VAM_PFeature_SetInstance_ID)
{
if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException ("VAM_PFeature_SetInstance_ID is mandatory.");
Set_Value ("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetVAM_PFeature_SetInstance_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt Line.
@param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
public void SetVAM_Inv_InOutLine_ID (int VAM_Inv_InOutLine_ID)
{
if (VAM_Inv_InOutLine_ID <= 0) Set_Value ("VAM_Inv_InOutLine_ID", null);
else
Set_Value ("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
}
/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
public int GetVAM_Inv_InOutLine_ID() 
{
Object ii = Get_Value("VAM_Inv_InOutLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt.
@param VAM_Inv_InOut_ID Material Shipment Document */
public void SetVAM_Inv_InOut_ID (int VAM_Inv_InOut_ID)
{
if (VAM_Inv_InOut_ID <= 0) Set_Value ("VAM_Inv_InOut_ID", null);
else
Set_Value ("VAM_Inv_InOut_ID", VAM_Inv_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetVAM_Inv_InOut_ID() 
{
Object ii = Get_Value("VAM_Inv_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phys Inventory Line.
@param VAM_InventoryLine_ID Unique line in an Inventory document */
public void SetVAM_InventoryLine_ID (int VAM_InventoryLine_ID)
{
if (VAM_InventoryLine_ID <= 0) Set_Value ("VAM_InventoryLine_ID", null);
else
Set_Value ("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
}
/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetVAM_InventoryLine_ID() 
{
Object ii = Get_Value("VAM_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phys.Inventory.
@param VAM_Inventory_ID Parameters for a Physical Inventory */
public void SetVAM_Inventory_ID (int VAM_Inventory_ID)
{
if (VAM_Inventory_ID <= 0) Set_Value ("VAM_Inventory_ID", null);
else
Set_Value ("VAM_Inventory_ID", VAM_Inventory_ID);
}
/** Get Phys.Inventory.
@return Parameters for a Physical Inventory */
public int GetVAM_Inventory_ID() 
{
Object ii = Get_Value("VAM_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param VAM_Locator_ID Warehouse Locator */
public void SetVAM_Locator_ID (int VAM_Locator_ID)
{
if (VAM_Locator_ID < 1) throw new ArgumentException ("VAM_Locator_ID is mandatory.");
Set_Value ("VAM_Locator_ID", VAM_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetVAM_Locator_ID() 
{
Object ii = Get_Value("VAM_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Line.
@param VAM_InvTrf_Line_ID Inventory Move document Line */
public void SetVAM_InvTrf_Line_ID (int VAM_InvTrf_Line_ID)
{
if (VAM_InvTrf_Line_ID <= 0) Set_Value ("VAM_InvTrf_Line_ID", null);
else
Set_Value ("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
}
/** Get Move Line.
@return Inventory Move document Line */
public int GetVAM_InvTrf_Line_ID() 
{
Object ii = Get_Value("VAM_InvTrf_Line_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Move.
@param VAM_InventoryTransfer_ID Movement of Inventory */
public void SetVAM_InventoryTransfer_ID (int VAM_InventoryTransfer_ID)
{
if (VAM_InventoryTransfer_ID <= 0) Set_Value ("VAM_InventoryTransfer_ID", null);
else
Set_Value ("VAM_InventoryTransfer_ID", VAM_InventoryTransfer_ID);
}
/** Get Inventory Move.
@return Movement of Inventory */
public int GetVAM_InventoryTransfer_ID() 
{
Object ii = Get_Value("VAM_InventoryTransfer_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param VAM_Product_ID Product, Service, Item */
public void SetVAM_Product_ID (int VAM_Product_ID)
{
if (VAM_Product_ID < 1) throw new ArgumentException ("VAM_Product_ID is mandatory.");
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
/** Set Production Line.
@param VAM_ProductionLine_ID Document Line representing a production */
public void SetVAM_ProductionLine_ID (int VAM_ProductionLine_ID)
{
if (VAM_ProductionLine_ID <= 0) Set_Value ("VAM_ProductionLine_ID", null);
else
Set_Value ("VAM_ProductionLine_ID", VAM_ProductionLine_ID);
}
/** Get Production Line.
@return Document Line representing a production */
public int GetVAM_ProductionLine_ID() 
{
Object ii = Get_Value("VAM_ProductionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Production.
@param VAM_Production_ID Plan for producing a product */
public void SetVAM_Production_ID (int VAM_Production_ID)
{
if (VAM_Production_ID <= 0) Set_Value ("VAM_Production_ID", null);
else
Set_Value ("VAM_Production_ID", VAM_Production_ID);
}
/** Get Production.
@return Plan for producing a product */
public int GetVAM_Production_ID() 
{
Object ii = Get_Value("VAM_Production_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Transaction.
@param VAM_Inv_Trx_ID Inventory Transaction */
public void SetVAM_Inv_Trx_ID (int VAM_Inv_Trx_ID)
{
if (VAM_Inv_Trx_ID < 1) throw new ArgumentException ("VAM_Inv_Trx_ID is mandatory.");
Set_Value ("VAM_Inv_Trx_ID", VAM_Inv_Trx_ID);
}
/** Get Inventory Transaction.
@return Inventory Transaction */
public int GetVAM_Inv_Trx_ID() 
{
Object ii = Get_Value("VAM_Inv_Trx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
if (MovementDate == null) throw new ArgumentException ("MovementDate is mandatory.");
Set_Value ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
if (MovementQty == null) throw new ArgumentException ("MovementQty is mandatory.");
Set_Value ("MovementQty", (Decimal?)MovementQty);
}
/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() 
{
Object bd =Get_Value("MovementQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** MovementType VAF_Control_Ref_ID=189 */
public static int MOVEMENTTYPE_VAF_Control_Ref_ID=189;
/** Customer Returns = C+ */
public static String MOVEMENTTYPE_CustomerReturns = "C+";
/** Customer Shipment = C- */
public static String MOVEMENTTYPE_CustomerShipment = "C-";
/** Inventory In = I+ */
public static String MOVEMENTTYPE_InventoryIn = "I+";
/** Inventory Out = I- */
public static String MOVEMENTTYPE_InventoryOut = "I-";
/** Movement To = M+ */
public static String MOVEMENTTYPE_MovementTo = "M+";
/** Movement From = M- */
public static String MOVEMENTTYPE_MovementFrom = "M-";
/** Production + = P+ */
public static String MOVEMENTTYPE_ProductionPlus = "P+";
/** Production - = P- */
public static String MOVEMENTTYPE_Production_ = "P-";
/** Vendor Receipts = V+ */
public static String MOVEMENTTYPE_VendorReceipts = "V+";
/** Vendor Returns = V- */
public static String MOVEMENTTYPE_VendorReturns = "V-";
/** Work Order + = W+ */
public static String MOVEMENTTYPE_WorkOrderPlus = "W+";
/** Work Order - = W- */
public static String MOVEMENTTYPE_WorkOrder_ = "W-";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMovementTypeValid (String test)
{
return test.Equals("C+") || test.Equals("C-") || test.Equals("I+") || test.Equals("I-") || test.Equals("M+") || test.Equals("M-") || test.Equals("P+") || test.Equals("P-") || test.Equals("V+") || test.Equals("V-") || test.Equals("W+") || test.Equals("W-");
}
/** Set Movement Type.
@param MovementType Method of moving the inventory */
public void SetMovementType (String MovementType)
{
if (MovementType == null) throw new ArgumentException ("MovementType is mandatory");
if (!IsMovementTypeValid(MovementType))
throw new ArgumentException ("MovementType Invalid value - " + MovementType + " - Reference_ID=189 - C+ - C- - I+ - I- - M+ - M- - P+ - P- - V+ - V- - W+ - W-");
if (MovementType.Length > 2)
{
log.Warning("Length > 2 - truncated");
MovementType = MovementType.Substring(0,2);
}
Set_Value ("MovementType", MovementType);
}
/** Get Movement Type.
@return Method of moving the inventory */
public String GetMovementType() 
{
return (String)Get_Value("MovementType");
}

/** Search_InOut_ID VAF_Control_Ref_ID=337 */
public static int SEARCH_INOUT_ID_VAF_Control_Ref_ID=337;
/** Set Search Shipment/Receipt.
@param Search_InOut_ID Material Shipment Document */
public void SetSearch_InOut_ID (int Search_InOut_ID)
{
if (Search_InOut_ID <= 0) Set_Value ("Search_InOut_ID", null);
else
Set_Value ("Search_InOut_ID", Search_InOut_ID);
}
/** Get Search Shipment/Receipt.
@return Material Shipment Document */
public int GetSearch_InOut_ID() 
{
Object ii = Get_Value("Search_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Search_Invoice_ID VAF_Control_Ref_ID=336 */
public static int SEARCH_INVOICE_ID_VAF_Control_Ref_ID=336;
/** Set Search Invoice.
@param Search_Invoice_ID Search Invoice Identifier */
public void SetSearch_Invoice_ID (int Search_Invoice_ID)
{
if (Search_Invoice_ID <= 0) Set_Value ("Search_Invoice_ID", null);
else
Set_Value ("Search_Invoice_ID", Search_Invoice_ID);
}
/** Get Search Invoice.
@return Search Invoice Identifier */
public int GetSearch_Invoice_ID() 
{
Object ii = Get_Value("Search_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Search_Order_ID VAF_Control_Ref_ID=290 */
public static int SEARCH_ORDER_ID_VAF_Control_Ref_ID=290;
/** Set Search Order.
@param Search_Order_ID Order Identifier */
public void SetSearch_Order_ID (int Search_Order_ID)
{
if (Search_Order_ID <= 0) Set_Value ("Search_Order_ID", null);
else
Set_Value ("Search_Order_ID", Search_Order_ID);
}
/** Get Search Order.
@return Order Identifier */
public int GetSearch_Order_ID() 
{
Object ii = Get_Value("Search_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}