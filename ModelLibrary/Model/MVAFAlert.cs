﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFAlert : X_VAF_Alert
    {
        public MVAFAlert(Ctx ctx, int VAF_Alert_ID, Trx trx)
            : base(ctx, VAF_Alert_ID, trx)
        {
            if (VAF_Alert_ID == 0)
            {
                //	setVAF_AlertHandler_ID (0);
                //	setName (null);
                //	setAlertMessage (null);
                //	setAlertSubject (null);
                SetEnforceClientSecurity(true);	// Y
                SetEnforceRoleSecurity(true);	// Y
                SetIsValid(true);	// Y
            }
        }	//	MAlert

        public MVAFAlert(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MAlert

        /**	The Rules						*/
        private MVAFAlertSetting[] m_rules = null;
        /**	The Recipients					*/
        private MVAFAlertRecipient[] m_recipients = null;


        public MVAFAlertSetting[] GetRules(bool reload)
        {
            if (m_rules != null && !reload)
                return m_rules;
            String sql = "SELECT * FROM VAF_AlertSetting "
                + "WHERE isactive='Y' AND VAF_Alert_ID=" + GetVAF_Alert_ID();
            List<MVAFAlertSetting> list = new List<MVAFAlertSetting>();

            DataSet ds = DB.ExecuteDataset(sql);
            try
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MVAFAlertSetting MVAFAlertSetting = new MVAFAlertSetting(GetCtx(), dr, null);
                    ValidateAlertRuleCondition(MVAFAlertSetting);
                    list.Add(MVAFAlertSetting);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            m_rules = new MVAFAlertSetting[list.Count()];
            m_rules = list.ToArray();
            return m_rules;
        }	//	getRules


        public MVAFAlertRecipient[] GetRecipients(bool reload)
        {
            if (m_recipients != null && !reload)
                return m_recipients;
            String sql = "SELECT * FROM VAF_AlertRecipient "
                + "WHERE VAF_Alert_ID=" + GetVAF_Alert_ID();
            List<MVAFAlertRecipient> list = new List<MVAFAlertRecipient>();
            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MVAFAlertRecipient(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_recipients = new MVAFAlertRecipient[list.Count()];
            m_recipients = list.ToArray();
            return m_recipients;
        }	//	getRecipients


        public int GetFirstVAF_Role_ID()
        {
            GetRecipients(false);
            foreach (MVAFAlertRecipient element in m_recipients)
            {
                if (element.GetVAF_Role_ID() != -1)
                    return element.GetVAF_Role_ID();
            }
            return -1;
        }	//	getForstVAF_Role_ID


        public int GetFirstUserVAF_Role_ID()
        {
            GetRecipients(false);
            int VAF_UserContact_ID = GetFirstVAF_UserContact_ID();
            if (VAF_UserContact_ID != -1)
            {
                MVAFUserContactRoles[] urs = MVAFUserContactRoles.GetOfUser(GetCtx(), VAF_UserContact_ID);
                foreach (MVAFUserContactRoles element in urs)
                {
                    if (element.IsActive())
                        return element.GetVAF_Role_ID();
                }
            }
            return -1;
        }	//	getFirstUserVAF_Role_ID


        public int GetFirstVAF_UserContact_ID()
        {
            GetRecipients(false);
            foreach (MVAFAlertRecipient element in m_recipients)
            {
                if (element.GetVAF_UserContact_ID() != -1)
                    return element.GetVAF_UserContact_ID();
            }
            return -1;
        }	//	getFirstVAF_UserContact_ID


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlert[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid());
            if (m_rules != null)
                sb.Append(",Rules=").Append(m_rules.Length);
            if (m_recipients != null)
                sb.Append(",Recipients=").Append(m_recipients.Length);
            sb.Append("]");
            return sb.ToString();
        }
        /**
        * 	Alert Condition 
        *	@param alert alert
        *	@return true if processed
        */
        public bool ValidateAlertRuleCondition(MVAFAlertSetting AlertRule)
        {
            bool returnConditionValue = true;
            int errorType = 0;
            string Sql = "SELECT object_name FROM all_objects WHERE object_type IN ('TABLE','VIEW') AND (object_name)  = UPPER('VAF_AlertSettingCondition') AND OWNER LIKE '" + DB.GetSchema() + "'";
            string ObjectName = Convert.ToString(DB.ExecuteScalar(Sql));
            if (ObjectName != "")
            {
                //Fetch All Alert Condition Against AlertID.............
                DataSet dsAlertCondition = DB.ExecuteDataset("select VAF_AlertSettingCondition_id from VAF_AlertSettingCondition where VAF_AlertSetting_ID=" + AlertRule.GetVAF_AlertSetting_ID() + " and isactive='Y' order by sequence,VAF_AlertSettingCondition_id");
                //IF No Alert Condition Find then return true otherwise Follow further Condition............
                if (dsAlertCondition != null && dsAlertCondition.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsAlertCondition.Tables[0].Rows.Count; i++)
                    {
                        decimal numericValue = 0;
                        string stringValue = "";
                        DateTime dateValue = new DateTime();
                        bool validateResult = false;
                        errorType = 0;
                        int alertConditionID = Convert.ToInt32(dsAlertCondition.Tables[0].Rows[i]["VAF_AlertSettingCondition_id"]);

                        X_VAF_AlertSettingCondition alertCondition = new X_VAF_AlertSettingCondition(AlertRule.GetCtx(), alertConditionID, null);
                        string sqlQuery = alertCondition.GetSqlQuery();

                        try
                        {
                            if (alertCondition.GetSqlQuery().ToLower().Trim().StartsWith("select"))
                            {
                                //Check What is the return type of Query. if Query retrun more than one record than throw error............
                                if (alertCondition.GetReturnValueType() == X_VAF_AlertSettingCondition.RETURNVALUETYPE_Number) //Numeric Value
                                {
                                    errorType = 1;//if error occured in following query than used in catch 
                                    if (DB.ExecuteScalar(sqlQuery) == DBNull.Value || DB.ExecuteScalar(sqlQuery) == null)
                                    {
                                        numericValue =Convert.ToDecimal(0);
                                    }
                                    else
                                    {
                                        numericValue = Convert.ToDecimal(DB.ExecuteScalar(sqlQuery));
                                    }
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateNumaricLogic(numericValue, Convert.ToDecimal(alertCondition.GetAlphaNumValue()), alertCondition.GetOperator());
                                }
                                else if (alertCondition.GetReturnValueType() == X_VAF_AlertSettingCondition.RETURNVALUETYPE_String)//String Value
                                {
                                    errorType = 1;//if error occured in following query than used in catch 
                                    if (DB.ExecuteScalar(sqlQuery) == DBNull.Value || DB.ExecuteScalar(sqlQuery) == null)
                                    {
                                        stringValue = "";
                                    }
                                    else
                                    {
                                        stringValue = Convert.ToString(DB.ExecuteScalar(sqlQuery));
                                    }
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateStringLogic(stringValue, alertCondition.GetAlphaNumValue(), alertCondition.GetOperator());
                                }
                                else if (alertCondition.GetReturnValueType() == X_VAF_AlertSettingCondition.RETURNVALUETYPE_Date)// Date Value
                                {
                                    // this Date Section is not implemented in Alert Return Value Type List.......................

                                    errorType = 1;//if error occured in following query than used in catch 
                                    dateValue = Convert.ToDateTime(DB.ExecuteScalar(sqlQuery));
                                    errorType = 2;//if error occured in following comparison then used in catch
                                    //This function Match condition on Query Return Value and User's enterd Value based on users Selected Operator...........
                                    validateResult = EvaluateDateLogic(dateValue, Convert.ToDateTime(alertCondition.GetDateValue()), alertCondition.GetOperator(), alertCondition.IsDynamic(), alertCondition.GetDateOperation(), alertCondition.GetDay(), alertCondition.GetMONTH(), alertCondition.GetYEAR());
                                }
                                //if we Find multiple condition against same alert then we have to find on the basis of And OR
                                if (i != 0)
                                {
                                    if (X_VAF_AlertSettingCondition.ANDOR_Or.Equals(alertCondition.GetAndOr()))
                                    {
                                        returnConditionValue = returnConditionValue || validateResult;
                                    }
                                    else
                                    {
                                        returnConditionValue = returnConditionValue && validateResult;
                                    }
                                }
                                else
                                {
                                    returnConditionValue = validateResult;
                                }
                            }
                            else
                            {
                                returnConditionValue = false;
                                AlertRule.SetErrorMsg("Conditional Sequence Number " +alertCondition.GetSequence()+ " Error= Only Execute Select Query");
                                AlertRule.SetIsValid(false);
                                AlertRule.Save();
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            returnConditionValue = false;
                            if (errorType == 1)
                            {
                                AlertRule.SetErrorMsg("Conditional Sequence Number " + alertCondition.GetSequence() + " Select Error=" + e.Message);
                               
                            }
                            else
                            {
                                AlertRule.SetErrorMsg("Conditional Sequence Number " + alertCondition.GetSequence() + " Comparison Error=" + e.Message);
                                
                            }
                            AlertRule.SetIsValid(false);
                            AlertRule.Save();
                            return false;
                        }
                    }
                }
                else
                {
                    returnConditionValue = true;
                }
            }
            else
            {
                returnConditionValue = true;
            }
            if (AlertRule.GetErrorMsg() == null || AlertRule.GetErrorMsg() == string.Empty)
            {
                AlertRule.SetIsValid(returnConditionValue);
                AlertRule.Save();
            }
            return returnConditionValue;
        }
        private bool EvaluateNumaricLogic(decimal numericValue, decimal compareValue, string operation)
        {
            if (X_VAF_AlertSettingCondition.OPERATOR_Eq.Equals(operation))
                return numericValue.CompareTo(compareValue) == 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_Gt.Equals(operation))
                return numericValue.CompareTo(compareValue) > 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_GtEq.Equals(operation))
                return numericValue.CompareTo(compareValue) >= 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_Le.Equals(operation))
                return numericValue.CompareTo(compareValue) < 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_LeEq.Equals(operation))
                return numericValue.CompareTo(compareValue) <= 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_NotEq.Equals(operation))
                return numericValue.CompareTo(compareValue) != 0;
            else if (X_VAF_AlertSettingCondition.OPERATOR_Like.Equals(operation))
                return numericValue.CompareTo(compareValue) == 0;
            else
                return false;
        }

        private bool EvaluateStringLogic(string Value, string compareValue, string operation)
        {
            if (X_VAF_AlertSettingCondition.OPERATOR_Eq.Equals(operation))
            {
                return Value.Equals(compareValue, StringComparison.OrdinalIgnoreCase);
            }
            //else if (X_VAF_AlertCondition.OPERATOR_Like.Equals(operation))
            //{

            //    return Value.ToLower().Contains(compareValue.ToLower());
            //}
            else if (X_VAF_AlertSettingCondition.OPERATOR_NotEq.Equals(operation))
            {
                return Value.ToLower() != compareValue.ToLower();
            }
            else
            {
                return false;
            }
        }
        // This Date Comparison function not properly implemented for Alert...........
        #region
        private bool EvaluateDateLogic(DateTime value, DateTime compareValue, string operation, bool isDynamic, string dynamicValue, int day, int month, int year)
        {
            value = value.Date;
            compareValue = compareValue.Date;
            if (X_VAF_AlertSettingCondition.OPERATOR_Eq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) == 0;
            }
            else if (X_VAF_AlertSettingCondition.OPERATOR_Gt.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) > 0;
            }
            else if (X_VAF_AlertSettingCondition.OPERATOR_GtEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) >= 0;
            }
            else if (X_VAF_AlertSettingCondition.OPERATOR_Le.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) < 0;
            }
            else if (X_VAF_AlertSettingCondition.OPERATOR_LeEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) <= 0;
            }
            else if (X_VAF_AlertSettingCondition.OPERATOR_NotEq.Equals(operation))
            {
                if (isDynamic == true)
                {
                    compareValue = DynamicDateLogic(dynamicValue, day, month, year);
                }
                return value.CompareTo(compareValue) != 0;
            }
            else
                return false;
        }
        private DateTime DynamicDateLogic(string DynamicValue, int day, int month, int year)
        {

            if (X_VAF_AlertSettingCondition.DATEOPERATION_Today.Equals(DynamicValue))
            {
                return System.DateTime.Now.Date;
            }
            else if (X_VAF_AlertSettingCondition.DATEOPERATION_Now.Equals(DynamicValue))
            {
                return System.DateTime.Now;
            }
            else if (X_VAF_AlertSettingCondition.DATEOPERATION_LastxDays.Equals(DynamicValue))
            {
                return System.DateTime.Now.Date.AddDays(-day);
            }
            else if (X_VAF_AlertSettingCondition.DATEOPERATION_LastxMonth.Equals(DynamicValue))
            {
                int tempDay = (month * 31) + day;
                return System.DateTime.Now.Date.AddDays(-tempDay);
            }
            else if (X_VAF_AlertSettingCondition.DATEOPERATION_LastxYear.Equals(DynamicValue))
            {
                int tempDay = (year * 365) + (month * 31) + day;
                return System.DateTime.Now.Date.AddDays(-tempDay);
            }
            else
            {
                return System.DateTime.Now.Date;
            }

        }
        #endregion
    }
}