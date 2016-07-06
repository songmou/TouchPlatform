require("TSLib");	--触动精灵函数扩展库
require("basic");
require("FilesHelper");
require("WechatHelper");

pressHomeKey(0);    --Home 键
pressHomeKey(1);
mSleep(2*radix);

local DeviceId = getDeviceID(); 
local loadDevice=loadconfig(DeviceId);

toast("开始执行脚本："..loadDevice.Name);
mSleep(2*radix);

RebootApp("com.tencent.xin");

mSleep(2*radix);
click(84,1080,30);
mSleep(3*radix);

if not multiColor({{587,71,0xffffff},{588,96,0xffffff},{575,85,0xffffff}},fuzzy) then
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



local tbPhoneNums=strSplit(loadDevice.PhoneNums);
for i, v in pairs(tbPhoneNums) do
	
	AddFriendInPage(v,loadDevice.IsSendMsg,loadDevice.WelcomeMsg);
	
	ToastAwait(loadDevice.Interval,"脚本运行等待中");
end


ToastAwait(loadDevice.RestTime,"距离脚本结束");

toast("脚本'"..loadDevice.Name.."'已结束运行");
mSleep(2*radix);