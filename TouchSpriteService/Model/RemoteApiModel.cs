using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService.Model
{
    public class getAuth
    {
        public int status { get; set; }
        public string message { get; set; }
        public long time { get; set; }
        public string auth { get; set; }
        public int valid { get; set; }
        public int remainder_token { get; set; }
    }


}
