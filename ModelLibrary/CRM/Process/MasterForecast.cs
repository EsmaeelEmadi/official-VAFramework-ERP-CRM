﻿/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : MasterForecast
 * Purpose        : 
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   24-Jan-2012
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
using System.Security.Policy;
using VAdvantage.ProcessEngine;
//using ViennaAdvantage.Model;

namespace VAdvantage.Process
{
    public class MasterForecast : SvrProcess
    {
        #region Private Variables
        string msg = "";
        string sql = "";
        int VAB_YearPeriod_ID = 0;
        X_VAB_MasterForecast mf = null;
        #endregion

        protected override void Prepare()
        {

        }

        protected override string DoIt()
        {
            mf = new X_VAB_MasterForecast(GetCtx(), GetRecord_ID(), null);
            VAB_YearPeriod_ID = mf.GetVAB_YearPeriod_ID();

            // sql = "delete from VAB_MasterForecastline where VAB_MasterForecast_id = " + mf.GetVAB_MasterForecast_ID();
            // int count = DB.ExecuteQuery(sql, null, null);
            if (VAB_YearPeriod_ID != 0)
            {
                sql = "select count(*) from VAB_MasterForecastline where VAB_MasterForecast_id = " + GetRecord_ID();
                int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (count > 0)
                {
                    sql = "update VAB_MasterForecastline set Processed = 'Y' where VAB_MasterForecast_id = " + GetRecord_ID();
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                    sql = "update VAB_MasterForecast set Processed = 'Y' where VAB_MasterForecast_id = " + GetRecord_ID();
                    res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                    msg = Msg.GetMsg(GetCtx(), "RecordsProcessed");
                    return msg;
                }
                sql = "select Distinct(M_Product_ID) from VAB_Forecastline fl inner join VAB_Forecast f on (fl.VAB_Forecast_id = f.VAB_Forecast_id) where f.VAB_YearPeriod_id = " + VAB_YearPeriod_ID + " and f.vaf_client_id = " + GetCtx().GetVAF_Client_ID() + " and f.isactive = 'Y' and f.processed = 'Y'";
                IDataReader idr = null;
                try
                {

                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        Decimal? totalQtyTeam = 0;
                        Decimal? totalPriceTeam = 0;
                        Decimal? totalQtyOpp = 0;
                        Decimal? totalPriceOpp = 0;
                        sql = "select SUM(nvl(qtyentered,0)) from VAB_Forecastline where m_product_id = " + Util.GetValueOfInt(idr[0]) + " and Processed = 'Y' and isactive = 'Y' and vaf_client_id = " + mf.GetVAF_Client_ID();
                        //sql = "select SUM(nvl(fl.qtyentered,0)) from VAB_Forecastline fl inner join VAB_Forecast f on (f.VAB_Forecast_id = fl.VAB_Forecast_id) where f.vaf_client_id = " + mf.GetVAF_Client_ID() + " and fl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and fl.Processed = 'Y' and fl.isactive = 'Y' and f.VAB_YearPeriod_id = " + VAB_YearPeriod_ID;
                        totalQtyTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        sql = "select SUM(nvl(pricestd,0)) from VAB_Forecastline where m_product_id = " + Util.GetValueOfInt(idr[0]) + " and Processed = 'Y' and isactive = 'Y' and vaf_client_id = " + mf.GetVAF_Client_ID();
                      //  sql = "select SUM(nvl(fl.pricestd,0)) from VAB_Forecastline fl inner join VAB_Forecast f on (f.VAB_Forecast_id = fl.VAB_Forecast_id) where f.vaf_client_id = " + mf.GetVAF_Client_ID() + " and fl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and fl.Processed = 'Y' and fl.isactive = 'Y' and f.VAB_YearPeriod_id = " + VAB_YearPeriod_ID;
                        totalPriceTeam = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                        if (mf.IsIncludeOpp())
                        {
                            sql = "select sum(nvl(pl.plannedqty,0))  FROM VAB_ProjectLine pl inner join VAB_Project p on (p.VAB_Project_ID = pl.VAB_Project_ID) "
                                + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") "
                                + " AND (SELECT enddate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.VAB_Order_id is null and p.ref_order_id is null and pl.isactive = 'Y' and p.vaf_client_id = " + mf.GetVAF_Client_ID();
                            totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM VAB_ProjectLine pl inner join VAB_Project p on (p.VAB_Project_ID = pl.VAB_Project_ID) "
                                + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") "
                                + " AND (SELECT enddate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.VAB_Order_id is null and p.ref_order_id is null and pl.isactive = 'Y' and p.vaf_client_id = " + mf.GetVAF_Client_ID();
                            totalPriceOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                        }

                        Decimal? totalPrice = Decimal.Add(totalPriceTeam.Value, totalPriceOpp.Value);
                        Decimal? totalQty = Decimal.Add(totalQtyTeam.Value, totalQtyOpp.Value);

                        if (totalQty.Value > 0)
                        {
                            Decimal? avgPrice = Decimal.Divide(totalPrice.Value, totalQty.Value);
                            avgPrice = Decimal.Round(avgPrice.Value, 2, MidpointRounding.AwayFromZero);

                            GenerateMasterForecast(Util.GetValueOfInt(idr[0]), totalQtyTeam, totalQtyOpp, avgPrice);
                        }
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                if (mf.IsIncludeOpp())
                {
                    OnlyOpportunityProducts();
                }
                mf.SetCurrentVersion(true);
                mf.SetProcessed(true);
                if (!mf.Save())
                {
                    log.SaveError("MasterForecastNotSaved", "MasterForecastNotSaved");
                }
                msg = Msg.GetMsg(GetCtx(), "ProcessCompleted");
            }
            return msg;
        }

        private void OnlyOpportunityProducts()
        {
            //sql = " SELECT distinct(pl.m_product_id) FROM VAB_ProjectLine pl INNER JOIN VAB_Project p ON p.VAB_Project_ID = pl.VAB_Project_ID WHERE p.VAB_Order_id IS NULL"
            //    + " AND p.ref_order_id IS  ANDNULL pl.m_product_id NOT IN (SELECT DISTINCT(M_Product_ID) FROM VAB_Forecastline fl "
            //    + " INNER JOIN VAB_Forecast f ON (fl.VAB_Forecast_id = f.VAB_Forecast_id) WHERE f.VAB_YearPeriod_id = " + VAB_YearPeriod_ID
            //    + " AND f.vaf_client_id = " + GetCtx().GetVAF_Client_ID() + " AND fl.isactive = 'Y')";
            sql = " SELECT distinct(pl.m_product_id) FROM VAB_ProjectLine pl INNER JOIN VAB_Project p ON p.VAB_Project_ID = pl.VAB_Project_ID WHERE p.vaf_client_id = " + GetVAF_Client_ID() + " and p.VAB_Order_id IS NULL"
                + " AND p.ref_order_id IS NULL AND pl.m_product_id NOT IN (select m_product_id from VAB_MasterForecastline where isactive = 'Y' and VAB_MasterForecast_id = " + GetRecord_ID() + ")";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    Decimal? totalQtyOpp = 0;
                    Decimal? totalPriceOpp = 0;
                    sql = "select sum(nvl(pl.plannedqty,0))  FROM VAB_ProjectLine pl inner join VAB_Project p on (p.VAB_Project_ID = pl.VAB_Project_ID) "
                           + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") "
                           + " AND (SELECT enddate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.VAB_Order_id is null and p.ref_order_id is null and pl.isactive = 'Y' and p.vaf_client_id = " + mf.GetVAF_Client_ID();
                    totalQtyOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                    sql = " SELECT SUM(NVL(pl.plannedqty,0) * NVL(pl.plannedprice,0)) FROM VAB_ProjectLine pl inner join VAB_Project p on (p.VAB_Project_ID = pl.VAB_Project_ID) "
                        + " WHERE pl.planneddate BETWEEN (SELECT startdate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") "
                        + " AND (SELECT enddate FROM VAB_YearPeriod WHERE VAB_YearPeriod_id = " + VAB_YearPeriod_ID + ") AND pl.m_product_id =  " + Util.GetValueOfInt(idr[0]) + " and p.VAB_Order_id is null and p.ref_order_id is null and pl.isactive = 'Y' and p.vaf_client_id = " + mf.GetVAF_Client_ID();
                    totalPriceOpp = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));

                    if (totalQtyOpp.Value > 0)
                    {
                        Decimal? avgPrice = Decimal.Divide(totalPriceOpp.Value, totalQtyOpp.Value);
                        avgPrice = Decimal.Round(avgPrice.Value, 2, MidpointRounding.AwayFromZero);

                        GenerateMasterForecast(Util.GetValueOfInt(idr[0]), Util.GetValueOfDecimal(Decimal.Zero), totalQtyOpp, avgPrice);
                    }
                }
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            catch
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
        }

        private void GenerateMasterForecast(int M_Product_ID, decimal? totalQtyTeam, Decimal? totalQtyOpp, decimal? avgPrice)
        {
            sql = "select VAB_UOM_id from m_product where m_product_id = " + M_Product_ID;
            MMasterForecastLine mfLine = new MMasterForecastLine(GetCtx(), 0, null);
            mfLine.SetVAF_Client_ID(mf.GetVAF_Client_ID());
            mfLine.SetVAF_Org_ID(mf.GetVAF_Org_ID());
            mfLine.SetM_Product_ID(M_Product_ID);
            mfLine.SetVAB_MasterForecast_ID(mf.GetVAB_MasterForecast_ID());
            mfLine.SetForcastQty(totalQtyTeam);
            mfLine.SetOppQty(totalQtyOpp);
            Decimal? total = Decimal.Add(totalQtyOpp.Value, totalQtyTeam.Value);
            mfLine.SetTotalQty(total);
            mfLine.SetPrice(avgPrice);
            mfLine.SetVAB_UOM_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));
            mfLine.SetProcessed(true);
            Decimal? planRevenue = Decimal.Round(Decimal.Multiply(avgPrice.Value, total.Value), 2, MidpointRounding.AwayFromZero);
            mfLine.SetPlannedRevenue(planRevenue);
            if (!mfLine.Save())
            {
                log.SaveError("MasterForecastLineNotSaved", "MasterForecastLineNotSaved");
            }

        }
    }
}
