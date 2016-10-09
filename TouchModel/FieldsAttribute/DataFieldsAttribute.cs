using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchModel
{
    [AttributeUsage(AttributeTargets.All)]
    public class DataFieldsAttribute : Attribute
    {
        private bool isKey = false;
        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }
    }
}
