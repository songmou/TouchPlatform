require("TSLib");
require("basic");

local switch = {
    ["wait"] = function()    -- for case 1
		wait(data);
    end,
    ["cmd"] = function(data)    -- for case 2
		toast("成功获取到命令..."..data.title);
		mSleep(3*radix);
		
		--执行命令
		dofile(luaPath..data.name);
    end,
    ["exit"] = function()    -- for case 3
		toast("命令停止中...");
		mSleep(5*radix);
		lua_exit();
    end,
    ["time"] = function()    -- for case 3
		toast("获取到定时命令...");
		mSleep(10*radix);
    end
}

function wait()
	toast("等待命令中...");
	mSleep(10*radix);
end


WelcomeInfo("主程序...");


local sz = require("sz")
local http = require("szocket.http")

local deviceId = getDeviceID();
local RequestUrl=httpUrl.."getCommandline?deviceid="..deviceId;

while(true) do
	
	local res, code = http.request(RequestUrl);
	if code == 200 then
		local tb = sz.json.decode(res);
		local data=tb.data;
			
		local func= switch[data.type]
		if(func) then
			func(data);
		else                -- for case default
			wait();
		end
	else
		--timeout等网络情况
		dialog(code,0);
	end

end