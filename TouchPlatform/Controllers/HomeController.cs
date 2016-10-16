using System;
using System.Collections.Generic;
using System.IO;
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


        /// <summary>
        /// 本地请求中转
        /// </summary>
        /// <returns></returns>
        [Authorizes.ApiAuthorize]
        public byte[] RelayAPI()
        {
            var respone = HttpContext.Response;
            var request = HttpContext.Request;

            //初始化 本应用程序 Url
            string FromUrl = request.Url.ToString();

            //获取转换目标后的Url
            //将请求报文中的 Url 替换为 目标 Url
            //string ToHost = "192.168.20.1:50005";
            if (string.IsNullOrWhiteSpace(Request["host"]))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(Request["query"]))
            {
                return null;
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

            var encoding = System.Text.Encoding.UTF8;

            //判断是否是Get请求,如果不是Get就写入请求报文体
            if (String.Compare(request.HttpMethod, "GET", StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                System.IO.Stream reqStream = Request.InputStream;
                byte[] bufferInput = new byte[(int)reqStream.Length];
                reqStream.Read(bufferInput, 0, (int)reqStream.Length);
                string fileContent = encoding.GetString(bufferInput);

                byte[] fileData = bufferInput;

                //Js提交的时候会有WebKitFormBoundary的标记(lua脚本报错)
                //触动精灵脚本有问题，没有处理前端请求的这种情况
                if (fileContent.IndexOf("WebKitFormBoundary") > -1)
                {
                    //MultipartParser parser = new MultipartParser(stream);
                    //if (parser.Success)
                    //{
                    //    // Save the file
                    //    SaveFile(parser.Filename, parser.ContentType, parser.FileContents);
                    //}
                    int delimiterEndIndex = fileContent.IndexOf("\r\n");
                    if (delimiterEndIndex > -1)
                    {
                        string delimiter = fileContent.Substring(0, fileContent.IndexOf("\r\n"));
                        // Look for Content-Type
                        System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                        System.Text.RegularExpressions.Match contentTypeMatch = re.Match(fileContent);

                        // Look for filename
                        re = new System.Text.RegularExpressions.Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                        System.Text.RegularExpressions.Match filenameMatch = re.Match(fileContent);
                        if (contentTypeMatch.Success && filenameMatch.Success)
                        {
                            // Set properties
                            string ContentType = contentTypeMatch.Value.Trim();
                            string Filename = filenameMatch.Value.Trim();

                            int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                            byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                            int endIndex = IndexOf(bufferInput, delimiterBytes, startIndex);

                            int contentLength = endIndex - startIndex;
                            fileData = new byte[contentLength];
                            Buffer.BlockCopy(bufferInput, startIndex, fileData, 0, contentLength);

                        }
                    }

                    hRequest.ContentLength = fileData.Length;
                }


                //设置请求体
                hRequest.Method = "POST";
                var hStream = hRequest.GetRequestStream();
                byte[] hbuffer = fileData;
                hStream.Write(hbuffer, 0, fileData.Length);

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
                return null;
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

        private int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            int index = 0;
            int startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf<byte>(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }


        [Authorizes.ApiAuthorize]
        public byte[] uploadlua(string url, string auth, string filename,
            string root, string path)
        {
            if (string.IsNullOrWhiteSpace(Request["url"]) ||
                string.IsNullOrWhiteSpace(Request["auth"]) ||
                string.IsNullOrWhiteSpace(Request["filename"]) ||
                string.IsNullOrWhiteSpace(Request["root"]) ||
                string.IsNullOrWhiteSpace(Request["path"]) ||
                Request.Files.Count == 0)
            {
                return null;
            }

            HttpPostedFileBase file = Request.Files[0];
            if (file == null)
                return null;
            if (file.ContentLength == 0 || file.ContentLength > 2 * 1024 * 1024)
                return null;

            System.IO.Stream reqStream = file.InputStream;
            byte[] bufferInput = new byte[(int)reqStream.Length];
            reqStream.Read(bufferInput, 0, (int)reqStream.Length);

            string Url = url + "/upload";

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Headers.Add("Content-Type", "touchsprite/uploadfile");
            webClient.Headers.Add("auth", auth);

            webClient.Headers.Add("filename", filename);
            webClient.Headers.Add("root", root);
            webClient.Headers.Add("path", HttpUtility.UrlEncode(path));

            byte[] responseBytes;
            try
            {
                responseBytes = webClient.UploadData(Url, bufferInput);
                return responseBytes;
            }
            catch //(System.Net.WebException ex)
            {
                //return ex.Message;
                return null;
            }

        }
    }
}