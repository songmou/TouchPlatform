using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TouchSpriteService.Common;

namespace TouchPlatform
{
    [HubName("HubImages")]
    public class ImagesHub : Hub
    {
        SimpleCacheProvider cache = SimpleCacheProvider.GetInstance();
        public void sendMessage(string deviceid, string data)
        {
            //System.Diagnostics.Debug.WriteLine("客户端发送消息：" + data);
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
                            Clients.All.receiveImage(deviceid, bytes);
                            break;
                        }
                        //压缩图片
                        //byte[] buffer = TouchSpriteService.Common.ImageHelper.PercentImage(bytes, 0.2D);
                        decimal PixelRatio = 1.0M / 4.0M;
                        byte[] buffer = TouchSpriteService.Common.ImageHelper.GetReducedImage(
                            bytes,
                            (int)(640 * PixelRatio),
                            (int)(1136 * PixelRatio)
                        );

                        Clients.Caller.receiveImage(deviceid, buffer);

                        break;
                    }
                default:
                    Clients.All.receiveMessage(deviceid, data);
                    break;
            }
        }

        /// <summary>
        /// 用WebSocket发送动态命令
        /// </summary>
        /// <param name="devices">需要执行的设备，用逗号分隔</param>
        /// <param name="luaName">执行的脚本名称</param>
        /// <param name="data">传递的数据</param>
        public void sendCommand(string devices, string luaName, string data)
        {
            TouchSpriteService.Business.luaconfigService service = new TouchSpriteService.Business.luaconfigService();
            string StringCmd = "";

            switch (luaName)
            {
                case "Click":
                    //data为点击的坐标
                    string[] arrayPoint = data.Split(',');
                    int x, y;
                    if (arrayPoint.Length == 2 && int.TryParse(arrayPoint[0], out x)
                        && int.TryParse(arrayPoint[1], out y))
                    {
                        StringCmd = string.Format("click({0},{1},30);", x, y);
                        break;
                    }
                    break;
                case "Input":
                    //data为要输入的文本
                    StringCmd = string.Format("inputText('{0}');", data);
                    break;
                default:
                    StringCmd = service.GetValue("动态脚本", luaName);
                    break;

            }

            if (devices != "" && !string.IsNullOrWhiteSpace(StringCmd))
            {
                string[] deviceids = devices.Split(',');
                string where = " and luaType='{0}' and (deviceId='' OR deviceId='{1}') and luaName='{2}' ";
                string luaType = "动态脚本";
                TouchModel.luaconfig command = service.GetConfig(string.Format(where, luaType, "", "command"));
                if (command != null)
                {
                    service.SetValue(luaType, "command", StringCmd);

                    #region 设置执行路径
                    //多任务发送命令
                    var taskSendDevices = new Task<bool>[deviceids.Length];
                    var ActionService = new TouchSpriteService.authActionService();
                    for (int i = 0; i < deviceids.Length; i++)
                    {
                        var deviceid = deviceids[i];

                        taskSendDevices[i] = Task.Run(() =>
                        {
                            //此次需要执行的脚本路径
                            string luaPath = "/var/mobile/Media/TouchSprite/lua/Command/main.lua";

                            //与上次执行路径对比，路径一样就跳过这一步
                            var modelPath = cache.GetCache(deviceid) as TouchModel.device2GroupDetail;
                            if (modelPath != null && modelPath.luapath == luaPath)
                                return true;
                            else
                                return ActionService.setLuaPath(deviceid, luaPath);

                        });
                    }
                    Task.WaitAll(taskSendDevices);
                    #endregion

                    #region 执行脚本
                    //多任务发送命令
                    var taskDevices = new Task<string>[deviceids.Length];
                    for (int i = 0; i < deviceids.Length; i++)
                    {
                        var deviceid = deviceids[i];
                        taskDevices[i] = Task.Run(() =>
                        {
                            return ActionService.Runlua(deviceid); ;
                        });
                    }

                    #endregion
                }
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