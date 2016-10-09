using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    public class deviceClient
    {
        public string deviceid { get; set; }
        public string tsversion { get; set; }
        public string ip { get; set; }
        public string usbip { get; set; }
        public string port { get; set; }
        public string devname { get; set; }
        public string osType { get; set; }
    }

    public class devices : Entity
    {
        public string deviceid { get; set; }
        public string tsversion { get; set; }
        public string ip { get; set; }
        public string usbip { get; set; }
        public string port { get; set; }
        public string devname { get; set; }

        public string osType { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public int sortcode { get; set; }
        public string luapath { get; set; }
    }
}
