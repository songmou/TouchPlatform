using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService
{
    public static class ViewerConfig
    {
        public static readonly string devices_SQL = "select * from devices";
        public static readonly string device2Group_SQL =
            @"SELECT devices.ID, devices.deviceid, devices.tsversion, devices.ip, devices.usbip, devices.port, 
            devices.devname, devices.username, devices.osType, devices.status, devices.remark, 
            devices.sortcode,devices.createdate, devices.updatedate,devices.luapath, 
          group_device.groupid, groups.groupname, groups.auth, 
          groups.lastTime, groups.sortcode gsortcode,groups.groupip,groups.groupport
FROM   ((devices LEFT OUTER JOIN
          group_device ON devices.deviceid = group_device.deviceid) LEFT OUTER JOIN
          groups ON groups.ID = group_device.groupid)
WHERE 1=1 ";
        public static readonly string device2Group_SQL_OrderBy = " order by groups.sortcode ,devices.sortcode ";
        //WHERE (group_device.ID IS NOT NULL)";
    }
}
