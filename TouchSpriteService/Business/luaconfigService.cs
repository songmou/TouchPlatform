using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchModel;

namespace TouchSpriteService.Business
{
    public class luaconfigService
    {
        private string querySQL = "select * from luaconfig where 1=1 ";
        public List<luaconfig> GetList(string where = "", int top = 100)
        {
            ViewReflector<luaconfig> service = new ViewReflector<luaconfig>();

            string sql = querySQL;
            if (top != 0) sql = string.Format("select top {0} * from luaconfig where 1=1 ", top);
            var list = service.GetList(sql + where + " order by luatype,sortcode,ID");

            return list;
        }
        public List<luaconfig> GetPageList(string where, out int Total, int pageSize = 100, int pageIndex = 1)
        {
            ViewReflector<luaconfig> service = new ViewReflector<luaconfig>();

            string sql = @"
select * from(
	select *, ROW_NUMBER() over(order by luaType,sortcode,ID) as ranks from luaconfig
	where 1=1 {0}) as tb where tb.ranks between @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex";
            sql = string.Format(sql, where);

            System.Data.SqlClient.SqlParameter[] commandParameters = {
                new System.Data.SqlClient.SqlParameter("@pageSize", pageSize),
                new System.Data.SqlClient.SqlParameter("@pageIndex", pageIndex)
            };

            var list = service.GetList(sql, commandParameters);

            string sqlTotal = "select COUNT(ID) from luaconfig where 1=1 " + where;
            object o = TouchDataLayer.SqlHelper.ExecuteScalar(System.Data.CommandType.Text, sqlTotal, null);
            int.TryParse(o.ToString(), out Total);

            return list;
        }

        public List<luaconfigdevice> GetDetailPageList(string where, out int Total, int pageSize = 100, int pageIndex = 1)
        {
            ViewReflector<luaconfigdevice> service = new ViewReflector<luaconfigdevice>();

            string sql = @"
select d.devname,d.username,d.ip,lc.*  from devices d 
right join (
	select * from(
		select *, ROW_NUMBER() over(order by luaType,sortcode,ID) as ranks from luaconfig
		where 1=1 {0}) as tb where tb.ranks between  @pageSize*(@pageIndex-1) + 1 and @pageSize*@pageIndex
) lc on d.deviceid=lc.deviceId
";
            sql = string.Format(sql, where);

            System.Data.SqlClient.SqlParameter[] commandParameters = {
                new System.Data.SqlClient.SqlParameter("@pageSize", pageSize),
                new System.Data.SqlClient.SqlParameter("@pageIndex", pageIndex)
            };

            var list = service.GetList(sql, commandParameters);

            string sqlTotal = "select COUNT(ID) from luaconfig where 1=1 " + where;
            object o = TouchDataLayer.SqlHelper.ExecuteScalar(System.Data.CommandType.Text, sqlTotal, null);
            int.TryParse(o.ToString(), out Total);

            return list;
        }

        public luaconfig GetConfig(string where = "")
        {
            ViewReflector<luaconfig> service = new ViewReflector<luaconfig>();
            var config = service.GetDetail(querySQL + where);

            return config;
        }
        public luaconfig GetConfig(int ID)
        {
            TouchSpriteService.DataReflector<luaconfig> service = new TouchSpriteService.DataReflector<luaconfig>();
            return service.Get(ID);
        }

        public string GetValue(string luaType, string luaName)
        {
            ViewReflector<luaconfig> service = new ViewReflector<luaconfig>();
            string where = string.Format(" and luaType='{0}' and luaName='{1}'", luaType, luaName);
            var config = service.GetDetail(querySQL + where);

            return config != null ? config.luaValue : "";
        }

        public bool SetValue(string luaType, string luaName, string luaValue)
        {
            ViewReflector<luaconfig> service = new ViewReflector<luaconfig>();
            string Sql = @"UPDATE luaconfig SET luaValue=@luaValue 
                where luaType=@luaType and luaName=@luaName ";

            System.Data.SqlClient.SqlParameter[] commandParameters = {
                new System.Data.SqlClient.SqlParameter("@luaValue", luaValue),
                new System.Data.SqlClient.SqlParameter("@luaType", luaType),
                new System.Data.SqlClient.SqlParameter("@luaName", luaName)
            };

            int r = TouchDataLayer.SqlHelper.excuteSql(Sql, commandParameters);
            return r == 1;
        }

        public bool UpdateConfig(luaconfig config)
        {
            TouchSpriteService.DataReflector<luaconfig> service = new TouchSpriteService.DataReflector<luaconfig>();
            return service.Update(config);
        }

        public bool AddConfig(luaconfig config)
        {
            TouchSpriteService.DataReflector<luaconfig> service = new TouchSpriteService.DataReflector<luaconfig>();
            return service.Add(config);
        }

        public bool AddOrUpdateConfig(string luaType, string luaName, string value)
        {
            TouchSpriteService.DataReflector<luaconfig> service = new TouchSpriteService.DataReflector<luaconfig>();
            string where = " and luaType='{0}' and luaName='{1}'";
            luaconfig config = GetConfig(string.Format(where, luaType, luaName));
            if (config == null)
            {
                config = new luaconfig();
                config.deviceId = "";
                config.luaName = luaName;
                config.luaType = luaType;
                config.sortcode = 1;
                config.status = "";
                config.createdate = config.updatedate = DateTime.Now;
                config.luaValue = value;
                return service.Add(config);
            }
            else
            {
                config.luaValue = value;
                config.updatedate = DateTime.Now;
                return service.Update(config);
            }
        }

        public bool AddOrUpdateConfig(luaconfig model)
        {
            TouchSpriteService.DataReflector<luaconfig> service = new TouchSpriteService.DataReflector<luaconfig>();
            luaconfig config = service.Get(model.ID);
            if (config == null)
            {
                config = new luaconfig();
                config.deviceId = "";
                config.luaName = model.luaName;
                config.luaType = model.luaType;
                config.sortcode = model.sortcode;
                config.status = "";
                config.createdate = config.updatedate = DateTime.Now;
                config.luaValue = model.luaValue;
                return service.Add(config);
            }
            else
            {
                config.luaValue = model.luaValue;
                config.sortcode = model.sortcode;
                config.luaType = model.luaType;
                config.luaName = model.luaName;
                config.status = model.status;
                config.deviceId = model.deviceId;
                config.updatedate = DateTime.Now;
                return service.Update(config);
            }
        }
    }
}
