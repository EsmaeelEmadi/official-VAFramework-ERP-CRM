﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using System.Web.Hosting;
using System.Web;
using System.Security.Cryptography;
using System.Web.Security;
/* Process: Generate Invitee List 
 * Writer :Arpit Singh
 * Date   : 20/1/12 
 */
namespace ViennaAdvantageServer.Process
{

    class CreateInviteeList : SvrProcess
    {
        HttpApplication app = new HttpApplication();
        // int Record_ID;
        string url = "";
        protected override void Prepare()
        {
            url = GetCtx().GetContext("#ApplicationURL");
            url = url.ToLower();

            url = url.Replace("http://", "");
            url = url.Replace("https://", "");

            //if (url.Contains("https://"))
            //{

            //}
            if (url.Contains("/viennaadvantage.aspx"))
            {
                url = url.Substring(0, url.LastIndexOf("/")).ToString() + "/CampaignInvitee.aspx";
            }
            else
            {
                url = url + "/CampaignInvitee.aspx";
            }
#pragma warning disable 612, 618
            object o = System.Configuration.ConfigurationSettings.AppSettings["IsSSLEnabled"];
#pragma warning restore 612, 618
            if (o != null && o.ToString() == "Y")
            {
                url = "https://" + url;
            }
            else
            {
                url = "http://" + url;
            }


        }

        protected override String DoIt()
        {
            string query = "Select VAB_PromotionTargetList_id from VAB_PromotionTargetList where VAB_Promotion_id=" + GetRecord_ID() + " and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
            IDataReader MainDr = DB.ExecuteReader(query, null, Get_Trx());
            IDataReader dr = null;

            try
            {

                query = "Delete From VAB_InviteeList  where VAB_Promotion_id=" + GetRecord_ID();
                int value = DB.ExecuteQuery(query);

                while (MainDr.Read())
                {
                    query = "Delete From VAB_InviteeList where   ";

                    int id = Util.GetValueOfInt(MainDr[0]);
                    VAdvantage.Model.X_VAB_PromotionTargetList MCapTarget = new VAdvantage.Model.X_VAB_PromotionTargetList(GetCtx(), id, null);

                    if (MCapTarget.GetVAB_MasterTargetList_ID() != 0)
                    {
                        query = "Select VAB_BusinessPartner_ID from C_TargetList where VAB_MasterTargetList_ID=" + MCapTarget.GetVAB_MasterTargetList_ID() + " and VAB_BusinessPartner_ID is not null";
                        dr = DB.ExecuteReader(query, null, Get_Trx());
                        while (dr.Read())
                        {
                            invitee(Util.GetValueOfInt(dr[0]));

                        }
                        dr.Close();

                        query = "Select Ref_BPartner_ID from C_TargetList where VAB_MasterTargetList_ID=" + MCapTarget.GetVAB_MasterTargetList_ID() + " and Ref_BPartner_ID is not null";
                        dr = DB.ExecuteReader(query, null, Get_Trx());
                        while (dr.Read())
                        {
                            invitee(Util.GetValueOfInt(dr[0]));

                        }
                        dr.Close();

                        query = "Select VAB_Lead_ID from C_TargetList where VAB_MasterTargetList_ID=" + MCapTarget.GetVAB_MasterTargetList_ID() + " and VAB_Lead_ID is not null";
                        dr = DB.ExecuteReader(query, null, Get_Trx());
                        while (dr.Read())
                        {
                            string sql = "Select VAB_InviteeList_id from VAB_InviteeList where VAB_Lead_id=" + Util.GetValueOfInt(dr[0]);
                            object Leadid = DB.ExecuteScalar(sql, null, Get_Trx());
                            if (Util.GetValueOfInt(Leadid) == 0)
                            {
                                VAdvantage.Model.X_VAB_Lead lead = new VAdvantage.Model.X_VAB_Lead(GetCtx(), Util.GetValueOfInt(dr[0]), Get_Trx());
                                if (lead.GetVAB_BusinessPartner_ID() != 0)
                                {
                                    invitee(lead.GetVAB_BusinessPartner_ID());

                                }
                                else if (lead.GetRef_BPartner_ID() != 0)
                                {
                                    invitee(lead.GetRef_BPartner_ID());
                                }

                                else if (lead.GetContactName() != null)
                                {

                                    VAdvantage.Model.X_VAB_InviteeList Invt = new VAdvantage.Model.X_VAB_InviteeList(GetCtx(), 0, Get_Trx());
                                    //Invt.SetC_TargetList_ID(Util.GetValueOfInt(dr[0]));
                                    Invt.SetVAB_Promotion_ID(GetRecord_ID());

                                    Invt.SetName(lead.GetContactName());
                                    Invt.SetEMail(lead.GetEMail());
                                    Invt.SetPhone(lead.GetPhone());
                                    Invt.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                                    Invt.SetAddress1(lead.GetAddress1());
                                    Invt.SetAddress1(lead.GetAddress2());
                                    Invt.SetVAB_City_ID(lead.GetVAB_City_ID());
                                    Invt.SetCity(lead.GetCity());
                                    Invt.SetVAB_RegionState_ID(lead.GetVAB_RegionState_ID());
                                    Invt.SetRegionName(lead.GetRegionName());
                                    Invt.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                                    Invt.SetPostal(lead.GetPostal());
                                    // Invt.SetURL(url);
                                    if (!Invt.Save())
                                    {
                                        Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                    }

                                    string ID = Invt.GetVAB_InviteeList_ID().ToString();
                                    string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
                                    string urlFinal = "";
                                    urlFinal = url + "?" + encrypt;
                                    sql = "update VAB_InviteeList set url = '" + urlFinal + "' where VAB_InviteeList_id = " + Invt.GetVAB_InviteeList_ID();
                                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));

                                    //Random rand = new Random();
                                    //String s = "";
                                    //for (int i = 0; i < 9; i++)
                                    //    s = String.Concat(s, rand.Next(10).ToString());
                                    //string urlFinal = "";
                                    //// urlFinal = url + "?" + Invt.GetVAB_InviteeList_ID().ToString();
                                    //urlFinal = url + "?" + s;
                                    ////string urlFinal = "";
                                    ////urlFinal = url + "?" + Invt.GetVAB_InviteeList_ID().ToString();
                                    //Invt.SetURL(urlFinal);
                                    //if (!Invt.Save(Get_Trx()))
                                    //{
                                    //    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                    //}

                                }
                            }

                        }
                        dr.Close();
                    }

                    if (MCapTarget.GetR_InterestArea_ID() != 0)
                    {

                        query = "Select VAB_BusinessPartner_ID from VAR_InterestedUser where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID() + " and VAB_BusinessPartner_ID is not null";
                        dr = DB.ExecuteReader(query, null, Get_Trx());
                        while (dr.Read())
                        {
                            invitee(Util.GetValueOfInt(dr[0]));

                        }
                        dr.Close();

                        //query = "Select VAB_BusinessPartner_ID from C_TargetList where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID();
                        //dr = DB.ExecuteReader(query);
                        //while (dr.Read())
                        //{
                        //    invitee(Util.GetValueOfInt(dr[0]));

                        //}
                        //dr.Close();

                        query = "Select VAB_Lead_ID from vss_lead_interestarea where R_InterestArea_ID=" + MCapTarget.GetR_InterestArea_ID() + " and VAB_Lead_ID is not null";
                        dr = DB.ExecuteReader(query, null, Get_Trx());
                        while (dr.Read())
                        {

                            string sql = "Select VAB_InviteeList_id from VAB_InviteeList where VAB_Lead_id=" + Util.GetValueOfInt(dr[0]);
                            object Leadid = DB.ExecuteScalar(sql, null, Get_Trx());
                            if (Util.GetValueOfInt(Leadid) == 0)
                            {

                                VAdvantage.Model.X_VAB_Lead lead = new VAdvantage.Model.X_VAB_Lead(GetCtx(), Util.GetValueOfInt(dr[0]), Get_Trx());
                                if (lead.GetVAB_BusinessPartner_ID() != 0)
                                {
                                    invitee(lead.GetVAB_BusinessPartner_ID());

                                }
                                else if (lead.GetRef_BPartner_ID() != 0)
                                {
                                    invitee(lead.GetRef_BPartner_ID());
                                }

                                else if (lead.GetContactName() != null)
                                {

                                    VAdvantage.Model.X_VAB_InviteeList Invt = new VAdvantage.Model.X_VAB_InviteeList(GetCtx(), 0, Get_Trx());
                                    //Invt.SetC_TargetList_ID(Util.GetValueOfInt(dr[0]));
                                    Invt.SetVAB_Promotion_ID(GetRecord_ID());
                                    Invt.SetName(lead.GetContactName());
                                    Invt.SetEMail(lead.GetEMail());
                                    Invt.SetPhone(lead.GetPhone());
                                    Invt.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                                    Invt.SetAddress1(lead.GetAddress1());
                                    Invt.SetAddress1(lead.GetAddress2());
                                    Invt.SetVAB_City_ID(lead.GetVAB_City_ID());
                                    Invt.SetCity(lead.GetCity());
                                    Invt.SetVAB_RegionState_ID(lead.GetVAB_RegionState_ID());
                                    Invt.SetRegionName(lead.GetRegionName());
                                    Invt.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                                    Invt.SetPostal(lead.GetPostal());
                                    //Invt.SetURL(url);
                                    if (!Invt.Save(Get_Trx()))
                                    {
                                        Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                    }

                                    string ID = Invt.GetVAB_InviteeList_ID().ToString();
                                    string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
                                    string urlFinal = "";
                                    urlFinal = url + "?" + encrypt;
                                    sql = "update VAB_InviteeList set url = '" + urlFinal + "' where VAB_InviteeList_id = " + Invt.GetVAB_InviteeList_ID();
                                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));


                                    //Random rand = new Random();
                                    //String s = "";
                                    //for (int i = 0; i < 9; i++)
                                    //    s = String.Concat(s, rand.Next(10).ToString());
                                    //string urlFinal = "";
                                    //urlFinal = url + "?" + s;
                                    ////string urlFinal = "";
                                    ////urlFinal = url + "?" + Invt.GetVAB_InviteeList_ID().ToString();
                                    //Invt.SetURL(urlFinal);
                                    //if (!Invt.Save(Get_Trx()))
                                    //{
                                    //    Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
                                    //}


                                }
                            }

                        }
                        dr.Close();
                    }
                }
                MainDr.Close();
            }
            catch
            {
                if (MainDr != null)
                {
                    MainDr.Close();
                    MainDr = null;
                }
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            return Msg.GetMsg(GetCtx(), "InviteeCteationDone");
        }


        public void invitee(int bpid)
        {
            VAdvantage.Model.X_VAB_BusinessPartner bp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), bpid, Get_Trx());
            String query = "Select VAF_UserContact_id from VAF_UserContact where VAB_BusinessPartner_id=" + bpid;
            int AD_Id = Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_Trx()));
            string sql = "Select VAB_InviteeList_id from VAB_InviteeList where VAF_UserContact_id=" + AD_Id + " and VAB_Promotion_id=" + GetRecord_ID();
            object id = DB.ExecuteScalar(sql, null, Get_Trx());
            VAdvantage.Model.X_VAB_InviteeList Invt;
            if (Util.GetValueOfInt(id) != 0)
            {

                Invt = new VAdvantage.Model.X_VAB_InviteeList(GetCtx(), Util.GetValueOfInt(id), Get_Trx());

            }
            else
            {
                Invt = new VAdvantage.Model.X_VAB_InviteeList(GetCtx(), 0, Get_Trx());
                Invt.SetVAF_UserContact_ID(AD_Id);
            }
            // code added by Anuj 22/12/2015
            String query1 = "Select VAB_Address_ID from VAB_BPart_Location where VAB_BusinessPartner_id=" + bpid;
            int _location = Util.GetValueOfInt(DB.ExecuteScalar(query1, null, Get_Trx()));
            if (_location != 0)
            {
                Invt.SetVAB_Address_ID(_location);
            }
            VAdvantage.Model.X_VAF_UserContact user = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), AD_Id, Get_Trx());
            // Commented By Anuj 22/12/2015
            //int BpLoc = user.GetVAB_BPart_Location_ID();
            //if (BpLoc != 0)
            //{
            //    String Sql = "Select VAB_Address_ID From VAB_BPart_Location where VAB_BPart_Location_id=" + BpLoc;
            //    Invt.SetVAB_Address_ID(Util.GetValueOfInt(DB.ExecuteScalar(Sql)));
            //}
            Invt.SetVAB_Promotion_ID(GetRecord_ID());
            Invt.SetName(user.GetName());
            Invt.SetEMail(user.GetEMail());
            Invt.SetPhone(user.GetPhone());
            // Invt.SetURL(url);
            if (!Invt.Save(Get_Trx()))
            {
                Msg.GetMsg(GetCtx(), "InviteeCteationNotDone");
            }

            string ID = Invt.GetVAB_InviteeList_ID().ToString();
            string encrypt = FormsAuthentication.HashPasswordForStoringInConfigFile(ID, "SHA1");
            string urlFinal = "";
            urlFinal = url + "?" + encrypt;
            sql = "update VAB_InviteeList set url = '" + urlFinal + "' where VAB_InviteeList_id = " + Invt.GetVAB_InviteeList_ID();
            int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_Trx()));



        }
    }
}
