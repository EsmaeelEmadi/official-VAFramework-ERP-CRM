﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportConversionRate
 * Purpose        : Import Currency Conversion Rates
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportConversionRate : ProcessEngine.SvrProcess
    {

        /**	Client to be imported to			*/
        private int _VAF_Client_ID = 0;
        /**	Organization to be imported to		*/
        private int _VAF_Org_ID = 0;
        /**	Conversion Type to be imported to	*/
        private int _VAB_CurrencyType_ID = 0;
        /**	Default Date					*/
        private DateTime? _ValidFrom = null;
        /** Default Reciprocal				*/
        private bool _CreateReciprocalRate = false;
        /**	Delete old Imported				*/
        private bool _DeleteOldImported = false;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAF_Client_ID"))
                    _VAF_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAF_Org_ID"))
                    _VAF_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAB_CurrencyType_ID"))
                    _VAB_CurrencyType_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("ValidFrom"))
                    _ValidFrom = (DateTime?)para[i].GetParameter();
                else if (name.Equals("CreateReciprocalRate"))
                    _CreateReciprocalRate = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOldImported"))
                    _DeleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("doIt - VAF_Client_ID=" + _VAF_Client_ID
                + ",VAF_Org_ID=" + _VAF_Org_ID
                + ",VAB_CurrencyType_ID=" + _VAB_CurrencyType_ID
                + ",ValidFrom=" + _ValidFrom
                + ",CreateReciprocalRate=" + _CreateReciprocalRate);
            //
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND VAF_Client_ID=" + _VAF_Client_ID;
            //	****	Prepare	****

            //	Delete Old Imported
            if (_DeleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Conversion_Rate "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, Location, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_Conversion_Rate "
                  + "SET VAF_Client_ID = COALESCE (VAF_Client_ID,").Append(_VAF_Client_ID).Append("),"
                  + " VAF_Org_ID = COALESCE (VAF_Org_ID,").Append(_VAF_Org_ID).Append("),");
            if (_VAB_CurrencyType_ID != 0)
                sql.Append(" VAB_CurrencyType_ID = COALESCE (VAB_CurrencyType_ID,").Append(_VAB_CurrencyType_ID).Append("),");
            if (_ValidFrom != null)
                sql.Append(" ValidFrom = COALESCE (ValidFrom,").Append(DataBase.DB.TO_DATE(_ValidFrom)).Append("),");
            else
                sql.Append(" ValidFrom = COALESCE (ValidFrom,SysDate),");
            sql.Append(" CreateReciprocalRate = COALESCE (CreateReciprocalRate,'").Append(_CreateReciprocalRate ? "Y" : "N").Append("'),"
                + " IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = ").Append(GetVAF_UserContact_ID()).Append(","
                + " I_ErrorMsg = NULL,"
                + " Processed = 'N',"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset =" + no);

            //	Org
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Conversion_Rate o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (VAF_Org_ID IS NULL"
                + " OR EXISTS (SELECT * FROM VAF_Org oo WHERE o.VAF_Org_ID=oo.VAF_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org =" + no);

            //	Conversion Type
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET VAB_CurrencyType_ID = (SELECT VAB_CurrencyType_ID FROM VAB_CurrencyType c"
                + " WHERE c.Value=i.ConversionTypeValue AND c.VAF_Client_ID IN (0,i.VAF_Client_ID) AND c.IsActive='Y') "
                + "WHERE VAB_CurrencyType_ID IS NULL AND ConversionTypeValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("Set ConversionType =" + no);
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ConversionType, ' "
                + "WHERE (VAB_CurrencyType_ID IS NULL"
                + " OR NOT EXISTS (SELECT * FROM VAB_CurrencyType c "
                    + "WHERE i.VAB_CurrencyType_ID=c.VAB_CurrencyType_ID AND c.IsActive='Y'"
                    + " AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ConversionType =" + no);

            //	Currency
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET VAB_Currency_ID = (SELECT VAB_Currency_ID FROM VAB_Currency c"
                + "	WHERE c.ISO_Code=i.ISO_Code AND c.VAF_Client_ID IN (0,i.VAF_Client_ID) AND c.IsActive='Y') "
                + "WHERE VAB_Currency_ID IS NULL AND ISO_Code IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("Set Currency =" + no);
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Currency, ' "
                + "WHERE (VAB_Currency_ID IS NULL"
                + " OR NOT EXISTS (SELECT * FROM VAB_Currency c "
                    + "WHERE i.VAB_Currency_ID=c.VAB_Currency_ID AND c.IsActive='Y'"
                    + " AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency =" + no);

            //	Currency To
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET VAB_Currency_To_ID = (SELECT VAB_Currency_ID FROM VAB_Currency c"
                + "	WHERE c.ISO_Code=i.ISO_Code_To AND c.VAF_Client_ID IN (0,i.VAF_Client_ID) AND c.IsActive='Y') "
                + "WHERE VAB_Currency_To_ID IS NULL AND ISO_Code_To IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("Set Currency To =" + no);
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Currency To, ' "
                + "WHERE (VAB_Currency_To_ID IS NULL"
                + " OR NOT EXISTS (SELECT * FROM VAB_Currency c "
                    + "WHERE i.VAB_Currency_To_ID=c.VAB_Currency_ID AND c.IsActive='Y'"
                    + " AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency To =" + no);

            //	Rates
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET MultiplyRate = 1 / DivideRate "
                + "WHERE (MultiplyRate IS NULL OR MultiplyRate = 0) AND DivideRate IS NOT NULL AND DivideRate<>0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("Set MultiplyRate =" + no);
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET DivideRate = 1 / MultiplyRate "
                + "WHERE (DivideRate IS NULL OR DivideRate = 0) AND MultiplyRate IS NOT NULL AND MultiplyRate<>0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("Set DivideRate =" + no);
            sql = new StringBuilder("UPDATE I_Conversion_Rate i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Rates, ' "
                + "WHERE (MultiplyRate IS NULL OR MultiplyRate = 0 OR DivideRate IS NULL OR DivideRate = 0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Rates =" + no);
            //	sql = new StringBuilder ("UPDATE I_Conversion_Rate i "	//	Rate diff > 10%
            //		+ "SET I_IsImported='E', I_ErrorMsg="+ts +"||'ERR=Inconsistent Rates='||(MultiplyRate - (1/DivideRate)) "
            //		+ "WHERE ((MultiplyRate - (1/DivideRate)) > (MultiplyRate * .1))"
            //		+ " AND I_IsImported<>'Y'").Append (clientCheck);
            //	no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
            //	if (no != 0)
            //		log.warn ("Inconsistent Rates =" + no);

            Commit();
            /*********************************************************************/

            int noInsert = 0;
            sql = new StringBuilder("SELECT * FROM I_Conversion_Rate "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY VAB_Currency_ID, VAB_Currency_To_ID, ValidFrom");
            //PreparedStatement pstmt = null;
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_VAI_CurrencyExchangeRate imp = new X_VAI_CurrencyExchangeRate(GetCtx(), idr, Get_TrxName());
                    MVABExchangeRate rate = new MVABExchangeRate(imp,
                        imp.GetVAB_CurrencyType_ID(),
                        imp.GetVAB_Currency_ID(), imp.GetVAB_Currency_To_ID(),
                        imp.GetMultiplyRate(), imp.GetValidFrom());
                    if (imp.GetValidTo() != null)
                        rate.SetValidTo(imp.GetValidTo());
                    if (rate.Save())
                    {
                        imp.SetVAB_ExchangeRate_ID(rate.GetVAB_ExchangeRate_ID());
                        imp.SetI_IsImported(X_VAI_CurrencyExchangeRate.I_ISIMPORTED_Yes);
                        imp.SetProcessed(true);
                        imp.Save();
                        noInsert++;
                        //
                        if (imp.IsCreateReciprocalRate())
                        {
                            rate = new MVABExchangeRate(imp,
                                imp.GetVAB_CurrencyType_ID(),
                                imp.GetVAB_Currency_To_ID(), imp.GetVAB_Currency_ID(),
                                imp.GetDivideRate(), imp.GetValidFrom());
                            if (imp.GetValidTo() != null)
                                rate.SetValidTo(imp.GetValidTo());
                            if (rate.Save())
                                noInsert++;
                        }
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Conversion_Rate "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@VAB_ExchangeRate_ID@: @Inserted@");
            return "";
        }	//	doIt

    }	
}
