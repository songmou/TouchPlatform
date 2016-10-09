using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService.Model
{
    public class ClientInfo
    {
        public string deviceid { get; set; }
        public string tsversion { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public string devname { get; set; }
        public string osType { get; set; }
    }
}
