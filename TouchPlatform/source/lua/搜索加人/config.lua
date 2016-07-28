require("TSLib");	--触动精灵函数扩展库
require("basic");

local sz = require("sz");

DeviceId = getDeviceID(); 

function loadconfig()
	--local config=unserialize(readFileString(configPath));
	
	jsonstring=httpGet(httpUrl.."GetConfig?luaType=搜索加人");
	local tb = sz.json.decode(jsonstring);
	local config=tb.data;
	
	if config==nil then
		toast("该脚本未配置运行文件，将会加载默认数据");
		mSleep(2*radix);
		
		--return nil;
		
		config={
			["DeviceId"]=DeviceId,
			["Name"]="搜索加人",     		--脚本名称
			["InWifi"]="WIFI",				--网络环境
			["Interval"]=20,				--加人间隔时间
			["RestTime"]=60,				--延迟时间
			--["PhoneNums"]="18717171225",
			["Count"]=1,					--一次加的数量
			["IsSendMsg"]=true,
			["WelcomeMsg"]="您好"
		}
		--[[writeFileString(configPath,serialize(config))]]
	end
	
	--号码库
	jsonstring=httpGet(httpUrl.."GetPhoneNums?luaType=号码库&deviceId="..DeviceId.."&Count="..config.Count);
	tb = sz.json.decode(jsonstring);
	config.PhoneNums=tb.data.PhoneNums;
	
	
	if config.PhoneNums==nil then
		config.PhoneNums="18717171225";
	end
	
	
	return config;
end

function phonenumSign(phoneNum,status)
	jsonstring=httpGet(httpUrl.."SetStatus?luaType=号码库&luaName=PhoneNums&luaValue="..phoneNum.."&status="..status.."&deviceId="..DeviceId);
	--local tb = sz.json.decode(jsonstring);
end


--判断这个手机号码，是否已经被其他号码添加过
--如果其他微信号添加过返回false，未添加过或者自己添加过返回true
function getDetailStatus(phoneNum)
	
	jsonstring=httpGet(httpUrl.."GetDetail?luaType=号码库&luaName=PhoneNums&luaValue="..phoneNum.."&status=&deviceId="..DeviceId)
	local tb = sz.json.decode(jsonstring);
	if tostring(tb.data)=="null" then
		return false;
	else
		return true;
	end
	
end

--获取随机的数字
function getRandom()
	str =httpGet(httpUrl.."GetRandom?max=2000");
	return tonumber(str);
end








