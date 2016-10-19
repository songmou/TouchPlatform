require("TSLib");
require("basic");

--清除所有相册图片，重置相册
local path="/var/mobile/Media/DCIM/";
local ThumbPath="/var/mobile/Media/PhotoData/";

delFile(path);
delFile(ThumbPath);
clearCache();

toast("相册清除成功，系统即将重启",10);

mSleep(3000);
reboot();