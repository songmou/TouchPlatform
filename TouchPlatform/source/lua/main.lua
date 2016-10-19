--[[
--清空缓存
function clearCache()
    os.execute("su mobile -c uicache");
end

--删除文件
function delFile(path)
    os.execute("rm -rf "..path);
end

local path="/var/mobile/Media/DCIM/";
local listPath="/var/mobile/Media/PhotoData/Thumbnails/V2/DCIM/";

local thum="/var/mobile/Media/Photos/Thumbs";

delFile(thum);
clearCache();
dialog("删除成功", 5);
mSleep(1000)
]]
