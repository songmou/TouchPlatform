require("TSLib");	--触动精灵函数扩展库
require("basic");
require("config");
require("business");

pressHomeKey(0);    --Home 键
pressHomeKey(1);


toast("获取数据中...");
mSleep(2*radix);


--USB模式下Y坐标有40px的高度差
--UsbHeight=getUsbHeight();


--toast("开始执行脚本：搜索加人");
WelcomeInfo("搜索加人");
mSleep(2*radix);

RebootApp("com.tencent.xin");

init("0");

local SleepNum=getRandom();
if(SleepNum==nil) then SleepNum=3000; end
mSleep(SleepNum);

local DeviceId = getDeviceID(); 
local loadDevice=loadsearchconfig();
mSleep(6000-SleepNum);

mSleep(1*radix);
click(84,1080,30);
mSleep(3*radix);

if not multiColor({
		{587,71,0xffffff},
		{588,96,0xffffff},
		{575,85,0xffffff}},fuzzy) then
	toast("主界面异常");
	mSleep(5*radix);
	return ;
end



--点击+
click(586,86,30);
mSleep(2*radix);

--点击添加朋友
click(503,278,30);
mSleep(3*radix);

--点击输入框(进入输入微信号或者手机号的输入框界面)
click(231,202,30);
mSleep(2*radix);



--获取的号码库
local tbPhoneNums=strSplit(loadDevice.PhoneNums);

--一次添加的人数
local count=tonumber(loadDevice.Count);
for i, v in pairs(tbPhoneNums) do
	if(i<=count) then
		AddFriendInPage(v,loadDevice.IsSendMsg,loadDevice.WelcomeMsg);
		ToastAwait(loadDevice.Interval,"脚本运行等待中");
	end
	
end

BackRight(1);
Backer(1);

ToastAwait(loadDevice.RestTime,"距离脚本结束");

toast("脚本'"..loadDevice.Name.."'已结束运行");
mSleep(2*radix);


openAppBid('com.touchsprite.ios');

mSleep(4*radix);
openAppBid("com.tencent.xin");