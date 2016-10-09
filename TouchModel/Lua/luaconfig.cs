using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    public class luaconfig: Entity
    {
        public string deviceId { get; set; }
        public string luaType { get; set; }
        public string luaName { get; set; }
        public string luaValue { get; set; }
        public string status { get; set; }
        public int sortcode { get; set; }

    }

    public class luaconfigdevice : luaconfig
    {
        public string devname { get; set; }
        public string username { get; set; }
        public string ip { get; set; }

    }
}
