using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouchDataLayer;

namespace TouchSpriteService
{
    public class ViewReflector<T>
    {
        public List<T> GetList(string sql = "", System.Data.SqlClient.SqlParameter[] commandParameters = null)
        {
            if (sql == "")
            {
                return null;
            }

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            List<T> list = new List<T>();

            System.Data.IDataReader reader = SqlHelper.dataReader(sql, commandParameters);
            while (reader.Read())
            {
                object entity = type.Assembly.CreateInstance(type.FullName);
                T t = (T)entity;
                foreach (PropertyInfo field in properties)
                {
                    string fieldText = field.Name;
                    object fieldValue = reader[fieldText];
                    if (!fieldValue.Equals(System.DBNull.Value))
                        field.SetValue(t, fieldValue);
                }
                list.Add(t);
            }
            return list;
        }

        public T GetDetail(string sql = "")
        {
            if (sql == "")
            {
                return default(T);
            }

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            List<T> list = new List<T>();

            object entity = type.Assembly.CreateInstance(type.FullName);
            T t = (T)entity;
            t = default(T);

            System.Data.IDataReader reader = SqlHelper.dataReader(sql);
            while (reader.Read())
            {
                t = (T)entity;
                foreach (PropertyInfo field in properties)
                {
                    string fieldText = field.Name;
                    object fieldValue = reader[fieldText];
                    if (!fieldValue.Equals(System.DBNull.Value))
                    {
                        field.SetValue(t, fieldValue);
                    }
                }
            }
            return t;
        }


    }
}
