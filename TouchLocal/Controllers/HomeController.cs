using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TouchLocal.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 本地请求中转
        /// </summary>
        /// <returns></returns>
        public byte[] Index()
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var respone = HttpContext.Response;
            var request = HttpContext.Request;

            //初始化 本应用程序 Url
            string FromUrl = request.Url.ToString();

            //获取转换目标后的Url
            //将请求报文中的 Url 替换为 目标 Url
            //string ToHost = "192.168.20.1:50005";
            if (string.IsNullOrWhiteSpace(Request["host"]))
            {
                return System.Text.Encoding.Default.GetBytes("");
            }
            if (string.IsNullOrWhiteSpace(Request["query"]))
            {
                return System.Text.Encoding.Default.GetBytes("");
            }

            string ToHost = Request["host"].ToString();
            string query = Request["query"].ToString();

            //请求的地址
            string ToUrl = string.Format("http://{0}/{1}", ToHost, query);

            //创建 Http 请求 用于将 替换后 请求报文 发往 目标 Url
            System.Net.HttpWebRequest hRequest = System.Net.HttpWebRequest.CreateHttp(ToUrl);

            //设置请求头
            foreach (var key in request.Headers.AllKeys)
            {
                try
                {
                    hRequest.Headers.Add(key, request.Headers[key]);
                }
                catch (Exception)
                {
                    continue;
                }

            }

            #region 设置特殊请求头
            if (!string.IsNullOrEmpty(request.Headers["Accept"]))
            {
                hRequest.Accept = request.Headers["Accept"];
            }
            if (!string.IsNullOrEmpty(request.Headers["Connection"]))
            {
                string connection = request.Headers["Connection"];
                hRequest.KeepAlive =
                    string.Compare(connection, "keep-alive", StringComparison.CurrentCultureIgnoreCase) == 0;

            }
            if (!string.IsNullOrEmpty(request.Headers["Content-Type"]))
            {
                hRequest.ContentType = request.Headers["Content-Type"];
            }
            if (!string.IsNullOrEmpty(request.Headers["Expect"]))
            {
                hRequest.Expect = request.Headers["Expect"];
            }
            if (!string.IsNullOrEmpty(request.Headers["Date"]))
            {
                hRequest.Date = Convert.ToDateTime(request.Headers["Date"]);
            }
            if (!string.IsNullOrEmpty(request.Headers["Host"]))
            {
                hRequest.Host = ToHost;
            }
            if (!string.IsNullOrEmpty(request.Headers["If-Modified-Since"]))
            {
                hRequest.IfModifiedSince = Convert.ToDateTime(request.Headers["If-Modified-Since"]);
            }
            if (!string.IsNullOrEmpty(request.Headers["Referer"]))
            {
                hRequest.Referer = request.Headers["Referer"].ToString();
            }
            if (!string.IsNullOrEmpty(request.Headers["User-Agent"]))
            {
                hRequest.UserAgent = request.Headers["User-Agent"];
            }
            if (!string.IsNullOrEmpty(request.Headers["Content-Length"]))
            {
                hRequest.ContentLength = Convert.ToInt32(request.Headers["Content-Length"]);
            }
            #endregion

            //判断是否是Get请求,如果不是Get就写入请求报文体
            if (String.Compare(request.HttpMethod, "get", StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                //设置请求体
                hRequest.Method = "POST";
                var hStream = hRequest.GetRequestStream();
                byte[] hbuffer = new byte[1024 * 2];
                int hLength = 0;
                do
                {
                    hLength = Request.InputStream.Read(hbuffer, 0, hbuffer.Length);
                    hStream.Write(hbuffer, 0, hLength);
                } while (hLength > 0);
            }

            //获取响应报文
            System.Net.WebResponse hRespone = null;
            try
            {
                hRespone = hRequest.GetResponse();
            }
            catch (Exception exp)
            {

                respone.Write(exp.Message);
                respone.End();
            }


            //设置响应头
            //this.SetResponeHead(hRespone, respone);
            foreach (var key in respone.Headers.AllKeys)
            {
                try
                {
                    hRespone.Headers.Add(key, respone.Headers[key]);
                }
                catch (Exception)
                {

                    continue;
                }

            }

            //#region 设置特殊响应头
            if (!string.IsNullOrEmpty(hRespone.Headers["Content-Type"]))
            {
                respone.ContentType = hRespone.Headers["Content-Type"];
            }
            if (!string.IsNullOrEmpty(hRespone.Headers["Host"]))
            {
                respone.AddHeader("Host", FromUrl);
            }
            if (!string.IsNullOrEmpty(hRespone.Headers["Referer"]))
            {
                respone.AddHeader("Referer", hRespone.Headers["Referer"]);
            }

            //#endregion

            //写入响应内容
            //this.SetResponeBody(hRespone, respone);
            var nStream = hRespone.GetResponseStream();
            byte[] buffer = new byte[1024 * 2];
            int rLength = 0;
            do
            {
                rLength = nStream.Read(buffer, 0, buffer.Length);
                respone.OutputStream.Write(buffer, 0, rLength);
            } while (rLength > 0);

            respone.End();

            return buffer;
        }
    }
}