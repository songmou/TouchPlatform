using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouchDataLayer;
using TouchModel;

namespace TouchSpriteService
{
    public class DataReflector<T>
    {

        public bool Add(T t)
        {
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();

            string Sql = string.Format("INSERT INTO {0} (", type.Name);
            foreach (PropertyInfo attr in properties)
            {
                if (isKey(attr))
                {
                    continue;
                }

                Sql += attr.Name + ",";
            }
            Sql = Sql.TrimEnd(',') + ") VALUES(";

            foreach (PropertyInfo attr in properties)
            {
                if (isKey(attr))
                {
                    continue;
                }

                object val = attr.GetValue(t, null);
                if (val is int || val is float || val is double || val is decimal)
                {
                    Sql += val + ",";
                }
                else
                {
                    if (val == null || (val is DateTime && val.Equals(DateTime.MinValue)))
                    {
                        Sql += "NULL,";
                    }
                    else
                    {
                        Sql += string.Format("'{0}',", val);
                    }
                }
            }
            Sql = Sql.TrimEnd(',') + ")";

            int r = SqlHelper.excuteSql(Sql);
            return r == 1;
        }

        public bool Update(T t)
        {
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();
            PropertyInfo KeyProper = null;

            string sql = string.Format("UPDATE {0} SET ", type.Name);
            foreach (var attr in properties)
            {
                if (isKey(attr))
                {
                    KeyProper = attr;
                    continue;
                }

                var val = attr.GetValue(t, null);

                if (val is int || val is float || val is double || val is decimal)
                {
                    sql += string.Format("{0}={1},", attr.Name, val);
                }
                else
                {
                    if (val == null)
                    {
                        sql += string.Format("{0}=NULL,", attr.Name);
                    }
                    else
                    {
                        sql += string.Format("{0}='{1}',", attr.Name, val);
                    }
                }
            }
            sql = sql.TrimEnd(',');

            if (KeyProper == null) return false;
            else
            {
                var val = KeyProper.GetValue(t, null);
                if (val == null) return false;

                sql += string.Format("where {0}={1}", KeyProper.Name, val);
            }

            int r = SqlHelper.excuteSql(sql);
            return r == 1;
        }


        public bool Delete(int ID)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            string sql = string.Format("Delete from {0} where ", type.Name);
            PropertyInfo KeyProper = null;
            foreach (var attr in properties)
            {
                if (isKey(attr))
                {
                    KeyProper = attr;
                    break;
                }
            }

            if (KeyProper == null) return false;
            else
            {
                sql += string.Format("{0}={1}", KeyProper.Name, ID);
            }

            int r = SqlHelper.excuteSql(sql);
            return r == 1;
        }

        public T Get(int ID)
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            string sql = string.Format("select * from {0} where ", type.Name);
            PropertyInfo KeyProper = null;
            foreach (PropertyInfo proper in properties)
            {
                if (isKey(proper))
                {
                    KeyProper = proper;
                    break;
                }
            }

            if (KeyProper == null) return default(T);
            else
            {
                sql += string.Format("{0}={1}", KeyProper.Name, ID);
            }

            System.Data.IDataReader reader = SqlHelper.dataReader(sql);
            object entity = type.Assembly.CreateInstance(type.FullName);
            T t = (T)entity;
            t = default(T);
            while (reader.Read())
            {
                t = (T)entity;
                foreach (PropertyInfo field in properties)
                {
                    string fieldText = field.Name;
                    object fieldValue = reader[fieldText];
                    if (!fieldValue.Equals(System.DBNull.Value))
                        field.SetValue(t, fieldValue);
                }
            }
            return t;
        }

        public T Get(string fName, string fValue)
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            string sql = string.Format("select * from {0} where ", type.Name);

            if (string.IsNullOrWhiteSpace(fName)) return default(T);
            else
            {
                sql += string.Format("{0}='{1}'", fName, fValue);
            }

            System.Data.IDataReader reader = SqlHelper.dataReader(sql);
            object entity = type.Assembly.CreateInstance(type.FullName);
            T t = (T)entity;
            t = default(T);
            while (reader.Read())
            {
                t = (T)entity;
                foreach (PropertyInfo field in properties)
                {
                    string fieldText = field.Name;
                    object fieldValue = reader[fieldText];
                    if (!fieldValue.Equals(System.DBNull.Value))
                        field.SetValue(t, fieldValue);
                }
            }
            return t;
        }


        public List<T> Get(string where = "", string sortFiled = "ID", bool sortASC = true)
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            string sql = string.Format("select * from {0} where 1=1 {1} ORDER BY {2} {3}",
                type.Name,
                where,
                sortFiled,
                sortASC ? "ASC" : "DESC");

            List<T> list = new List<T>();

            System.Data.IDataReader reader = SqlHelper.dataReader(sql);
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

        #region 私有方法
        private bool isKey(PropertyInfo proper)
        {
            bool isKey = false;
            object[] objs = proper.GetCustomAttributes(typeof(DataFieldsAttribute), true);
            foreach (var obj in objs)
            {
                if (obj is DataFieldsAttribute)
                {
                    DataFieldsAttribute attr = (DataFieldsAttribute)obj;
                    if (attr.IsKey)
                    {
                        isKey = true;
                        break;
                    }
                }
            }

            return isKey;
        }
        #endregion

    }
}
