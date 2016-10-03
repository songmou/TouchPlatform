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
        public ActionResult Index()
        {
            //TouchSpriteService.Business.deviceService service = new TouchSpriteService.Business.deviceService();
            //var list=service.GetDevices();

            //string base64 = "iVBORw0KGgoAAAANSUhEUgAAAAQAAAADCAIAAAA7ljmRAAAAGElEQVQIW2P4DwcMDAxAfBvMAhEQMYgcACEHG8ELxtbPAAAAAElFTkSuQmCC";
            //string result = "";

            //for (int i = 0; i < base64.Length; i = i + 5)
            //{
            //    if (i + 5 <= base64.Length)
            //    {
            //        string tempSplit = base64.Substring(i, 5);
            //        result += tempSplit;
            //    }
            //    else
            //    {
            //        string tempSplit = base64.Substring(i, base64.Length - i);
            //        result += tempSplit;
            //    }
            //}

            //var screenshot = System.Web.HttpContext.Current.Server.MapPath("~/source/files/screenshot.jpg");
            //byte[] responseBytes = TouchSpriteService.Common.ImageHelper.BytesFromImageFile(screenshot);
            //string base64=Convert.ToBase64String(responseBytes);

            //return "<img src='data:image/png;base64," + base64 + "'>";

            //return Request.UserHostAddress+ Request.ServerVariables["HTTP_VIA"]+"，"
            //    +Request.ServerVariables["HTTP_X_FORWARDED_FOR"]+"，"+
            //    Request.ServerVariables["REMOTE_ADDR"];

            return Redirect("/Assets/index.html");
        }
    }
}