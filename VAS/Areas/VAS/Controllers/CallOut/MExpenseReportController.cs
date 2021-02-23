﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MExpenseReportController:Controller
    {
        /// <summary>
        /// Get the curency of the pricelist
        /// </summary>
        /// <param name="fields">Time expense ID</param>
        /// <returns>Currency ID</returns>
        public JsonResult GetPriceListCurrency(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVASExpenseReport timeExpense = new MVASExpenseReport(ctx, Util.GetValueOfInt(fields), null);
                MVAMPriceList priceList = new MVAMPriceList(ctx, timeExpense.GetVAM_PriceList_ID(), null);
                retJSON = JsonConvert.SerializeObject(priceList.GetVAB_Currency_ID());
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}