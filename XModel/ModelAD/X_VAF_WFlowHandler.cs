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
/** Generated Model for VAF_WFlowHandler
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_WFlowHandler : PO
{
public X_VAF_WFlowHandler (Context ctx, int VAF_WFlowHandler_ID, Trx trxName) : base (ctx, VAF_WFlowHandler_ID, trxName)
{
/** if (VAF_WFlowHandler_ID == 0)
{
SetVAF_WFlowHandler_ID (0);
SetKeepLogDays (0);	// 7
SetName (null);
SetSupervisor_ID (0);
}
 */
}
public X_VAF_WFlowHandler (Ctx ctx, int VAF_WFlowHandler_ID, Trx trxName) : base (ctx, VAF_WFlowHandler_ID, trxName)
{
/** if (VAF_WFlowHandler_ID == 0)
{
SetVAF_WFlowHandler_ID (0);
SetKeepLogDays (0);	// 7
SetName (null);
SetSupervisor_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandler (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandler (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_WFlowHandler (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_WFlowHandler()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366874L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050085L;
/** VAF_TableView_ID=697 */
public static int Table_ID;
 // =697;

/** TableName=VAF_WFlowHandler */
public static String Table_Name="VAF_WFlowHandler";

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
StringBuilder sb = new StringBuilder ("X_VAF_WFlowHandler[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Schedule.
@param VAF_Plan_ID Execution Schedule */
public void SetVAF_Plan_ID (int VAF_Plan_ID)
{
if (VAF_Plan_ID <= 0) Set_Value ("VAF_Plan_ID", null);
else
Set_Value ("VAF_Plan_ID", VAF_Plan_ID);
}
/** Get Schedule.
@return Execution Schedule */
public int GetVAF_Plan_ID() 
{
Object ii = Get_Value("VAF_Plan_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Workflow Processor.
@param VAF_WFlowHandler_ID Workflow Processor Server */
public void SetVAF_WFlowHandler_ID (int VAF_WFlowHandler_ID)
{
if (VAF_WFlowHandler_ID < 1) throw new ArgumentException ("VAF_WFlowHandler_ID is mandatory.");
Set_ValueNoCheck ("VAF_WFlowHandler_ID", VAF_WFlowHandler_ID);
}
/** Get Workflow Processor.
@return Workflow Processor Server */
public int GetVAF_WFlowHandler_ID() 
{
Object ii = Get_Value("VAF_WFlowHandler_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alert over Priority.
@param AlertOverPriority Send alert email when over priority */
public void SetAlertOverPriority (int AlertOverPriority)
{
Set_Value ("AlertOverPriority", AlertOverPriority);
}
/** Get Alert over Priority.
@return Send alert email when over priority */
public int GetAlertOverPriority() 
{
Object ii = Get_Value("AlertOverPriority");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_Value ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
}
/** Set Date next run.
@param DateNextRun Date the process will run next */
public void SetDateNextRun (DateTime? DateNextRun)
{
Set_Value ("DateNextRun", (DateTime?)DateNextRun);
}
/** Get Date next run.
@return Date the process will run next */
public DateTime? GetDateNextRun() 
{
return (DateTime?)Get_Value("DateNextRun");
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
/** Set Frequency.
@param Frequency Frequency of events */
public void SetFrequency (int Frequency)
{
Set_Value ("Frequency", Frequency);
}
/** Get Frequency.
@return Frequency of events */
public int GetFrequency() 
{
Object ii = Get_Value("Frequency");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** FrequencyType VAF_Control_Ref_ID=221 */
public static int FREQUENCYTYPE_VAF_Control_Ref_ID=221;
/** Day = D */
public static String FREQUENCYTYPE_Day = "D";
/** Hour = H */
public static String FREQUENCYTYPE_Hour = "H";
/** Minute = M */
public static String FREQUENCYTYPE_Minute = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFrequencyTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("H") || test.Equals("M");
}
/** Set Frequency Type.
@param FrequencyType Frequency of event */
public void SetFrequencyType (String FrequencyType)
{
if (!IsFrequencyTypeValid(FrequencyType))
throw new ArgumentException ("FrequencyType Invalid value - " + FrequencyType + " - Reference_ID=221 - D - H - M");
if (FrequencyType != null && FrequencyType.Length > 1)
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
/** Set Inactivity Alert Days.
@param InactivityAlertDays Send Alert when there is no activity after days (0= no alert) */
public void SetInactivityAlertDays (int InactivityAlertDays)
{
Set_Value ("InactivityAlertDays", InactivityAlertDays);
}
/** Get Inactivity Alert Days.
@return Send Alert when there is no activity after days (0= no alert) */
public int GetInactivityAlertDays() 
{
Object ii = Get_Value("InactivityAlertDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Days to keep Log.
@param KeepLogDays Number of days to keep the log entries */
public void SetKeepLogDays (int KeepLogDays)
{
Set_Value ("KeepLogDays", KeepLogDays);
}
/** Get Days to keep Log.
@return Number of days to keep the log entries */
public int GetKeepLogDays() 
{
Object ii = Get_Value("KeepLogDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Reminder Days.
@param RemindDays Days between sending Reminder Emails for a due or inactive Document */
public void SetRemindDays (int RemindDays)
{
Set_Value ("RemindDays", RemindDays);
}
/** Get Reminder Days.
@return Days between sending Reminder Emails for a due or inactive Document */
public int GetRemindDays() 
{
Object ii = Get_Value("RemindDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Supervisor_ID VAF_Control_Ref_ID=316 */
public static int SUPERVISOR_ID_VAF_Control_Ref_ID=316;
/** Set Supervisor.
@param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
public void SetSupervisor_ID (int Supervisor_ID)
{
if (Supervisor_ID < 1) throw new ArgumentException ("Supervisor_ID is mandatory.");
Set_Value ("Supervisor_ID", Supervisor_ID);
}
/** Get Supervisor.
@return Supervisor for this user/organization - used for escalation and approval */
public int GetSupervisor_ID() 
{
Object ii = Get_Value("Supervisor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}