﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
   public class ProspectToOpportunity:SvrProcess
    {
        int _VAB_Lead_ID;
        protected override void Prepare()
        {
            
        }

        protected override string DoIt()
        {
            VAdvantage.Model.X_VAB_BusinessPartner partner = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), GetRecord_ID(), null);
            
            string _BPName =partner.GetName();

            string _sql = "Select VAB_Lead_ID From VAB_Lead Where BPName='" + _BPName + "'  AND IsActive = 'Y' AND VAF_Client_ID = " + GetVAF_Client_ID();
             DataSet ds = new DataSet();
                 ds = DB.ExecuteDataset(_sql.ToString());
                 if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                 {
                     for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                     {
                         _VAB_Lead_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Lead_ID"]);
                         VAdvantage.Model.X_VAB_Lead lead = new VAdvantage.Model.X_VAB_Lead(GetCtx(), _VAB_Lead_ID, Get_TrxName());
                         //  lead.GetRef_BPartner_ID()))
                         //int ExCustomer = lead.GetVAB_BusinessPartner_ID();
                         int Pospect = lead.GetRef_BPartner_ID();
                         
                       
                         if (Pospect != 0)
                         {
                             VAdvantage.Model.X_VAB_Project opp = new VAdvantage.Model.X_VAB_Project(GetCtx(), 0, Get_TrxName());
                             opp.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                             opp.SetVAB_BusinessPartnerSR_ID(lead.GetRef_BPartner_ID());
                             opp.SetSalesRep_ID(lead.GetSalesRep_ID());
                             opp.SetDateContract(DateTime.Today);
                             opp.SetVAB_Promotion_ID(lead.GetVAB_Promotion_ID());
                             // opp.SetR_Source_ID (lead.GetR_Source_ID());
                             //opp.SetOpportunityStatus ("N");
                             // opp.SetVAF_Client_ID(GetVAF_Client_ID());
                             opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                             VAdvantage.Model.X_VAB_BusinessPartner bp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), Pospect, Get_TrxName());
                             //X_VAB_BPart_Location loc = new X_VAB_BPart_Location(GetCtx(), Pospect, Get_TrxName());

                             opp.SetName(bp.GetName());
                             opp.SetVAB_BPart_Location_ID(lead.GetVAB_BPart_Location_ID());
                             opp.SetIsOpportunity(true);


                             if (opp.Save())
                             {
                                 lead.SetVAB_Project_ID(opp.GetVAB_Project_ID());
                                 lead.Save();
                                 
                                 bp.SetCreateProject("Y");
                                 if (bp.Save())
                                 { 
                                 }
                                
                                 return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");
                             }
                             else
                             {
                                 return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");
                             }

                         }
                         //if (ExCustomer != 0)
                         //{
                         //    VAdvantage.Model.X_VAB_Project opp = new VAdvantage.Model.X_VAB_Project(GetCtx(), 0, Get_TrxName());
                         //    opp.SetVAB_Lead_ID(lead.GetVAB_Lead_ID());
                         //    opp.SetVAB_BusinessPartner_ID(lead.GetVAB_BusinessPartner_ID());
                         //    opp.SetSalesRep_ID(lead.GetSalesRep_ID());
                         //    opp.SetDateContract(DateTime.Today);
                         //    opp.SetVAB_Promotion_ID(lead.GetVAB_Promotion_ID());
                         //    opp.SetR_Source_ID(lead.GetR_Source_ID());
                         //    opp.SetOpportunityStatus("N");
                         //    opp.SetVAF_UserContact_ID(lead.GetVAF_UserContact_ID());
                         //    VAdvantage.Model.X_VAB_BusinessPartner bp = new VAdvantage.Model.X_VAB_BusinessPartner(GetCtx(), ExCustomer, Get_TrxName());
                         //    VAdvantage.Model.X_VAB_BPart_Location loc = new VAdvantage.Model.X_VAB_BPart_Location(GetCtx(), ExCustomer, Get_TrxName());

                         //    opp.SetName(bp.GetName()); ;
                         //    opp.SetVAB_BPart_Location_ID(lead.GetVAB_BPart_Location_ID());
                         //    opp.SetIsOpportunity(true);

                         //    if (opp.Save())
                         //    {
                         //        lead.SetVAB_Project_ID(opp.GetVAB_Project_ID());
                         //        lead.Save();
                         //        return Msg.GetMsg(GetCtx(), "OpprtunityGenerateDone");

                         //    }
                         //    else
                         //    {
                         //        return Msg.GetMsg(GetCtx(), "OpprtunityGenerateNotDone");

                         //    }
                         //}
                         


                     }

                 }
                 return "";
        }
    }
}
