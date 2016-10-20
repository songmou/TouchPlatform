using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Xml.Serialization;
using System.Text;

namespace TouchSpriteService.Common
{
    /// <summary>
    /// IO帮助类
    /// </summary>
    public class IOHelper
    {
        //public static string path = Environment.CurrentDirectory+"\\config\\";
        //public static string path = HttpContext.Current.Request.PhysicalApplicationPath + "\\App_Data\\";
        public static string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";

        /// <summary>
        /// 获得文件物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetMapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else
            {
                return System.Web.Hosting.HostingEnvironment.MapPath(path);
            }
        }

        #region  序列化

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="obj">序列对象</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>是否成功</returns>
        public static bool SerializeToXml(object obj, string filePath)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            bool result = false;

            FileStream fs = null;
            try
            {
                fs = new FileStream(path + filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return result;

        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="type">目标类型(Type类型)</param>
        /// <param name="filePath">XML文件路径</param>
        /// <returns>序列对象</returns>
        public static object DeserializeFromXML(Type type, string filePath)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(path + filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Exists(string fileName)
        {
            if (fileName == null || fileName.Trim() == "")
            {
                return false;
            }

            if (File.Exists(path + fileName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 读文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Read(string fileName)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            if (!Exists(fileName))
            {
                return null;
            }
            //将文件信息读入流中
            using (FileStream fs = new FileStream(path + fileName, FileMode.Open))
            {
                return new StreamReader(fs).ReadToEnd();
            }
        }


        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                FileStream fs = File.Create(path + fileName);
                fs.Close();
                fs.Dispose();
            }
            return true;

        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">文件内容</param>
        /// <returns></returns>
        public static bool Write(string fileName, string content)
        {
            if (content == null)
            {
                return false;
            }

            if (!Exists(fileName))
            {
                CreateFile(fileName);
            }

            //将文件信息读入流中
            using (FileStream fs = new FileStream(path + fileName, FileMode.OpenOrCreate))
            {
                lock (fs)//锁住流
                {
                    if (!fs.CanWrite)
                    {
                        //throw new System.Security.SecurityException("文件fileName=" + fileName + "是只读文件不能写入!");
                        return false;
                    }
                    else
                    {

                        byte[] buffer = Encoding.UTF8.GetBytes(content);
                        fs.Write(buffer, 0, buffer.Length);
                        return true;
                    }
                }
            }
        }

        #endregion

    }
}
