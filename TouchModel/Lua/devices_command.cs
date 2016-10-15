using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel.Lua
{
    /// <summary>
    /// 手机要执行的脚本任务表
    /// </summary>
    public class devices_command : Entity
    {
        public string deviceid { get; set; }
        public string luatype { get; set; }
        public string luaname { get; set; }
        public string luatitle { get; set; }
        public string status { get; set; }
    }
}
