using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    public class groups : Entity
    {
        public string groupname { get; set; }
        public string auth { get; set; }
        public DateTime lastTime { get; set; }
        public int sortcode { get; set; }
    }
}
