using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchModel;

namespace TouchSpriteService.Business
{
    public class deviceService
    {
        Common.SimpleCacheProvider cache = Common.SimpleCacheProvider.GetInstance();

        /// <summary>
        /// 设备信息
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<devices> GetDevices(string where = "")
        {
            if (!string.IsNullOrWhiteSpace(where))
                where = " where 1=1 " + where;
            ViewReflector<devices> service = new ViewReflector<devices>();
            var list = service.GetList(ViewerConfig.devices_SQL + where);

            return list;
        }

        public devices GetDevice(string deviceid)
        {
            TouchSpriteService.DataReflector<devices> service = new TouchSpriteService.DataReflector<devices>();
            var model = service.Get("deviceid", deviceid);
            return model;
        }

        #region 缓存服务
        public List<TouchModel.device2GroupDetail> GetCacheDevicelistDetail(int groupid = 0)
        {
            var key = "guoupid-" + groupid;

            var devicelist = cache.GetCache(key) as List<TouchModel.device2GroupDetail>;
            if (devicelist == null)
            {
                //加载所有的
                string where = groupid == 0 ? "" : ("groups.ID=" + groupid);

                devicelist = GetDevice2GroupDetail(where);
                cache.SetCache(key, devicelist, 10000);
            }
            return devicelist;
        }
        public void SetCacheDevicelistDetail(int groupid, List<TouchModel.device2GroupDetail> list)
        {
            var key = "guoupid-" + groupid;
            cache.SetCache(key, null);
        }

        public devices GetCacheDevice(string deviceid)
        {
            var authDevice = GetCacheDeviceDetail(deviceid);
            return deviceParse(authDevice);
        }

        public device2GroupDetail GetCacheDeviceDetail(string deviceid)
        {
            var authDevice = cache.GetCache(deviceid) as TouchModel.device2GroupDetail;
            if (authDevice == null)
            {
                authDevice = GetDeviceDetail(deviceid);
                cache.SetCache(deviceid, authDevice, 1000);
            }
            return authDevice;
        }

        public devices deviceParse(TouchModel.device2GroupDetail authDevice)
        {
            if (authDevice == null)
                return null;

            var model = new TouchModel.devices();
            model.deviceid = authDevice.deviceid;
            model.username = authDevice.username;
            model.devname = authDevice.devname;
            model.ID = authDevice.ID;
            model.ip = authDevice.ip;
            model.usbip = authDevice.usbip;
            model.osType = authDevice.osType;
            model.port = authDevice.port;
            model.remark = authDevice.remark;
            model.sortcode = authDevice.sortcode;
            model.status = authDevice.status;
            model.tsversion = authDevice.tsversion;
            model.updatedate = authDevice.updatedate;
            model.createdate = authDevice.createdate;
            return model;
        }
        #endregion

        public bool UpdateDevice(devices model)
        {
            TouchSpriteService.DataReflector<devices> service = new TouchSpriteService.DataReflector<devices>();
            return service.Update(model);
        }

        /// <summary>
        /// 设备信息（包含分组）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<device2GroupDetail> GetDevice2GroupDetail(string where = "")
        {
            if (!string.IsNullOrWhiteSpace(where))
                where = " AND " + where;
            ViewReflector<device2GroupDetail> service = new ViewReflector<device2GroupDetail>();
            var list = service.GetList(ViewerConfig.device2Group_SQL + where + ViewerConfig.device2Group_SQL_OrderBy);

            //foreach (var d in list)
            //{
            //    cache.SetCache(d.deviceid, d, 1000);
            //}

            return list;
        }

        /// <summary>
        /// 设备信息（包含分组）
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public device2GroupDetail GetDeviceDetail(string deviceid)
        {
            string where = string.Format(" AND devices.deviceid='{0}'", deviceid);
            ViewReflector<device2GroupDetail> service = new ViewReflector<device2GroupDetail>();
            var model = service.GetDetail(ViewerConfig.device2Group_SQL + where);

            return model;
        }

    }
}
