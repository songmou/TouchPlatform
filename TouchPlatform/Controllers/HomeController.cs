using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchModel;

namespace TouchPlatform.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public JsonResult Index()
        {
            //TouchSpriteService.Business.deviceService service = new TouchSpriteService.Business.deviceService();
            //var list=service.GetDevices();

            return Json("欢迎使用中控平台", JsonRequestBehavior.AllowGet);
        }
    }
}