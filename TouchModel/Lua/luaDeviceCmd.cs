using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel.Lua
{
    /// <summary>
    /// 和手机脚本请求有关的任务Model
    public class luaDeviceCmd
    {
        public luaDeviceCmd()
        {
            this.type = luaCmdType.wait.ToString();
            this.text = "";
            this.title = "等待命令";
            this.status = "等待命令";
            this.lastDate = DateTime.Now;
        }
        public string type { get; set; }

        /// <summary>
        /// 命令的文件名或文本
        /// </summary>
        public string text { get; set; }
        public string title { get; set; }
        public string status { get; set; }
        public DateTime lastDate { get; set; }
    }

    public enum luaCmdType
    {
        wait,
        cmd,
        text,
        exit,
        time
    }
}
