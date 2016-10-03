require("TSLib");	--触动精灵函数扩展库
require("basic");

DeviceId = getDeviceID(); 

pressHomeKey(0);    --Home 键
pressHomeKey(1);

toast("开始执行,粉丝统计");
mSleep(2*radix);


RebootApp("com.tencent.xin");
mSleep(5*radix);

touchDown(9, 234, 1090);
mSleep(49);
touchUp(9, 230, 1090);


--连续3次某个点的颜色符合要求，则判断已经滑到底了
local looper=0;
local pointArray={
	["color1"]=getColor(30,810),
	["color2"]=getColor(30,860),
	["color3"]=getColor(30,910)
};
while looper<4 do
	local slidelen=950;
	touchDown(1, 175, slidelen); --按下
	mSleep(50);

	while slidelen>150 do
		touchMove(1, 180, slidelen-20); --移动
		mSleep(15);
		touchMove(1, 175, slidelen-40); --移动
		mSleep(14);
		
		slidelen=slidelen-40;
	end
	touchUp(1, 180, slidelen);   --抬起
	mSleep(2000);
	
	local currArray={
		["color1"]=getColor(30,810),
		["color2"]=getColor(30,860),
		["color3"]=getColor(30,910)
	};
	if currArray.color1==pointArray.color1 
		and currArray.color2==pointArray.color2
		and currArray.color3==pointArray.color3
	then
		looper=looper+1;
	end
	pointArray=currArray;

end


local whitelist = "1234567890";
local fcount = ocrText(215,975, 270,1000, 20, whitelist);
mSleep(1000);

logs("识别出的字符："..fcount);


if msgcount==nil then
	
end

local sz = require("sz")
local json = sz.json;

local Url=httpUrl.."SetValue?luaType=粉丝人数";
Url=Url.."&luaName="..DeviceId;
Url=Url.."&deviceId="..DeviceId;
Url=Url.."&luaValue="..fcount;

jsonstring=httpGet(Url);


toast("脚本结束运行");
mSleep(1*radix);


openAppBid('com.touchsprite.ios');