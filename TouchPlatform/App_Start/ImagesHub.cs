using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TouchPlatform
{
    [HubName("HubImages")]
    public class ImagesHub : Hub
    {
        public void sendMessage(string deviceid, string data)
        {
            System.Diagnostics.Debug.WriteLine("客户端发送消息：" + data);
            switch (data)
            {
                #region 可以直接传递图片字节
                //case "image":
                //    {
                //        int orient = 0;
                //        decimal compress = 0.1M;
                //        string ext = "jpg";
                //        TouchSpriteService.authActionService service = new TouchSpriteService.authActionService();
                //        byte[] bytes = service.snapshot(deviceid, ext, compress, orient);
                //        if (bytes.Length == 0)
                //        {
                //            Clients.All.receiveImage(deviceid, "end", "");
                //            break;
                //        }
                //        //压缩图片
                //        //byte[] buffer = TouchSpriteService.Common.ImageHelper.PercentImage(bytes, 0.2D);
                //        byte[] buffer = TouchSpriteService.Common.ImageHelper.GetReducedImage(bytes, 240, 426);

                //        string base64 = "data:image/png;base64," + Convert.ToBase64String(buffer);

                //        //string base64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAQAAAADCAIAAAA7ljmRAAAAGElEQVQIW2P4DwcMDAxAfBvMAhEQMYgcACEHG8ELxtbPAAAAAElFTkSuQmCC";

                //        string status = "ing";
                //        for (int i = 0; i <= base64.Length; i = i + 500)
                //        {
                //            if (i + 500 < base64.Length)
                //            {
                //                string tempSplit = base64.Substring(i, 500);
                //                Clients.All.receiveImage(deviceid, status, tempSplit);
                //            }
                //            else
                //            {
                //                string tempSplit = base64.Substring(i, base64.Length - i);
                //                Clients.All.receiveImage(deviceid, "end", tempSplit);
                //            }
                //        }
                //        break;
                //    } 
                #endregion

                case "FullImage":
                    {
                        int orient = 0;
                        decimal compress = 0.1M;
                        string ext = "jpg";
                        TouchSpriteService.authActionService service = new TouchSpriteService.authActionService();
                        byte[] bytes = service.snapshot(deviceid, ext, compress, orient);
                        if (bytes.Length == 0)
                        {
                            Clients.All.receiveImage(deviceid,bytes);
                            break;
                        }
                        //压缩图片
                        //byte[] buffer = TouchSpriteService.Common.ImageHelper.PercentImage(bytes, 0.2D);
                        byte[] buffer = TouchSpriteService.Common.ImageHelper.GetReducedImage(bytes, 240, 426);

                        Clients.Caller.receiveImage(deviceid, buffer);

                        break;
                    }
                default:
                    Clients.All.receiveMessage(deviceid, data);
                    break;
            }
        }

        // 发送图片
        public void sendImage(string name, IEnumerable<byte[]> images)
        {
            foreach (var item in images ?? Enumerable.Empty<byte[]>())
            {
                if (item.Length == 0) continue;
                Clients.All.receiveImage(name, ""); // 调用客户端receiveImage方法将图片进行显示
            }
        }


        public override Task OnConnected()
        {
            System.Diagnostics.Debug.WriteLine("客户端连接成功");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            System.Diagnostics.Debug.WriteLine("客户端下线成功");
            return base.OnDisconnected(true);
        }

    }
}