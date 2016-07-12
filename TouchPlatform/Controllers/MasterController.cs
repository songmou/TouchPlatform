using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchModel;
using TouchSpriteService;

namespace TouchPlatform.Controllers
{
    public class MasterController : Controller
    {
        // GET: Node
        public ActionResult Top()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }


        public ActionResult Menu()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }


        public ActionResult Devicelist()
        {
            DataReflector<devices> service = new DataReflector<devices>();
            var list = service.Get();
            return View(list);
        }
        public ActionResult socket()
        {
            return View();
        }
    }
}