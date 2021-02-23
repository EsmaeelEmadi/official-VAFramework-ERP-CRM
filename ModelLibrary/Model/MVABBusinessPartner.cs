﻿/********************************************************
 * Module Name    : VFramwork
 * Purpose        : Business Partner Model
 * Class Used     : X_VAB_BusinessPartner
 * Chronological Development
 * Veena Pandey     02-June-2009
 * Raghunandan      24-june-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Business Partner Model
    /// </summary>
    public class MVABBusinessPartner : X_VAB_BusinessPartner
    {
        #region private Variables
        // Users						
        private MVAFUserContact[] _contacts = null;
        //Addressed						
        private MVABBPartLocation[] _locations = null;
        // BP Bank Accounts				
        private MVABBPartBankAcct[] _accounts = null;
        // Prim Address					
        private int? _primaryVAB_BPart_Location_ID = null;
        // Prim User						
        private int? _primaryVAF_UserContact_ID = null;
        // Credit Limit recently calcualted		
        private bool _TotalOpenBalanceSet = false;
        // BP Group						
        private MVABBPartCategory _group = null;
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABBusinessPartner).FullName);
        #endregion

        /// <summary>
        /// Get Empty Template Business Partner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <returns>Template Business Partner or null</returns>
        public static MVABBusinessPartner GetTemplate(Ctx ctx, int VAF_Client_ID)
        {
            MVABBusinessPartner template = GetBPartnerCashTrx(ctx, VAF_Client_ID);
            if (template == null)
                template = new MVABBusinessPartner(ctx, 0, null);
            //	Reset
            if (template != null)
            {
                template.Set_ValueNoCheck("VAB_BusinessPartner_ID", 0);
                template.SetValue("");
                template.SetName("");
                template.SetName2(null);
                template.SetDUNS("");
                template.SetFirstSale(null);
                //
                template.SetSO_CreditLimit(Env.ZERO);
                template.SetSO_CreditUsed(Env.ZERO);
                template.SetTotalOpenBalance(Env.ZERO);
                //	s_template.setRating(null);
                //
                template.SetActualLifeTimeValue(Env.ZERO);
                template.SetPotentialLifeTimeValue(Env.ZERO);
                template.SetAcqusitionCost(Env.ZERO);
                template.SetShareOfCustomer(0);
                template.SetSalesVolume(0);
            }
            return template;
        }

        /// <summary>
        /// Get Cash Trx Business Partner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <returns>Cash Trx Business Partner or null</returns>
        public static MVABBusinessPartner GetBPartnerCashTrx(Ctx ctx, int VAF_Client_ID)
        {
            MVABBusinessPartner retValue = null;
            String sql = "SELECT * FROM VAB_BusinessPartner "
                + " WHERE VAB_BusinessPartner_ID IN (SELECT VAB_BusinessPartnerCashTrx_ID FROM VAF_ClientDetail" +
                " WHERE VAF_Client_ID=" + VAF_Client_ID + ")";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVABBusinessPartner(ctx, dr, null);
                }
                if (dt == null)
                {
                    _log.Log(Level.SEVERE, "Not found for VAF_Client_ID=" + VAF_Client_ID);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            return retValue;
        }

        /// <summary>
        /// Get BPartner with Value
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">value</param>
        /// <returns>BPartner or null</returns>
        public static MVABBusinessPartner Get(Ctx ctx, String Value)
        {
            if (Value == null || Value.Length == 0)
                return null;
            MVABBusinessPartner retValue = null;
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAB_BusinessPartner WHERE Value=@Value"
                + " AND VAF_Client_ID=" + VAF_Client_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@Value", Value);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVABBusinessPartner(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            return retValue;
        }

        /// <summary>
        /// Get BPartner with Value
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_BusinessPartner_ID">id</param>
        /// <returns>BPartner or null</returns>
        public static MVABBusinessPartner Get(Ctx ctx, int VAB_BusinessPartner_ID)
        {
            MVABBusinessPartner retValue = null;
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID;
            //    + " AND  VAF_Client_ID=" + VAF_Client_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVABBusinessPartner(ctx, dr, null);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Not Invoiced Shipment Value
        /// </summary>
        /// <param name="VAB_BusinessPartner_ID">partner</param>
        /// <returns>value in accounting currency</returns>
        public static Decimal GetNotInvoicedAmt(int VAB_BusinessPartner_ID)
        {
            Decimal retValue = new decimal();
            String sql = "SELECT SUM(COALESCE("
                + "CURRENCYBASEWITHCONVERSIONTYPE((ol.QtyDelivered-ol.QtyInvoiced)*ol.PriceActual,o.VAB_Currency_ID,o.DateOrdered, o.VAF_Client_ID,o.VAF_Org_ID, o.VAB_CurrencyType_ID) ,0)) "
                + " FROM VAB_OrderLine ol"
                + " INNER JOIN VAB_Order o ON (ol.VAB_Order_ID=o.VAB_Order_ID) "
                + " WHERE o.IsSOTrx='Y' AND Bill_BPartner_ID=" + VAB_BusinessPartner_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Constructor for new BPartner from Template
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="trxName">transaction</param>
        public MVABBusinessPartner(Ctx ctx, Trx trxName)
            : this(ctx, -1, trxName)
        {

        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABBusinessPartner(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_BusinessPartner_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABBusinessPartner(Ctx ctx, int VAB_BusinessPartner_ID, Trx trxName)
            : base(ctx, VAB_BusinessPartner_ID, trxName)
        {
            //
            if (VAB_BusinessPartner_ID == -1)
            {
                InitTemplate(ctx.GetContextAsInt("VAF_Client_ID"));
                VAB_BusinessPartner_ID = 0;
            }
            if (VAB_BusinessPartner_ID == 0)
            {
                //	setValue ("");
                //	setName ("");
                //	setName2 (null);
                //	setDUNS("");
                //
                SetIsCustomer(true);
                SetIsProspect(true);
                //
                SetSendEMail(false);
                SetIsOneTime(false);
                SetIsVendor(false);
                SetIsSummary(false);
                SetIsEmployee(false);
                SetIsSalesRep(false);
                SetIsTaxExempt(false);
                SetIsDiscountPrinted(false);
                //
                SetSO_CreditLimit(Env.ZERO);
                SetSO_CreditUsed(Env.ZERO);
                SetTotalOpenBalance(Env.ZERO);
                SetSOCreditStatus(SOCREDITSTATUS_NoCreditCheck);
                //
                SetFirstSale(null);
                SetActualLifeTimeValue(Env.ZERO);
                SetPotentialLifeTimeValue(Env.ZERO);
                SetAcqusitionCost(Env.ZERO);
                SetShareOfCustomer(0);
                SetSalesVolume(0);
            }
            log.Fine(ToString());
        }

        /// <summary>
        /// Import Contstructor
        /// </summary>
        /// <param name="impBP">import</param>
        public MVABBusinessPartner(X_I_BPartner impBP)
            : this(impBP.GetCtx(), 0, impBP.Get_TrxName())
        {

            SetClientOrg(impBP);
            SetUpdatedBy(impBP.GetUpdatedBy());
            //
            String value = impBP.GetValue();
            if (value == null || value.Length == 0)
                value = impBP.GetEMail();
            if (value == null || value.Length == 0)
                value = impBP.GetContactName();
            SetValue(value);
            String name = impBP.GetName();
            if (name == null || name.Length == 0)
                name = impBP.GetContactName();
            if (name == null || name.Length == 0)
                name = impBP.GetEMail();
            SetName(name);
            SetName2(impBP.GetName2());
            SetDescription(impBP.GetDescription());
            //	setHelp(impBP.getHelp());
            SetDUNS(impBP.GetDUNS());
            SetTaxID(impBP.GetTaxID());
            SetNAICS(impBP.GetNAICS());
            SetVAB_BPart_Category_ID(impBP.GetVAB_BPart_Category_ID());
        }

        /// <summary>
        /// Load Default BPartner
        /// </summary>
        /// <param name="VAF_Client_ID">client id</param>
        /// <returns>true if loaded</returns>
        private bool InitTemplate(int VAF_Client_ID)
        {
            if (VAF_Client_ID == 0)
                throw new ArgumentException("Client_ID=0");

            bool success = true;
            String sql = "SELECT * FROM VAB_BusinessPartner "
                + " WHERE VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartnerCashTrx_ID FROM VAF_ClientDetail" +
                " WHERE VAF_Client_ID=" + VAF_Client_ID + ")";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                {
                    success = Load(ds.Tables[0].Rows[0]);
                }
                else
                {
                    Load(0, (Trx)null);
                    success = false;
                    log.Severe("None found");
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            SetStandardDefaults();
            //	Reset
            Set_ValueNoCheck("VAB_BusinessPartner_ID", I_ZERO);
            SetValue("");
            SetName("");
            SetName2(null);
            return success;
        }

        /// <summary>
        /// Get All Contacts
        /// </summary>
        /// <param name="reload">if true users will be requeried</param>
        /// <returns>contacts</returns>
        public MVAFUserContact[] GetContacts(bool reload)
        {
            if (reload || _contacts == null || _contacts.Length == 0)
            {
                ;
            }
            else
                return _contacts;
            //
            List<MVAFUserContact> list = new List<MVAFUserContact>();
            String sql = "SELECT * FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAFUserContact(GetCtx(), dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _contacts = new MVAFUserContact[list.Count];
            _contacts = list.ToArray();
            return _contacts;
        }

        /// <summary>
        /// Get specified or first Contact
        /// </summary>
        /// <param name="VAF_UserContact_ID">optional user</param>
        /// <returns>contact or null</returns>
        public MVAFUserContact GetContact(int VAF_UserContact_ID)
        {
            MVAFUserContact[] users = GetContacts(false);
            if (users.Length == 0)
                return null;
            for (int i = 0; VAF_UserContact_ID != 0 && i < users.Length; i++)
            {
                if (users[i].GetVAF_UserContact_ID() == VAF_UserContact_ID)
                    return users[i];
            }
            return users[0];
        }

        /// <summary>
        /// Get All Locations
        /// </summary>
        /// <param name="reload">if true locations will be requeried</param>
        /// <returns>locations</returns>
        public MVABBPartLocation[] GetLocations(bool reload)
        {
            if (reload || _locations == null || _locations.Length == 0)
            {
                ;
            }
            else
            {
                return _locations;
            }
            //
            List<MVABBPartLocation> list = new List<MVABBPartLocation>();
            String sql = "SELECT * FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVABBPartLocation(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _locations = new MVABBPartLocation[list.Count];
            _locations = list.ToArray();
            return _locations;
        }

        /// <summary>
        /// Get explicit or first bill Location
        /// </summary>
        /// <param name="VAB_BPart_Location_ID">optional explicit location</param>
        /// <returns>location or null</returns>
        public MVABBPartLocation GetLocation(int VAB_BPart_Location_ID)
        {
            MVABBPartLocation[] locations = GetLocations(false);
            if (locations.Length == 0)
                return null;
            MVABBPartLocation retValue = null;
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i].GetVAB_BPart_Location_ID() == VAB_BPart_Location_ID)
                    return locations[i];
                if (retValue == null && locations[i].IsBillTo())
                    retValue = locations[i];
            }
            if (retValue == null)
                return locations[0];
            return retValue;
        }

        /// <summary>
        /// Get Bank Accounts
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>Bank Accounts</returns>
        public MVABBPartBankAcct[] GetBankAccounts(bool requery)
        {
            if (_accounts != null && _accounts.Length >= 0 && !requery)	//	re-load
                return _accounts;
            //
            List<MVABBPartBankAcct> list = new List<MVABBPartBankAcct>();
            String sql = "SELECT * FROM VAB_BPart_Bank_Acct WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID()
                + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABBPartBankAcct(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            _accounts = new MVABBPartBankAcct[list.Count];
            _accounts = list.ToArray();
            return _accounts;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBPartner[ID=")
                .Append(Get_ID())
                .Append(",Value=").Append(GetValue())
                .Append(",Name=").Append(GetName())
                .Append(",OpenBalance=").Append(GetTotalOpenBalance())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Set Client/Org
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        //public void SetClientOrg(int VAF_Client_ID, int VAF_Org_ID)
        //{
        //    base.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
        //}

        /// <summary>
        /// Set Linked Organization.
        /// </summary>
        /// <param name="VAF_OrgBP_ID">id</param>
        public void SetVAF_OrgBP_ID(int VAF_OrgBP_ID)
        {
            if (VAF_OrgBP_ID == 0)
                base.SetVAF_OrgBP_ID(null);
            else
                base.SetVAF_OrgBP_ID(VAF_OrgBP_ID.ToString());
        }

        /// <summary>
        /// Get Linked Organization.
        ///	(is Button)
        /// The Business Partner is another Organization 
        ///	for explicit Inter-Org transactions 
        /// </summary>
        /// <returns>VAF_OrgBP_ID if BP</returns>
        public int GetVAF_OrgBP_ID_Int()
        {
            String org = base.GetVAF_OrgBP_ID();
            if (org == null)
                return 0;
            int VAF_OrgBP_ID = 0;
            try
            {
                VAF_OrgBP_ID = int.Parse(org);
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, org, ex);
            }
            return VAF_OrgBP_ID;
        }

        /// <summary>
        /// Get Primary VAB_BPart_Location_ID
        /// </summary>
        /// <returns>VAB_BPart_Location_ID</returns>
        public int GetPrimaryVAB_BPart_Location_ID()
        {
            if (_primaryVAB_BPart_Location_ID == null)
            {
                MVABBPartLocation[] locs = GetLocations(false);
                for (int i = 0; _primaryVAB_BPart_Location_ID == null && i < locs.Length; i++)
                {
                    if (locs[i].IsBillTo())
                    {
                        SetPrimaryVAB_BPart_Location_ID(locs[i].GetVAB_BPart_Location_ID());
                        break;
                    }
                }
                //	get first
                if (_primaryVAB_BPart_Location_ID == null && locs.Length > 0)
                    SetPrimaryVAB_BPart_Location_ID(locs[0].GetVAB_BPart_Location_ID());
            }
            if (_primaryVAB_BPart_Location_ID == null)
                return 0;
            return (int)_primaryVAB_BPart_Location_ID;
        }

        /// <summary>
        /// Get Primary VAB_BPart_Location
        /// </summary>
        /// <returns>VAB_BPart_Location</returns>
        public MVABBPartLocation GetPrimaryVAB_BPart_Location()
        {
            if (_primaryVAB_BPart_Location_ID == null)
            {
                _primaryVAB_BPart_Location_ID = GetPrimaryVAB_BPart_Location_ID();
            }
            if (_primaryVAB_BPart_Location_ID == null)
                return null;
            return new MVABBPartLocation(GetCtx(), (int)_primaryVAB_BPart_Location_ID, null);
        }

        /// <summary>
        /// Get Primary VAF_UserContact_ID
        /// </summary>
        /// <returns>VAF_UserContact_ID</returns>
        public int GetPrimaryVAF_UserContact_ID()
        {
            if (_primaryVAF_UserContact_ID == null)
            {
                MVAFUserContact[] users = GetContacts(false);
                //	for (int i = 0; i < users.Length; i++)
                //	{
                //	}
                if (_primaryVAF_UserContact_ID == null && users.Length > 0)
                    SetPrimaryVAF_UserContact_ID(users[0].GetVAF_UserContact_ID());
            }
            if (_primaryVAF_UserContact_ID == null)
                return 0;
            return (int)_primaryVAF_UserContact_ID;
        }

        /// <summary>
        /// Set Primary VAB_BPart_Location_ID
        /// </summary>
        /// <param name="VAB_BPart_Location_ID">id</param>
        public void SetPrimaryVAB_BPart_Location_ID(int VAB_BPart_Location_ID)
        {
            _primaryVAB_BPart_Location_ID = VAB_BPart_Location_ID;
        }

        /// <summary>
        /// Set Primary VAF_UserContact_ID
        /// </summary>
        /// <param name="VAF_UserContact_ID">id</param>
        public void SetPrimaryVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            _primaryVAF_UserContact_ID = VAF_UserContact_ID;
        }

        /// <summary>
        /// Calculate Total Open Balance and SO_CreditUsed.
        /// (includes drafted invoices)
        /// </summary>
        public void SetTotalOpenBalance()
        {
            Decimal? SO_CreditUsed = null;
            Decimal? TotalOpenBalance = null;
            String sql = "SELECT "
                //	SO Credit Used	= SO Invoices
                + "COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal,i.VAB_Currency_ID,i.DateOrdered, i.VAF_Client_ID,i.VAF_Org_ID, i.VAB_CurrencyType_ID)) "
                    + " FROM VAB_Invoice_v i "
                    + " WHERE i.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND i.IsSOTrx='Y' AND i.DocStatus IN('CO','CL')),0) "
                //					- All SO Allocations
                + "-COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)) "
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                    + " INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID) "
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID AND a.IsActive='Y'"
                    + " AND i.isSoTrx='Y' AND h.DocStatus IN('CO','CL')),0) "
                //					- Unallocated Receipts	= (All Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(p.PayAmt+p.DiscountAmt+p.WriteoffAmt,p.VAB_Currency_ID,p.DateTrx,p.VAF_Client_ID,p.VAF_Org_ID, p.VAB_CurrencyType_ID)),0) "
                    + " FROM VAB_Payment_V p "
                    + " WHERE p.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND p.IsReceipt='Y' AND p.DocStatus IN('CO','CL')"
                    + " AND p.VAB_Charge_ID IS NULL)"
                // JID_1224: Consider Cash Journal Transaction also to Get Total Open Balance of Business Partner
                //                  - Unallocated Cash Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(cl.Amount,cl.VAB_Currency_ID,c.StatementDate, cl.VAF_Client_ID,cl.VAF_Org_ID, cl.VAB_CurrencyType_ID)),0)"
                    + " FROM VAB_CASHJRNLLINE cl INNER JOIN VAB_CashJRNL C ON C.VAB_CashJRNL_ID = cl.VAB_CashJRNL_ID WHERE cl.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND cl.VSS_PaymentType ='R' AND c.DocStatus IN('CO','CL') AND cl.VAB_Charge_ID IS NULL)"
                //											- All Receipt Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)),0) "
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                    + " INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID) "
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND a.IsActive='Y' AND a.VAB_Payment_ID IS NOT NULL"
                    + " AND i.isSoTrx='Y' AND h.DocStatus IN('CO','CL')) "
                //                                          - All Cash Receipt Allocatioons
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)),0)"
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID)"
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID AND A.IsActive ='Y' AND a.VAB_CashJRNLLine_ID IS NOT NULL AND i.isSoTrx ='Y' AND h.DocStatus IN('CO','CL')), "

                //	Balance			= All Invoices
                + "COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal*MultiplierAP,i.VAB_Currency_ID,i.DateOrdered, i.VAF_Client_ID,i.VAF_Org_ID, i.VAB_CurrencyType_ID)) "
                    + " FROM VAB_Invoice_v i "
                    + " WHERE i.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND i.DocStatus IN('CO','CL')),0) "
                //					- All Allocations
                + "-COALESCE((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)) "
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                    + " INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID) "
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID AND a.IsActive='Y' AND h.DocStatus IN('CO','CL')),0) "
                //					- Unallocated Receipts	= (All Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(p.PayAmt+p.DiscountAmt+p.WriteoffAmt,p.VAB_Currency_ID,p.DateTrx,p.VAF_Client_ID,p.VAF_Org_ID, p.VAB_CurrencyType_ID)),0) "
                    + " FROM VAB_Payment_V p "
                    + " WHERE p.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND p.DocStatus IN('CO','CL')"
                    + " AND p.VAB_Charge_ID IS NULL)"
                // JID_1224: Consider Cash Journal Transaction also to Get Total Open Balance of Business Partner
                //                  - Unallocated Cash Receipts
                + "-(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(cl.Amount,cl.VAB_Currency_ID,c.StatementDate, cl.VAF_Client_ID,cl.VAF_Org_ID, cl.VAB_CurrencyType_ID)),0)"
                    + " FROM VAB_CASHJRNLLINE cl INNER JOIN VAB_CashJRNL C ON C.VAB_CashJRNL_ID = cl.VAB_CashJRNL_ID WHERE cl.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND c.DocStatus IN('CO','CL') AND cl.VAB_Charge_ID IS NULL)"
                //											- All Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)),0) "
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                    + " INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID) "
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND a.IsActive='Y' AND a.VAB_Payment_ID IS NOT NULL AND h.DocStatus IN('CO','CL')) "
                //											- All Cash Allocations
                + "+(SELECT COALESCE(SUM(CURRENCYBASEWITHCONVERSIONTYPE(a.Amount+a.DiscountAmt+a.WriteoffAmt,i.VAB_Currency_ID,i.DateOrdered,a.VAF_Client_ID,a.VAF_Org_ID, i.VAB_CurrencyType_ID)),0) "
                    + " FROM VAB_DocAllocationLine a INNER JOIN VAB_Invoice i ON (a.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                    + " INNER JOIN VAB_DocAllocation h ON (a.VAB_DocAllocation_ID = h.VAB_DocAllocation_ID) "
                    + " WHERE a.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID"
                    + " AND a.IsActive='Y' AND a.VAB_CashJRNLLine_ID IS NOT NULL AND h.DocStatus IN('CO','CL')) "
                //
                + " FROM VAB_BusinessPartner bp "
                + " WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    SO_CreditUsed = Utility.Util.GetValueOfDecimal(dr[0]);
                    TotalOpenBalance = Utility.Util.GetValueOfDecimal(dr[1]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            _TotalOpenBalanceSet = true;
            String info = null;
            if (SO_CreditUsed != null)
            {
                info = "SO_CreditUsed=" + SO_CreditUsed;
                base.SetSO_CreditUsed(Convert.ToDecimal(SO_CreditUsed));
            }

            if (TotalOpenBalance != null)
            {
                if (info != null)
                    info += ", ";
                info += "TotalOpenBalance=" + TotalOpenBalance;
                base.SetTotalOpenBalance(Convert.ToDecimal(TotalOpenBalance));
            }
            log.Fine(info);
            SetSOCreditStatus();
        }

        /// <summary>
        /// Set Actual Life Time Value from DB
        /// </summary>
        public void SetActualLifeTimeValue()
        {
            Decimal? ActualLifeTimeValue = null;
            String sql = "SELECT "
                + "COALESCE ((SELECT SUM(CURRENCYBASEWITHCONVERSIONTYPE(i.GrandTotal,i.VAB_Currency_ID,i.DateOrdered,"
                + " i.VAF_Client_ID,i.VAF_Org_ID, i.VAB_CurrencyType_ID)) "
                    + " FROM VAB_Invoice_v i WHERE i.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID AND i.IsSOTrx='Y'"
            + " AND i.DocStatus IN('CO','CL')),0) FROM VAB_BusinessPartner bp "
                + " WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    ActualLifeTimeValue = Utility.Util.GetValueOfDecimal(dr[0]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            if (ActualLifeTimeValue != null)
                base.SetActualLifeTimeValue(Convert.ToDecimal(ActualLifeTimeValue));
        }

        /// <summary>
        /// Get Total Open Balance
        /// </summary>
        /// <param name="calculate">if null calculate it</param>
        /// <returns>open balance</returns>
        public Decimal GetTotalOpenBalance(bool calculate)
        {
            if (Env.Signum(GetTotalOpenBalance()) == 0 && calculate)
                SetTotalOpenBalance();
            return base.GetTotalOpenBalance();
        }

        /// <summary>
        /// Set Credit Status
        /// </summary>
        public void SetSOCreditStatus()
        {
            Decimal creditLimit = GetSO_CreditLimit();
            //	Nothing to do
            if (SOCREDITSTATUS_NoCreditCheck.Equals(GetSOCreditStatus())
                || SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus())
                || Env.ZERO.CompareTo(creditLimit) == 0)
                return;

            //	Above Credit Limit
            // changed this function for fetching open balance because in case of void it calculates again and fetches wrong value of open balance // Lokesh Chauhan
            //if (creditLimit.CompareTo(GetTotalOpenBalance(!_TotalOpenBalanceSet)) < 0)
            if (creditLimit.CompareTo(GetTotalOpenBalance()) <= 0)
                SetSOCreditStatus(SOCREDITSTATUS_CreditHold);
            else
            {
                //	Above Watch Limit
                Decimal watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());
                if (watchAmt.CompareTo(GetTotalOpenBalance()) < 0)
                    SetSOCreditStatus(SOCREDITSTATUS_CreditWatch);
                else	//	is OK
                    SetSOCreditStatus(SOCREDITSTATUS_CreditOK);
            }
            log.Fine("SOCreditStatus=" + GetSOCreditStatus());
        }

        /// <summary>
        /// Get SO CreditStatus with additional amount
        /// </summary>
        /// <param name="additionalAmt">additional amount in Accounting Currency</param>
        /// <returns>sinulated credit status</returns>
        public String GetSOCreditStatus(Decimal? additionalAmt)
        {
            if (additionalAmt == null || Env.Signum((Decimal)additionalAmt) == 0)
                return GetSOCreditStatus();
            //
            Decimal creditLimit = GetSO_CreditLimit();
            //	Nothing to do
            if (SOCREDITSTATUS_NoCreditCheck.Equals(GetSOCreditStatus())
                || SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus())
                || Env.ZERO.CompareTo(creditLimit) == 0)
                return GetSOCreditStatus();
            //	Above (reduced) Credit Limit
            creditLimit = Decimal.Subtract(creditLimit, (Decimal)additionalAmt);
            if (creditLimit.CompareTo(GetTotalOpenBalance(!_TotalOpenBalanceSet)) < 0)
                return SOCREDITSTATUS_CreditHold;

            //	Above Watch Limit
            Decimal watchAmt = Decimal.Multiply(creditLimit, GetCreditWatchRatio());
            if (watchAmt.CompareTo(GetTotalOpenBalance()) < 0)
                return SOCREDITSTATUS_CreditWatch;
            //	is OK
            return SOCREDITSTATUS_CreditOK;
        }

        /// <summary>
        /// Get Credit Watch Ratio
        /// </summary>
        /// <returns>BP Group ratio or 0.9</returns>
        public Decimal GetCreditWatchRatio()
        {
            return GetBPGroup().GetCreditWatchRatio();
        }

        /// <summary>
        /// Credit Status is Stop or Hold.
        /// </summary>
        /// <returns>true if Stop/Hold</returns>
        public bool IsCreditStopHold()
        {
            String status = GetSOCreditStatus();
            return SOCREDITSTATUS_CreditStop.Equals(status)
                || SOCREDITSTATUS_CreditHold.Equals(status);
        }

        /// <summary>
        /// Set Total Open Balance
        /// </summary>
        /// <param name="TotalOpenBalance">Total Open Balance</param>
        public void SetTotalOpenBalance(Decimal TotalOpenBalance)
        {
            _TotalOpenBalanceSet = false;
            base.SetTotalOpenBalance(TotalOpenBalance);
        }

        /// <summary>
        /// Get BP Group
        /// </summary>
        /// <returns>group</returns>
        public MVABBPartCategory GetBPGroup()
        {
            if (_group == null)
            {
                if (GetVAB_BPart_Category_ID() == 0)
                    _group = MVABBPartCategory.GetDefault(GetCtx());
                else
                    _group = MVABBPartCategory.Get(GetCtx(), GetVAB_BPart_Category_ID());
            }
            return _group;
        }

        /// <summary>
        /// Get BP Group
        /// </summary>
        /// <param name="group">group</param>
        public void SetBPGroup(MVABBPartCategory group)
        {
            _group = group;
            if (_group == null)
                return;
            SetVAB_BPart_Category_ID(_group.GetVAB_BPart_Category_ID());
            if (_group.GetVAB_Dunning_ID() != 0)
                SetVAB_Dunning_ID(_group.GetVAB_Dunning_ID());
            // if pricelist already selected from UI then skip..
            if (base.GetVAM_PriceList_ID() > 0) { }
            else
            {
                if (_group.GetVAM_PriceList_ID() != 0)
                    SetVAM_PriceList_ID(_group.GetVAM_PriceList_ID());
            }
            if (_group.GetPO_PriceList_ID() != 0)
                SetPO_PriceList_ID(_group.GetPO_PriceList_ID());
            if (_group.GetVAM_DiscountCalculation_ID() != 0)
                SetVAM_DiscountCalculation_ID(_group.GetVAM_DiscountCalculation_ID());
            if (_group.GetPO_DiscountSchema_ID() != 0)
                SetPO_DiscountSchema_ID(_group.GetPO_DiscountSchema_ID());
        }

        /// <summary>
        /// Get PriceList
        /// </summary>
        /// <returns>price list</returns>
        public new int GetVAM_PriceList_ID()
        {
            int ii = base.GetVAM_PriceList_ID();
            if (ii == 0)
                ii = GetBPGroup().GetVAM_PriceList_ID();
            return ii;
        }

        /// <summary>
        /// Get PO PriceList
        /// </summary>
        /// <returns>price list</returns>
        public new int GetPO_PriceList_ID()
        {
            int ii = base.GetPO_PriceList_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_PriceList_ID();
            return ii;
        }

        /// <summary>
        /// Get DiscountSchema
        /// </summary>
        /// <returns>Discount Schema</returns>
        public new int GetVAM_DiscountCalculation_ID()
        {
            int ii = base.GetVAM_DiscountCalculation_ID();
            if (ii == 0)
                ii = GetBPGroup().GetVAM_DiscountCalculation_ID();
            return ii;
        }

        /// <summary>
        /// Get PO DiscountSchema
        /// </summary>
        /// <returns>PO Discount</returns>
        public new int GetPO_DiscountSchema_ID()
        {
            int ii = base.GetPO_DiscountSchema_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_DiscountSchema_ID();
            return ii;
        }

        /// <summary>
        /// Get ReturnPolicy
        /// </summary>
        /// <returns>Return Policy</returns>
        public new int GetVAM_ReturnRule_ID()
        {
            int ii = base.GetVAM_ReturnRule_ID();
            if (ii == 0)
                ii = GetBPGroup().GetVAM_ReturnRule_ID();
            if (ii == 0)
                ii = MVAMReturnRule.GetDefault(GetCtx());
            return ii;
        }

        /// <summary>
        /// Get Vendor ReturnPolicy
        /// </summary>
        /// <returns>Return Policy</returns>
        public new int GetPO_ReturnPolicy_ID()
        {
            int ii = base.GetPO_ReturnPolicy_ID();
            if (ii == 0)
                ii = GetBPGroup().GetPO_ReturnPolicy_ID();
            if (ii == 0)
                ii = MVAMReturnRule.GetDefault(GetCtx());
            return ii;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord || Is_ValueChanged("VAB_BPart_Category_ID"))
            {
                MVABBPartCategory grp = GetBPGroup();
                if (grp == null)
                {
                    log.SaveWarning("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@:  @VAB_BPart_Category_ID@"));
                    return false;
                }
                SetBPGroup(grp);	//	setDefaults
            }

            //when we select payment method then need to update  payment rule accordingly
            if (Get_ColumnIndex("VA009_PaymentMethod_ID") >= 0)
            {
                if ((GetVA009_PaymentMethod_ID() > 0) || (GetVA009_PO_PaymentMethod_ID() > 0))
                {

                    if (IsCustomer())
                    {
                        string paymentBaseType = Util.GetValueOfString(DB.ExecuteScalar(@"select VA009_PAYMENTBASETYPE from VA009_PAYMENTMETHOD where VA009_PAYMENTMETHOD_ID=" + GetVA009_PaymentMethod_ID()));
                        //if (paymentBaseType == "C")  // Cash + Card
                        //{

                        //}
                        //else if (paymentBaseType == "W") // Wire Transfer
                        //{

                        //}
                        //else
                        SetPaymentRule(paymentBaseType);
                    }
                    if (IsVendor())
                    {
                        string paymentBaseType = Util.GetValueOfString(DB.ExecuteScalar(@"select VA009_PAYMENTBASETYPE from VA009_PAYMENTMETHOD where VA009_PAYMENTMETHOD_ID=" + GetVA009_PO_PaymentMethod_ID()));
                        //if (paymentBaseType == "C") // Cash + Card
                        //{

                        //}
                        //else if (paymentBaseType == "W") // Wire Transfer
                        //{

                        //}
                        //else
                        SetPaymentRulePO(paymentBaseType);
                    }
                }
                else
                {
                    SetPaymentRulePO(null);
                    SetPaymentRule(null);
                }
            }

            // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
            if (IsEmployee())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAF_ModuleInfo WHERE Prefix='VAHRUAE_' AND isactive='Y'")) > 0)
                {
                    if (newRecord)
                    {
                        X_VAB_Position job = new X_VAB_Position(GetCtx(), GetVAB_Position_ID(), null);
                        if (Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) > 0)
                        {
                            string sql = "SELECT SUM(VAHRUAE_ApprovedVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='CO' and vac.VAB_Position_ID = " + GetVAB_Position_ID() +
                         " AND VAF_Client_ID = " + GetVAF_Client_ID();
                            int ApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                            sql = "SELECT SUM(VAHRUAE_ApprovableVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='IP' and vac.VAB_Position_ID=" + GetVAB_Position_ID() +
                                  " AND VAF_Client_ID = " + GetVAF_Client_ID();
                            int InApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if ((Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - (ApprVac + InApprVac)) <= 0)
                            {
                                SetVAB_Position_ID(0);
                                log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                return false;
                            }
                        }
                        else
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                            return false;
                        }
                    }
                    else
                    {
                        if (Is_ValueChanged("VAB_Position_ID"))
                        {
                            X_VAB_Position job1 = new X_VAB_Position(GetCtx(), GetVAB_Position_ID(), null);
                            if (Util.GetValueOfInt(job1.Get_Value("VAHRUAE_FreeSeats")) > 0)
                            {
                                string sql = "SELECT SUM(VAHRUAE_ApprovedVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='CO' and vac.VAB_Position_ID = " + GetVAB_Position_ID() +
                         " AND VAF_Client_ID = " + GetVAF_Client_ID();
                                int ApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                                sql = "SELECT SUM(VAHRUAE_ApprovableVacancy) FROM VAHRUAE_Vacancy vac WHERE vac.DocStatus='IP' and vac.VAB_Position_ID=" + GetVAB_Position_ID() +
                                      " AND VAF_Client_ID = " + GetVAF_Client_ID();
                                int InApprVac = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if ((Util.GetValueOfInt(job1.Get_Value("VAHRUAE_FreeSeats")) - (ApprVac + InApprVac)) <= 0)
                                {
                                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                    return false;
                                }
                            }
                            else
                            {
                                log.SaveError("Error", Msg.GetMsg(GetCtx(), "VAHRUAE_NoFreeSeat"));
                                return false;
                            }
                            X_VAB_Position job = new X_VAB_Position(GetCtx(), Util.GetValueOfInt(Get_ValueOld("VAB_Position_ID")), null);
                            job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) + 1);
                            job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) - 1);
                            job.Save();
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// After save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return false;

            StringBuilder _sql = new StringBuilder("");

            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_BP_Customer_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Customer_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Customer_Acct"));
            int countC = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countC > 0)
            {
                if (IsCustomer())
                {
                    PO obj = null;
                    //MFRPTBPCustomerAcct obj = null;
                    int Customer_ID = GetVAB_BusinessPartner_ID();
                    int VAB_BPart_Category_ID = GetVAB_BPart_Category_ID();
                    string sql = "SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where   r.name='FRPT_RelatedTo' and l.name='Customer'";
                    //string sql = "select VALUE from VAF_CtrlRef_List where name='Customer'";
                    string _RelatedToCustmer = Convert.ToString(DB.ExecuteScalar(sql));

                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Customer_Acct   where VAB_BusinessPartner_ID=" + Customer_ID + " AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {

                        _sql.Clear();
                        _sql.Append("Select  BPG.VAB_AccountBook_id, BPG.VAB_Acct_ValidParameter_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.VAB_BPart_Category_ID=" + VAB_BPart_Category_ID + " and ACC.frpt_relatedto='" + _RelatedToCustmer + "' AND BPG.IsActive = 'Y' AND BPG.VAF_Client_ID = " + GetVAF_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPCustomerAcct(GetCtx(), 0, null);
                                obj = MVAFTableView.GetPO(GetCtx(), "FRPT_BP_Customer_Acct", 0, null);
                                obj.Set_ValueNoCheck("VAB_BusinessPartner_ID", Customer_ID);
                                obj.Set_ValueNoCheck("VAF_Org_ID", 0);
                                obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]));
                                obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            _sql.Clear();
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_BP_Vendor_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Vendor_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Vendor_Acct"));
            int countV = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countV > 0)
            {
                if (IsVendor())
                {
                    PO obj = null;
                    //MFRPTBPVendorAcct obj = null;
                    int Vendor_ID = GetVAB_BusinessPartner_ID();
                    int VAB_BPart_Category_ID = GetVAB_BPart_Category_ID();
                    string sql = "SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where   r.name='FRPT_RelatedTo' and l.name='Vendor'";
                    //string sql = "select VALUE from VAF_CtrlRef_List where name='Vendor'";
                    string _RelatedToVendor = Convert.ToString(DB.ExecuteScalar(sql));
                    //string _RelatedToVendor = X_FRPT_AcctDefault.FRPT_RELATEDTO_Vendor.ToString();


                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Vendor_Acct   where VAB_BusinessPartner_ID=" + Vendor_ID + " AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {
                        _sql.Clear();
                        _sql.Append("Select  BPG.VAB_AccountBook_id, BPG.VAB_Acct_ValidParameter_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.VAB_BPart_Category_ID=" + VAB_BPart_Category_ID + " and ACC.frpt_relatedto='" + _RelatedToVendor + "' AND BPG.IsActive = 'Y' AND BPG.VAF_Client_ID = " + GetVAF_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPVendorAcct(GetCtx(), 0, null);
                                obj = MVAFTableView.GetPO(GetCtx(), "FRPT_BP_Vendor_Acct", 0, null);
                                obj.Set_ValueNoCheck("VAB_BusinessPartner_ID", Vendor_ID);
                                obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]));
                                obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));

                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            _sql.Clear();
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_BP_Employee_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BP_Employee_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BP_Employee_Acct"));
            int countE = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (countE > 0)
            {

                if (IsEmployee())
                {
                    // MFRPTBPEmployeeAcct obj = null;
                    int Employee_ID = GetVAB_BusinessPartner_ID();
                    int VAB_BPart_Category_ID = GetVAB_BPart_Category_ID();
                    string sql = "SELECT L.VALUE FROM VAF_CTRLREF_LIST L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where   r.name='FRPT_RelatedTo' and l.name='Employee'";
                    //string sql = "select VALUE from VAF_CtrlRef_List where name='Employee'";
                    string _RelatedToEmployee = Convert.ToString(DB.ExecuteScalar(sql));
                    //string _RelatedToEmployee = X_FRPT_AcctDefault.FRPT_RELATEDTO_Employee.ToString();


                    _sql.Clear();
                    _sql.Append("Select Count(*) From FRPT_BP_Employee_Acct  where VAB_BusinessPartner_ID=" + Employee_ID + " AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID());
                    int value = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                    if (value < 1)
                    {
                        _sql.Clear();
                        _sql.Append("Select  BPG.VAB_AccountBook_id, BPG.VAB_Acct_ValidParameter_id, BPG.frpt_acctdefault_id From FRPT_BP_Group_Acct  BPG inner join frpt_acctdefault ACC ON ACC.frpt_acctdefault_id= BPG.frpt_acctdefault_id where BPG.VAB_BPart_Category_ID=" + VAB_BPart_Category_ID + " and ACC.frpt_relatedto='" + _RelatedToEmployee + "' AND BPG.IsActive = 'Y' AND BPG.VAF_Client_ID = " + GetVAF_Client_ID());
                        DataSet ds = DB.ExecuteDataset(_sql.ToString());
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                //obj = new MFRPTBPEmployeeAcct(GetCtx(), 0, null);
                                var obj = MVAFTableView.GetPO(GetCtx(), "FRPT_BP_Employee_Acct", 0, null);
                                obj.Set_ValueNoCheck("VAB_BusinessPartner_ID", Employee_ID);
                                obj.Set_ValueNoCheck("VAB_AccountBook_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_AccountBook_ID"]));
                                obj.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_ID"]));
                                obj.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                if (!obj.Save())
                                {
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // work done by Amit -- skip inserting default accounting on save of new record
                if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
                {
                    //	Accounting
                    bool sucs = Insert_Accounting("C_BP_Customer_Acct", "VAB_BPart_Category_Acct",
                           "p.VAB_BPart_Category_ID=" + GetVAB_BPart_Category_ID());
                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }

                    sucs = Insert_Accounting("C_BP_Vendor_Acct", "VAB_BPart_Category_Acct",
                           "p.VAB_BPart_Category_ID=" + GetVAB_BPart_Category_ID());

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }


                    sucs = Insert_Accounting("C_BP_Employee_Acct", "VAB_AccountBook_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }
                }

                //	Value/Name change
                if (success && !newRecord
                    && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                    MVABAccount.UpdateValueDescription(GetCtx(), "VAB_BusinessPartner_ID=" +
                        GetVAB_BusinessPartner_ID(), Get_TrxName());
            }
            //Added by Neha Thakur--05 Jan 2018--Set "Report To" (from Header tab) as Supervisor in Login User tab--Asked by Ravikant
            if (IsEmployee())
            {
                int ModuleId = Util.GetValueOfInt(DB.ExecuteScalar("select VAF_ModuleInfo_id from VAF_ModuleInfo where prefix='VAHRUAE_' and isactive='Y'"));
                if (ModuleId > 0)
                {
                    int _Emp_ID = Util.GetValueOfInt(Get_Value("VAHRUAE_HR_Employee"));
                    if (_Emp_ID > 0)
                    {
                        _sql.Clear();
                        _sql.Append(@"SELECT VAF_USERCONTACT_ID FROM VAF_USERCONTACT WHERE VAB_BUSINESSPARTNER_ID=" + _Emp_ID + " AND VAF_CLIENT_ID =" + GetVAF_Client_ID());
                        int VAF_UserContact_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString(), null, null));
                        _sql.Clear();
                        if (VAF_UserContact_ID > 0)
                        {
                            _sql.Append(@"UPDATE VAF_USERCONTACT SET SUPERVISOR_ID=" + VAF_UserContact_ID + " WHERE VAB_BUSINESSPARTNER_ID=" + GetVAB_BusinessPartner_ID());

                        }
                        else
                        {
                            _sql.Append(@"UPDATE VAF_USERCONTACT SET SUPERVISOR_ID=null WHERE VAB_BUSINESSPARTNER_ID=" + GetVAB_BusinessPartner_ID());
                        }
                        int _count = Util.GetValueOfInt(DB.ExecuteQuery(_sql.ToString(), null, null));
                        _sql.Clear();
                    }
                    // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
                    if (newRecord)
                    {
                        X_VAB_Position job = new X_VAB_Position(GetCtx(), GetVAB_Position_ID(), null);
                        job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - 1);
                        job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) + 1);
                        job.Save();
                    }
                    else
                    {
                        if (Is_ValueChanged("VAB_Position_ID"))
                        {
                            X_VAB_Position job = new X_VAB_Position(GetCtx(), GetVAB_Position_ID(), null);
                            job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) - 1);
                            job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) + 1);
                            job.Save();
                        }
                    }
                }

            }

            int count = DB.ExecuteQuery("UPDATE VAB_BPart_Location SET CreditStatusSettingOn = '" + GetCreditStatusSettingOn() + "' WHERE VAB_BusinessPartner_ID = " + GetVAB_BusinessPartner_ID(), null, Get_Trx());

            //---------End----------------------------------------------------
            return success;
        }

        /// <summary>
        /// Check whether credit Validation matches against the Credit Validation,
        /// set either on Business Partner header Or Locaion based on the settings
        /// </summary>
        /// <param name="TrxType">Credit Validation type to match</param>
        /// <param name="VAB_BPart_Location_ID">Business Partner Locaion ID</param>
        /// <returns>True/False</returns>
        public bool ValidateCreditValidation(string TrxType, int VAB_BPart_Location_ID)
        {
            if (GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                if (TrxType.Contains(GetCreditValidation()))
                    return true;
            }
            else if (VAB_BPart_Location_ID > 0)
            {
                MVABBPartLocation loc = new MVABBPartLocation(GetCtx() , VAB_BPart_Location_ID, Get_Trx());
                if (TrxType.Contains(loc.GetCreditValidation()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// function to check credit limit and other validations for Business partner,
        /// based on the settings on Business partner 
        /// </summary>
        /// <param name="VAB_BPart_Location_ID">Business partner location ID</param>
        /// <param name="Amt">Amount</param>
        /// <param name="retMsg">Return Message</param>
        /// <returns>Status True/False (whether credit allowed or not)</returns>
        public bool IsCreditAllowed(int VAB_BPart_Location_ID, Decimal? Amt, out string retMsg)
        {
            Decimal? totOpnBal = 0;
            Decimal? crdLmt = 0;
            retMsg = "";
            string creditStatus = GetSOCreditStatus();
            if (GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                //Credit Limit by Vivek on 30/09/2016
                if (X_VAB_BusinessPartner.SOCREDITSTATUS_CreditStop.Equals(GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = " 
                        + GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + GetSO_CreditLimit();
                    return false;
                }
                if (X_VAB_BusinessPartner.SOCREDITSTATUS_CreditHold.Equals(GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditHold") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = " 
                        + GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + GetSO_CreditLimit();
                    return false;
                }
                totOpnBal = GetTotalOpenBalance();
                crdLmt = GetSO_CreditLimit();
            }
            else if (VAB_BPart_Location_ID > 0 && GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            {
                MVABBPartLocation bploc = new MVABBPartLocation(GetCtx(), VAB_BPart_Location_ID, Get_Trx());
                creditStatus = bploc.GetSOCreditStatus();
                if (X_VAB_BusinessPartner.SOCREDITSTATUS_CreditStop.Equals(bploc.GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = " 
                        + bploc.GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + bploc.GetSO_CreditLimit();
                    return false;
                }
                if (X_VAB_BusinessPartner.SOCREDITSTATUS_CreditHold.Equals(bploc.GetSOCreditStatus()))
                {
                    retMsg = Msg.GetMsg(GetCtx(), "BPartnerCreditHold") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = " 
                        + bploc.GetTotalOpenBalance()
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + bploc.GetSO_CreditLimit();
                    return false;
                }
                totOpnBal = bploc.GetTotalOpenBalance();
                crdLmt = bploc.GetSO_CreditLimit();
            }
            // check for payment if Total Open Balance + payment Amount exceeds Credit limit do not allow to complete transaction
            if ((creditStatus != X_VAB_BusinessPartner.SOCREDITSTATUS_NoCreditCheck) && (crdLmt > 0) && (crdLmt < (totOpnBal + Amt)))
            {
                retMsg = Msg.GetMsg(GetCtx(), "VIS_CreditLimitExceed") + " - " + Msg.Translate(GetCtx(), "TotalOpenBalance") + " = " 
                        + totOpnBal + ", " + Msg.GetMsg(GetCtx(), "BPartnerCreditStop") + " = " + Amt 
                        + ", " + Msg.Translate(GetCtx(), "SO_CreditLimit") + " = " + crdLmt;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether Business partner credit stauts is on Credit Watch
        /// </summary>
        /// <param name="VAB_BPart_Location_ID">Business Partner Location ID</param>
        /// <returns></returns>
        public bool IsCreditWatch(int VAB_BPart_Location_ID)
        {
            if (GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerHeader)
            {
                if (GetSOCreditStatus() == X_VAB_BusinessPartner.SOCREDITSTATUS_CreditWatch)
                    return true;
            }
            else if (GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            {
                MVABBPartLocation bploc = new MVABBPartLocation(GetCtx(), VAB_BPart_Location_ID, Get_Trx());
                if (bploc.GetSOCreditStatus() == X_VAB_BusinessPartner.SOCREDITSTATUS_CreditWatch)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("C_BP_Customer_Acct")
                && Delete_Accounting("C_BP_Vendor_Acct")
                && Delete_Accounting("C_BP_Employee_Acct");
        }
        // change done by mohit to handle the free seats and filled seats on creation and deletion of employee from employee master window.- asked by ravikant.- 22/01/2018
        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                X_VAB_Position job = new X_VAB_Position(GetCtx(), GetVAB_Position_ID(), null);
                job.Set_Value("VAHRUAE_FreeSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FreeSeats")) + 1);
                job.Set_Value("VAHRUAE_FilledSeats", Util.GetValueOfInt(job.Get_Value("VAHRUAE_FilledSeats")) - 1);
                job.Save();
            }
            return success;
        }
    }
}