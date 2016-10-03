require("TSLib");	--触动精灵函数扩展库
require("basic");

local sz = require("sz");

DeviceId = getDeviceID(); 

--普通获取数据
function loadconfig(luaType)
	--local config=unserialize(readFileString(configPath));
	
	jsonstring=httpGet(httpUrl.."GetConfig?luaType="..luaType);
	local tb = sz.json.decode(jsonstring);
	local config=tb.data;
	
	if config==nil then
		toast("该脚本未配置运行文件");
		mSleep(2*radix);
		
		return nil;
		
		--[[config={
			["DeviceId"]=DeviceId,
			["Name"]="发送朋友圈",     		--脚本名称
			["InWifi"]="WIFI",				--网络环境
			["RestTime"]=90,				--延迟时间
			["SendMsg"]=""
			--["imageArray"]="1"
		}
		]]
		--[[writeFileString(configPath,serialize(config))]]
	end
	
	return config;
end


--获取随机的数字
function getRandomTime(time)
	str =httpGet(httpUrl.."GetRandom?max="..time);
	return tonumber(str);
end
function getRandom()
	--getRandomTime("3000");
	str =httpGet(httpUrl.."GetRandom?max=3000");
	return tonumber(str);
end


--[[
***********************************
***********搜索加人*****************
***********************************
]]

--搜索加人 获取配置项
function loadsearchconfig()
	--local config=unserialize(readFileString(configPath));
	
	jsonstring=httpGet(httpUrl.."GetConfig?luaType=".."搜索加人");
	local tb = sz.json.decode(jsonstring);
	local config=tb.data;
	
	if config==nil then
		toast("该脚本未配置运行文件");
		mSleep(2*radix);
		
		return nil;
		
		--[[config={
			["DeviceId"]=DeviceId,
			["Name"]="搜索加人",     		--脚本名称
			["netWay"]="WIFI",				--网络环境
			["Interval"]=20,				--加人间隔时间
			["RestTime"]=60,				--延迟时间
			--["PhoneNums"]="18717171225",
			["Count"]=1,					--一次加的数量
			["IsSendMsg"]=true,
			["WelcomeMsg"]="您好"
		}
		]]
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





--[[
***********************************
***********通讯录加人*****************
***********************************
]]

function AddContacts()

	removeAllContactsFromAB(); 
	toast("清空通讯录完成");
	mSleep(1*radix);

	local sz = require("sz");
	jsonstring=httpGet(httpUrl.."GetListByDevice?luaType=通讯录&status=&deviceId="..DeviceId.."&pageSize=30");
	tb = sz.json.decode(jsonstring);
	local Contacts=tb.data;


	for i, v in pairs(Contacts) do
		local tempV={lastname="戴",firstname=v.ID,mobile=v.luaValue};
		addContactToAB(tempV);
		configSign(v.ID,"已添加到通讯录")
	end

	toast("通讯录添加完毕")
end


function configSign(ID,status)
	jsonstring=httpGet(httpUrl.."SetStatusByID?ID="..ID.."&status="..status.."&deviceId="..DeviceId);
end


--判断这个手机号码，是否已经被其他号码添加过
--如果其他微信号添加过返回false，未添加过或者自己添加过返回true
function getDetailStatus(ID)
	
	jsonstring=httpGet(httpUrl.."GetDetail?ID="..ID.."&deviceId="..DeviceId)
	local tb = sz.json.decode(jsonstring);
	if tostring(tb.data)=="null" then
		return false;
	else
		return true;
	end
	
end








