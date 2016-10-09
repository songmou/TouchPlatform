using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    public class Entity
    {
        private int id;

        [DataFields(IsKey = true)]
        public int ID { get { return id; } set { id = value; } }

        public DateTime createdate { get; set; }
        public DateTime updatedate { get; set; }
    }
}
