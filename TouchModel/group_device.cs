using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    /// <summary>
    /// 设备分组 关系表
    /// </summary>
    public class group_device : Entity
    {
        public string deviceid { get; set; }
        public int groupid { get; set; }
    }

    /// <summary>
    /// 分组的设备信息
    /// </summary>
    public class device2GroupDetail : devices
    {
        public int groupid { get; set; }
        public string groupname { get; set; }
        public string auth { get; set; }
        public DateTime lastTime { get; set; }
    }
}
