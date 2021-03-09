﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAPAEvaluateCalc : X_VAPA_EvaluateCalc
    {
        /**
         * 	Get MVAPAEvaluateCalc from Cache
         *	@param ctx Ctx
         *	@param VAPA_EvaluateCalc_ID id
         *	@return MVAPAEvaluateCalc
         */
        public static MVAPAEvaluateCalc Get(Ctx ctx, int VAPA_EvaluateCalc_ID)
        {
            int key = VAPA_EvaluateCalc_ID;
            MVAPAEvaluateCalc retValue = (MVAPAEvaluateCalc)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAPAEvaluateCalc(ctx, VAPA_EvaluateCalc_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MVAPAEvaluateCalc> _cache
            = new CCache<int, MVAPAEvaluateCalc>("VAPA_EvaluateCalc", 10);

        /**
         * 	Standard Constructor
         *	@param ctx Ctx
         *	@param VAPA_EvaluateCalc_ID id
         *	@param trxName trx
         */
        public MVAPAEvaluateCalc(Ctx ctx, int VAPA_EvaluateCalc_ID, Trx trxName) :
            base(ctx, VAPA_EvaluateCalc_ID, trxName)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx Ctx
         *	@param rs result set
         *	@param trxName trx
         */
        public MVAPAEvaluateCalc(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }


        /**
         * 	Get Sql to return single value for the Performance Indicator
         *	@param restrictions array of goal restrictions
         *	@param MeasureScope scope of this value  
         *	@param MeasureDataType data type
         *	@param reportDate optional report date
         *	@param role role
         *	@return sql for performance indicator
         */
        public String GetSqlPI(MVAPATargetRestriction[] restrictions,
            String MeasureScope, String MeasureDataType, DateTime? reportDate, MVAFRole role)
        {
            StringBuilder sb = new StringBuilder(GetSelectClause())
                .Append(" ")
                .Append(GetWhereClause());
            //	Date Restriction
            if (GetDateColumn() != null
                && MVAPAEvaluate.MEASUREDATATYPE_QtyAmountInTime.Equals(MeasureDataType)
                && !MVAPATarget.MEASUREDISPLAY_Total.Equals(MeasureScope))
            {
                if (reportDate == null)
                    reportDate = Convert.ToDateTime(DateTime.Now);
                String dateString = DataBase.DB.TO_DATE((DateTime?)reportDate);
                // http://download-west.oracle.com/docs/cd/B14117_01/server.101/b10759/functions207.htm#i1002084
                String trunc = "DD";
                if (MVAPATarget.MEASUREDISPLAY_Year.Equals(MeasureScope))
                    trunc = "Y";
                else if (MVAPATarget.MEASUREDISPLAY_Quarter.Equals(MeasureScope))
                    trunc = "Q";
                else if (MVAPATarget.MEASUREDISPLAY_Month.Equals(MeasureScope))
                    trunc = "MM";
                else if (MVAPATarget.MEASUREDISPLAY_Week.Equals(MeasureScope))
                    trunc = "D";
                //	else if (MVAPATarget.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		;
                sb.Append(" AND TRUNC(")
                    .Append(GetDateColumn()).Append(",'").Append(trunc).Append("')=TRUNC(")
                    .Append(DataBase.DB.TO_DATE((DateTime?)reportDate)).Append(",'").Append(trunc).Append("')");
            }	//	date
            String sql = AddRestrictions(sb.ToString(), restrictions, role);

            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Sql to value for the bar chart
         *	@param restrictions array of goal restrictions
         *	@param MeasureDisplay scope of this value  
         *	@param startDate optional report start date
         *	@param role role
         *	@return sql for Bar Chart
         */
        public String GetSqlBarChart(MVAPATargetRestriction[] restrictions,
            String measureDisplay, DateTime? startDate, MVAFRole role)
        {
            StringBuilder sb = new StringBuilder();
            String dateCol = null;
            String groupBy = null;
            if (GetDateColumn() != null
                && !MVAPATarget.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String trunc = "D";
                if (MVAPATarget.MEASUREDISPLAY_Year.Equals(measureDisplay))
                    trunc = "Y";
                else if (MVAPATarget.MEASUREDISPLAY_Quarter.Equals(measureDisplay))
                    trunc = "Q";
                else if (MVAPATarget.MEASUREDISPLAY_Month.Equals(measureDisplay))
                    trunc = "MM";
                else if (MVAPATarget.MEASUREDISPLAY_Week.Equals(measureDisplay))
                    trunc = "W";
                //	else if (MVAPATarget.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		;
                dateCol = "TRUNC(" + GetDateColumn() + ",'" + trunc + "') ";
                groupBy = dateCol;
            }
            else
                dateCol = "MAX(" + GetDateColumn() + ") ";
            //
            String selectFrom = GetSelectClause();
            int index = selectFrom.IndexOf("FROM ");
            if (index == -1)
                index = selectFrom.ToUpper().IndexOf("FROM ");
            if (index == -1)
                throw new ArgumentException("Cannot find FROM in sql - " + selectFrom);
            sb.Append(selectFrom.Substring(0, index))
                .Append(",").Append(dateCol)
                .Append(selectFrom.Substring(index));

            //	** WHERE
            sb.Append(" ")
                .Append(GetWhereClause());
            //	Date Restriction
            if (GetDateColumn() != null
                && startDate != null
                && !MVAPATarget.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String dateString = DataBase.DB.TO_DATE((DateTime?)startDate);
                sb.Append(" AND ").Append(GetDateColumn())
                    .Append(">=").Append(dateString);
            }	//	date
            String sql = AddRestrictions(sb.ToString(), restrictions, role);
            if (groupBy != null)
                sql += " GROUP BY " + groupBy;
            //
            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Zoom Query
         * 	@param restrictions restrictions
         * 	@param MeasureDisplay display
         * 	@param date date
         * 	@param role role
         *	@return query
         */
        public Query GetQuery(MVAPATargetRestriction[] restrictions,
            String measureDisplay, DateTime? date, MVAFRole role)
        {
            Query query = new Query(GetVAF_TableView_ID().ToString());
            //
            StringBuilder sql = new StringBuilder("SELECT ").Append(GetKeyColumn()).Append(" ");
            String from = GetSelectClause();
            int index = from.IndexOf("FROM ");
            if (index == -1)
                throw new ArgumentException("Cannot find FROM " + from);
            sql.Append(from.Substring(index)).Append(" ")
                .Append(GetWhereClause());
            //	Date Range
            if (GetDateColumn() != null
                && !MVAPATarget.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String trunc = "D";
                if (MVAPATarget.MEASUREDISPLAY_Year.Equals(measureDisplay))
                    trunc = "Y";
                else if (MVAPATarget.MEASUREDISPLAY_Quarter.Equals(measureDisplay))
                    trunc = "Q";
                else if (MVAPATarget.MEASUREDISPLAY_Month.Equals(measureDisplay))
                    trunc = "MM";
                else if (MVAPATarget.MEASUREDISPLAY_Week.Equals(measureDisplay))
                    trunc = "W";
                //	else if (MVAPATarget.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		trunc = "D";
                sql.Append(" AND TRUNC(").Append(GetDateColumn()).Append(",'").Append(trunc)
                    .Append("')=TRUNC(").Append(DataBase.DB.TO_DATE(date)).Append(",'").Append(trunc).Append("')");
            }
            String finalSQL = AddRestrictions(sql.ToString(), restrictions, role);
            //	Execute
            StringBuilder where = new StringBuilder();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(finalSQL);
                while (idr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(idr[0].ToString());
                    if (where.Length > 0)
                        where.Append(",");
                    where.Append(id);
                }
                idr.Close();

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, finalSQL, e);
            }

            if (where.Length == 0)
                return Query.GetNoRecordQuery(query.GetTableName(), false);
            //
            StringBuilder whereClause = new StringBuilder(GetKeyColumn())
                .Append(" IN (").Append(where).Append(")");
            query.AddRestriction(whereClause.ToString());
            query.SetRecordCount(1);
            return query;
        }

        /**
         * 	Add Restrictions
         *	@param sql existing sql
         *	@param restrictions restrictions
         *	@param role role
         *	@return updated sql
         */
        private String AddRestrictions(String sql, MVAPATargetRestriction[] restrictions, MVAFRole role)
        {
            return AddRestrictions(sql, false, restrictions, role,
                GetTableName(), GetOrgColumn(), GetBPartnerColumn(), GetProductColumn(), GetCtx());
        }

        /**
         * 	Add Restrictions to SQL
         *	@param sql orig sql
         *	@param queryOnly incomplete sql for query restriction
         *	@param restrictions restrictions
         *	@param role role
         *	@param tableName table name
         *	@param orgColumn org column
         *	@param bpColumn bpartner column
         *	@param pColumn product column
         *	@return updated sql
         */
        public static String AddRestrictions(String sql, Boolean queryOnly,
            MVAPATargetRestriction[] restrictions, MVAFRole role,
            String tableName, String orgColumn, String bpColumn, String pColumn, Ctx ctx)
        {
            StringBuilder sb = new StringBuilder(sql);
            //	Org Restrictions
            if (orgColumn != null)
            {
                List<int> list = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MVAPATargetRestriction.GOALRESTRICTIONTYPE_Organization.Equals(restrictions[i].GetGoalRestrictionType()))
                        list.Add(restrictions[i].GetOrg_ID());
                    //	Hierarchy comes here
                }
                if (list.Count == 1)
                    sb.Append(" AND ").Append(orgColumn)
                        .Append("=").Append(list[0]);
                else if (list.Count > 1)
                {
                    sb.Append(" AND ").Append(orgColumn).Append(" IN (");
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(list[i]);
                    }
                    sb.Append(")");
                }
            }	//	org

            //	BPartner Restrictions
            if (bpColumn != null)
            {

                List<int> listBP = new List<int>();
                List<int> listBPG = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MVAPATargetRestriction.GOALRESTRICTIONTYPE_BusinessPartner.Equals(restrictions[i].GetGoalRestrictionType()))
                        listBP.Add(restrictions[i].GetVAB_BusinessPartner_ID());
                    //	Hierarchy comes here
                    if (MVAPATargetRestriction.GOALRESTRICTIONTYPE_BusPartnerGroup.Equals(restrictions[i].GetGoalRestrictionType()))
                        listBPG.Add(restrictions[i].GetVAB_BPart_Category_ID());
                }
                //	BP
                if (listBP.Count == 1)
                    sb.Append(" AND ").Append(bpColumn)
                        .Append("=").Append(listBP[0]);
                else if (listBP.Count > 1)
                {
                    sb.Append(" AND ").Append(bpColumn).Append(" IN (");
                    for (int i = 0; i < listBP.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listBP[i]);
                    }
                    sb.Append(")");
                }
                //	BPG
                if (bpColumn.IndexOf(".") == -1)
                    bpColumn = tableName + "." + bpColumn;
                if (listBPG.Count == 1)
                    sb.Append(" AND EXISTS (SELECT * FROM VAB_BusinessPartner bpx WHERE ")
                        .Append(bpColumn)
                        .Append("=bpx.VAB_BusinessPartner_ID AND bpx.VAB_BPART_CATEGORY_ID=")
                        .Append(listBPG[0]).Append(")");
                else if (listBPG.Count > 1)
                {
                    sb.Append(" AND EXISTS (SELECT * FROM VAB_BusinessPartner bpx WHERE ")
                        .Append(bpColumn)
                        .Append("=bpx.VAB_BusinessPartner_ID AND bpx.VAB_BPART_CATEGORY_ID IN (");
                    for (int i = 0; i < listBPG.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listBPG[i]);
                    }
                    sb.Append("))");
                }
            }	//	bp

            //	Product Restrictions
            if (pColumn != null)
            {
                List<int> listP = new List<int>();
                List<int> listPC = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MVAPATargetRestriction.GOALRESTRICTIONTYPE_Product.Equals(restrictions[i].GetGoalRestrictionType()))
                        listP.Add(restrictions[i].GetVAM_Product_ID());
                    //	Hierarchy comes here
                    if (MVAPATargetRestriction.GOALRESTRICTIONTYPE_ProductCategory.Equals(restrictions[i].GetGoalRestrictionType()))
                        listPC.Add(restrictions[i].GetVAM_ProductCategory_ID());
                }
                //	Product
                if (listP.Count == 1)
                    sb.Append(" AND ").Append(pColumn)
                        .Append("=").Append(listP[0]);
                else if (listP.Count > 1)
                {
                    sb.Append(" AND ").Append(pColumn).Append(" IN (");
                    for (int i = 0; i < listP.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listP[i]);
                    }
                    sb.Append(")");
                }
                //	Category
                if (pColumn.IndexOf(".") == -1)
                    pColumn = tableName + "." + pColumn;
                if (listPC.Count == 1)
                    sb.Append(" AND EXISTS (SELECT * FROM VAM_Product px WHERE ")
                        .Append(pColumn)
                        .Append("=px.VAM_Product_ID AND px.VAM_ProductCategory_ID=")
                        .Append(listPC[0]).Append(")");
                else if (listPC.Count > 1)
                {
                    sb.Append(" AND EXISTS (SELECT * FROM VAM_Product px WHERE ")
                    .Append(pColumn)
                    .Append("=px.VAM_Product_ID AND px.VAM_ProductCategory_ID IN (");
                    for (int i = 0; i < listPC.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listPC[i]);
                    }
                    sb.Append("))");
                }
            }	//	product
            String finalSQL = sb.ToString();
            if (queryOnly)
                return finalSQL;
            if (role == null)
                role = MVAFRole.GetDefault(ctx);
            String retValue = role.AddAccessSQL(finalSQL, tableName, true, false);
            return retValue;
        }

        /**
         * 	Get Table Name
         *	@return Table Name
         */
        public String GetTableName()
        {
            return MVAFTableView.GetTableName(GetCtx(), GetVAF_TableView_ID());
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAPAEvaluateCalc[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }

    }
}