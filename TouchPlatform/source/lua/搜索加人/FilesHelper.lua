require("TSLib");	--触动精灵函数扩展库
require("basic");

--NeedMod
configPath="/var/mobile/Media/TouchSprite/lua/config.txt";

function loadconfig(DeviceId)
	local config=unserialize(readFileString(configPath));
	if config==nil then
		toast("该脚本未配置运行文件");
		mSleep(2*radix);
		
		--return nil;
		
		config={
			[DeviceId]={
				["DeviceId"]=DeviceId,
				["Name"]="搜索加人",
				["InWifi"]=true,
				["Interval"]=20,
				["RestTime"]=60,
				["PhoneNums"]="18717171225@18000000000@yy138332255",
				["Count"]=2,
				["IsSendMsg"]=true,
				["WelcomeMsg"]="您好",
				["Description"]="分别是设备编号，脚本名称，加人的号码库，加人间隔时间，一次加的数量,是否是WIFI条件下,是否发送验证消息，验证的消息。每个脚本的配置文件都不一样"
			}
		}
		writeFileString(configPath,serialize(config))
		
		return config[DeviceId];

	end


	local deviceInfo=config[DeviceId];
	
	return deviceInfo;
end

