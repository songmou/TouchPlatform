require("TSLib");	--触动精灵函数扩展库

--全局变量   系统等待时间基数
radix=1*1000;
fuzzy=85;
httpUrl="http://192.168.31.148/lua/";

luaPath="/var/mobile/Media/TouchSprite/lua/";

--脚本执行，提示信息
function WelcomeInfo(msg)
	toast("欢迎使用微领航系统\n即将开始执行脚本\n"..msg);
	mSleep(5*radix);
end


function IsUSBConnect()
	if multiColor({
				{    7,   58, 0x2484e8},
				{  617,   58, 0x2484e8},
				{  312,   37, 0x2484e8},
			})
	then
		return true;
	else
		return false;
	end
	
end


function getUsbHeight()
	if IsUSBConnect()
	then
		return 40;
	else
		return 0;
	end
	
end


--[[
** FUNC 返回上一级（i次）  左侧
]]
function Backer(i)
	local timer=math.floor(i);
	mSleep(0.2*radix);

	for s = 1, timer, 1 do
		--选择地区START
		touchDown(1, 62, 81);
		mSleep(0.01*radix);        --延迟
		touchUp(1, 70, 81);
		mSleep(0.8*radix);
	end
	
end

--[[
** FUNC 返回上一级（i次）  右侧
]]
function BackRight(i)
	local timer=math.floor(i);
	mSleep(0.2*radix);

	for s = 1, timer, 1 do
		--选择地区START
		touchDown(1, 580, 80);
		mSleep(0.01*radix);        --延迟
		touchUp(1, 581, 81);
		mSleep(0.8*radix);
	end
	
	--toast("返回"..timer.."步");
end


--[[
** FUNC 初始化app的主页
** 判断是否在app主页，则返回到app主页
]]
function InitAppHome()
	local timer=1;
	--根据主页多点的颜色判断
	while(timer<=10 and not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy))
	do
		Backer(1);
		timer=timer+1;
		mSleep(0.01*radix);
	end
	
	logs("timer"..timer);
	if(timer>10) then
		toast("可能未登录微信"); 
		logs("可能未登录微信");
		return false;
	else
		toast("已回到首页"); 
		logs("已回到首页");
		return true;
	end
	mSleep(1*radix);
end

--[[
** FUNC 打开app
]]
function openAppBid(AppBidName)
	--[[flag = appIsRunning(AppBidName); --检测app是否在运行
	if flag  == 0 then               --如果没有运行
		runApp(AppBidName)           --运行app
	end
	]]
	
	isfront=isFrontApp(AppBidName);					--  0 == 不在前台运行；1 == 在前台运行
	if isfront == 1 
	then                            				--如果应用处于前台则继续
		toast("微信已经运行"); 
		mSleep(1*radix);
		isfront = isFrontApp(AppBidName);   		--更新前台状态
	else
		r = runApp(AppBidName);    					--启动应用 
		mSleep(5*radix);
	end
	
end

function RebootApp(AppBidName)
	closeApp(AppBidName);
	mSleep(1*radix);
	r = runApp(AppBidName);    --启动应用 
	
	mSleep(5*radix);
	
	if r ~= 0 then
		dialog("应用启动失败",3);
	end
end



--[[
** FUNC 格式化AppIndex
** 返回的格式为“001”、“002”等
]]
function FormatIndex(AppIndex)
	if string.len(AppIndex)  == 1 then 
		return "00"..AppIndex;
	elseif string.len(AppIndex)  == 2 then
		return "0"..AppIndex;
	else
		return AppIndex;
	end
end


--[[
** FUNC 日志记录
]]
function logs(content)
	local date=os.date("%Y-%m-%d %H:%M:%S");
	log(date.."\r\n详情："..content.."\r\n","main");
	--log(content,"i5c.log");
end

function string.split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end


--遍历文件
function getList(path)
    local a = io.popen("ls "..path);
    local f = {};
    for l in a:lines() do
        table.insert(f,l)
    end
    return f
end

--另一个封装函数tap 或者 randomTap 
function click(x,y,time) --封装点击函数,方便调用。
	time=time or 30
	touchDown(1, x, y)
	mSleep(time)
	touchUp(1, x, y)
end

--[[
strSplit("要被分割的字符串",
	"选填，分割的依据，不填写默认'@'",
	"选填，起始分割位置")
	
]]

if string.find(_VERSION, "5.2") then
    table.getn = function (t)
        if t.n then
            return t.n
        else
            local n = 0
            for i in pairs(t) do
                if type(i) == "number" then
                    n = math.max(n, i)
                end
            end
        return n
        end
    end
end

function trim (s) 
    return (string.gsub(s, "^%s*(.-)%s*$", "%1")) 
end



--table 转 string
function serialize(obj)  
    local lua = ""  
    local t = type(obj)  
    if t == "number" then  
        lua = lua .. obj  
    elseif t == "boolean" then  
        lua = lua .. tostring(obj)  
    elseif t == "string" then  
        lua = lua .. string.format("%q", obj)  
    elseif t == "table" then  
        lua = lua .. "{\n"  
    for k, v in pairs(obj) do  
        lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"  
    end  
    local metatable = getmetatable(obj)  
        if metatable ~= nil and type(metatable.__index) == "table" then  
        for k, v in pairs(metatable.__index) do  
            lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"  
        end  
    end  
        lua = lua .. "}"  
    elseif t == "nil" then  
        return nil  
    else  
        error("can not serialize a " .. t .. " type.")  
    end  
    return lua  
end  


--string to table
function unserialize(lua)  
    local t = type(lua)  
    if t == "nil" or lua == "" then  
        return nil  
    elseif t == "number" or t == "string" or t == "boolean" then  
        lua = tostring(lua)  
    else  
        error("can not unserialize a " .. t .. " type.")  
    end  
    lua = "return " .. lua  
    local func = load(lua)  
    if func == nil then  
        return nil  
    end  
    return func()  
end


-- 遍历数组
function IsInTable(value, tbl)
	for k,v in ipairs(tbl) do
	  if v == value then
		return true;
	  end
	end
	return false;
end


--等待提示
function ToastAwait(Interval,msg)
	for s = 0,Interval, 5 do
		local leftSecond=Interval-s+5;
		toast(msg.."...\n 剩余"..leftSecond.."秒");
		mSleep(5*radix);
	end
end


