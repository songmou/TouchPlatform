require("TSLib");	--触动精灵函数扩展库
require("basic");

local sz = require("sz");

DeviceId = getDeviceID(); 

function loadconfig()
	--local config=unserialize(readFileString(configPath));
	
	jsonstring=httpGet(httpUrl.."GetConfig?luaType=发送朋友圈");
	local tb = sz.json.decode(jsonstring);
	local config=tb.data;
	
	if config==nil then
		toast("该脚本未配置运行文件，将会加载默认数据");
		mSleep(2*radix);
		
		--return nil;
		
		config={
			["DeviceId"]=DeviceId,
			["Name"]="发送朋友圈",     		--脚本名称
			["InWifi"]="WIFI",				--网络环境
			["RestTime"]=90,				--延迟时间
			["SendMsg"]="您好"
			--["imageArray"]="1"
		}
		--[[writeFileString(configPath,serialize(config))]]
	end
	
	return config;
end