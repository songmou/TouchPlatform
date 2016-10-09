using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService.Common
{
    public class EncodeHelper
    {
        /// <summary>
        /// Unicode解码，可以包括其他字符   
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string UnicodeDecoding(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            string outStr = "";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(?i)\\u([0-9a-f]{4})");
            outStr = reg.Replace(str, delegate (System.Text.RegularExpressions.Match m1)
            {
                return ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString();
            });
            return outStr;
        }

        /// <summary>
        /// 将字符串转化为Unicode字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string UnicodeEncode(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    //将中文字符转为10进制整数，然后转为16进制unicode字符  
                    outStr += "\\u" + ((int)str[i]).ToString("x");
                }
            }
            return outStr;
        }
    }
}
