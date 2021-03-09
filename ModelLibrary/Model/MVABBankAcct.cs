﻿/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_Bank_Acct
 * Chronological Development
 * Veena Pandey     24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;

namespace VAdvantage.Model
{
    public class MVABBankAcct : X_VAB_Bank_Acct
    {
        /**	Cache						*/
        private static CCache<int, MVABBankAcct> _cache
            = new CCache<int, MVABBankAcct>("VAB_Bank_Acct", 5);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Bank_Acct_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABBankAcct(Ctx ctx, int VAB_Bank_Acct_ID, Trx trxName)
            : base(ctx, VAB_Bank_Acct_ID, trxName)
        {
            if (VAB_Bank_Acct_ID == 0)
            {
                SetIsDefault(false);
                SetBankAccountType(BANKACCOUNTTYPE_Checking);
                SetCurrentBalance(Env.ZERO);
                SetUnMatchedBalance(Env.ZERO);
                //	SetVAB_Currency_ID (0);
                SetCreditLimit(Env.ZERO);
                //	SetVAB_Bank_Acct_ID (0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABBankAcct(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get BankAccount from Cache
	     *	@param ctx context
	     *	@param VAB_Bank_Acct_ID id
	     *	@return MBankAccount
	     */
        public static MVABBankAcct Get(Ctx ctx, int VAB_Bank_Acct_ID)
        {
            int key = VAB_Bank_Acct_ID;
            MVABBankAcct retValue = (MVABBankAcct)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVABBankAcct(ctx, VAB_Bank_Acct_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
	     * 	Get Bank
	     *	@return bank parent
	     */
        public MVABBank GetBank()
        {
            return MVABBank.Get(GetCtx(), GetVAB_Bank_ID());
        }

        /**
         * 	Get Bank Name and Account No
         *	@return Bank/Account
         */
        public String GetName()
        {
            return GetBank().GetName() + " " + GetAccountNo();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if valid
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            MVABBank bank = GetBank();
            BankVerificationInterface verify = bank.GetVerificationClass();
            if (verify != null)
            {
                String errorMsg = verify.VerifyAccountNo(bank, GetAccountNo());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @AccountNo@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyBBAN(bank, GetBBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @BBAN@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyIBAN(bank, GetIBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @IBAN@ " + errorMsg);
                    return false;
                }
            }

            //Issue ID- SI_0613 in Google Sheet Standard Issues.. On Bank account currency should not allow to change if any transcation made against bank.
            if (!newRecord && Is_ValueChanged("VAB_Currency_ID"))
            {
                string sql = "SELECT SUM(total) AS TOTAL FROM (SELECT COUNT(*) AS TOTAL FROM VAB_PAYMENT WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL SELECT COUNT(*) AS TOTAL FROM VAB_ACCT_ELEMENT WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM VAB_BPart_Bank_Acct WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM VAB_BANK_ACCT_ACCT  WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM VAB_BANKINGJRNL WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM VAB_PAYMENTHANDLER  WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM VAB_CASHJRNLLINE WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM VAB_PaymentOption WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM VAB_BANK_ACCTDOC WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID()
                               + " UNION ALL  SELECT COUNT(*) AS Total FROM VAB_Bank_AcctLine WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID();

                if (Env.IsModuleInstalled("VA026_")) // if Letter Of Credit Module is Installed..
                    sql += " UNION ALL SELECT count(*) as Total FROM VA026_LCDETAIL WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID();
                if (Env.IsModuleInstalled("VA027_")) // If Post dated Check Module is Installed..
                    sql += " UNION ALL SELECT count(*) as total FROM VA027_POSTDATEDCHECK WHERE VAF_Client_ID= " + GetVAF_Client_ID() + " AND VAB_BANK_ACCT_ID =" + GetVAB_Bank_Acct_ID();

                sql += ")";

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CouldNotChnageCurrency"));
                    return false;
                }

                // JID_1583: system will update the Opening balance value in Current Balance field
                if (newRecord && Get_ColumnIndex("OpenBalance") > 0)
                {
                    SetCurrentBalance(Util.GetValueOfDecimal(Get_Value("OpenBalance")));
                }
            }
            //End

            // JID_1583: system will update the Opening balance value in Current Balance field
            if (newRecord && Get_ColumnIndex("OpenBalance") > 0)
            {
                SetCurrentBalance(Util.GetValueOfDecimal(Get_Value("OpenBalance")));
            }
            return true;
        }

        /**
        * 	After Save
        *	@param newRecord new record
        *	@param success success
        *	@return success
        */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (newRecord & success)
            {
                int _client_ID = 0;
                StringBuilder _sql = new StringBuilder();
                //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_BankAccount_Acct'");
                //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BankAccount_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
                _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BankAccount_Acct"));
                int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (count > 0)
                {
                    string sql = " SELECT L.VALUE FROM VAF_CTRLREF_LIST L" +
                                                     " INNER JOIN VAF_Control_Ref r ON R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID" +
                                                     " WHERE r.name='FRPT_RelatedTo' AND l.name='BankAccount'";

                    var relatedto = Convert.ToString(DB.ExecuteScalar(sql));

                    PO bnkact = null;
                    //X_FRPT_BankAccount_Acct bnkact = null;
                    //MAcctSchema acschma = new MAcctSchema(GetCtx(), 0, null);
                    _client_ID = GetVAF_Client_ID();
                    _sql.Clear();
                    _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where VAF_CLIENT_ID=" + _client_ID);
                    DataSet ds3 = new DataSet();
                    ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                    if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                    {
                        for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                        {
                            int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                            _sql.Clear();
                            _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + _client_ID + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
                            DataSet ds = new DataSet();
                            ds = DB.ExecuteDataset(_sql.ToString(), null);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    //DataSet ds2 = new DataSet();
                                    string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                    if (_relatedTo != "")
                                    {

                                        // if (_relatedTo == X_FRPT_AcctSchema_Default.FRPT_RELATEDTO_BankAccount.ToString())
                                        if (_relatedTo == relatedto)
                                        {
                                            _sql.Clear();
                                            _sql.Append("Select COUNT(*) From VAB_Bank_Acct Ba Left Join FRPT_BankAccount_Acct ca On Ba.VAB_BANK_ACCT_ID=ca.VAB_BANK_ACCT_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Ba.IsActive='Y' AND Ba.VAF_Client_ID=" + _client_ID + " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) + " AND Ba.VAB_Bank_Acct_ID = " + GetVAB_Bank_Acct_ID());
                                            int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                            //ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                            //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                            //{
                                            //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                            //    {
                                            //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                            //        if (value == 0)
                                            //        {
                                            //bnkact = new X_FRPT_BankAccount_Acct(GetCtx(), 0, null);
                                            if (recordFound == 0)
                                            {
                                                bnkact = MVAFTableView.GetPO(GetCtx(), "FRPT_BankAccount_Acct", 0, null);
                                                bnkact.Set_ValueNoCheck("VAB_Bank_Acct_ID", Util.GetValueOfInt(GetVAB_Bank_Acct_ID()));
                                                bnkact.Set_ValueNoCheck("VAF_Org_ID", 0);
                                                bnkact.Set_Value("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                                bnkact.Set_Value("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                                bnkact.Set_Value("VAB_AccountBook_ID", _AcctSchema_ID);
                                                if (!bnkact.Save())
                                                { }
                                            }
                                            //        }
                                            //    }
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y")
                {
                    bool sucs = Insert_Accounting("VAB_Bank_Acct_Acct", "VAB_AccountBook_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }

                }
            }
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("VAB_Bank_Acct_Acct");
        }

        /**
	     * 	String representation
	     *	@return info
	     */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBankAccount[")
                .Append(Get_ID())
                .Append("-").Append(GetAccountNo())
                .Append("]");
            return sb.ToString();
        }

    }
}