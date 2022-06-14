using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;

namespace VIS.Areas.VIS.Controllers
{
    public class CardTemplateMasterController : Controller
    {
        // GET: VIS/CardTemplateMaster
        public ActionResult Index(string windowno)
        {
            ViewBag.WindowNumber = windowno;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                ViewBag.lang = ctx.GetAD_Language();
            }

            return PartialView();
        }
    }
}