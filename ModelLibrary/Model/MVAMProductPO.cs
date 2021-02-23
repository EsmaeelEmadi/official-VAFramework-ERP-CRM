﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMProductPO
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     18-Jun-2009
  ******************************************************/
using System;
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
    public class MVAMProductPO : X_VAM_Product_PO
    {
        /*	Get current PO of Product
        * 	@param ctx context
        *	@param VAM_Product_ID product
        *	@param trxName transaction
        *	@return PO - current vendor first
        */
        public static MVAMProductPO[] GetOfProduct(Ctx ctx, int VAM_Product_ID, Trx trxName)
        {
            List<MVAMProductPO> list = new List<MVAMProductPO>();
            String sql = "SELECT * FROM VAM_Product_PO "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND IsActive='Y' "
                + "ORDER BY IsCurrentVendor DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMProductPO(ctx, dr, trxName));
                }

            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MVAMProductPO[] retValue = new MVAMProductPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        // added By Amit 4-8-2015 VAMRP
        public static MVAMProductPO GetOfVendorProduct(Ctx ctx, int VAB_BusinessPartner_ID, int VAM_Product_ID, Trx trx)
        {
            MVAMProductPO productPO = null;
            String sql = "SELECT * FROM VAM_Product_PO "
                + "WHERE VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND VAM_Product_ID = " + VAM_Product_ID;

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    productPO = (new MVAMProductPO(ctx, dt.Rows[0], trx));
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return productPO;
        }
        //end

        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductPO).FullName);

        /**
         * 	Persistency Constructor
         *	@param ctx context
         *	@param ignored ignored
         *	@param trxName transaction
         */
        public MVAMProductPO(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            else
            {
                //	setVAM_Product_ID (0);	// @VAM_Product_ID@
                //	setVAB_BusinessPartner_ID (0);	// 0
                //	setVendorProductNo (null);	// @Value@
                SetIsCurrentVendor(true);	// Y
            }
        }


        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVAMProductPO(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        ///  Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true, on Save</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            // JID_0527: System is allowing to select 2 vendors as current vendor
            if (IsCurrentVendor())
            {
                String sql = "SELECT COUNT(VAM_Product_ID) FROM VAM_Product_PO "
                    + "WHERE VAB_BusinessPartner_ID != " + GetVAB_BusinessPartner_ID() + " AND VAM_Product_ID = " + GetVAM_Product_ID() + " AND IsActive = 'Y' "
                    + " AND IsCurrentVendor = 'Y'";
                int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (no > 0)
                {
                    log.SaveError("CurrentVendorIsDefined", "");
                    return false;
                }
            }
            return true;
        }
    }
}