using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Authorizes
{
    /// <summary>
    /// 权限拦截器
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //添加跨域请求
            if (filterContext.HttpContext.Request.HttpMethod.ToString().ToUpper() == "OPTIONS")
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                return;
            }
        }
    }
}