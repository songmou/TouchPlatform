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


local DeviceId = getDeviceID(); 
local loadDevice=loadconfig("发送朋友圈");

local SleepNum=getRandomTime("8000");
mSleep(SleepNum);

toast("开始执行脚本："..loadDevice.Name);
mSleep(2*radix);

RebootApp("com.tencent.xin");

mSleep(3*radix);
click(84,1080,30);
mSleep(3*radix);

if not multiColor({
		{587,71,0xffffff},
		{588,96,0xffffff},
		{575,85,0xffffff}},fuzzy) 
then
	toast("主界面异常");
	mSleep(5*radix);
	return ;
end

if loadDevice.imageArray==nil then
	toast("纯文本消息");
	mSleep(3*radix);
	
	local shareResult=sharingTxtAction(loadDevice.SendMsg);
else
	local imageArray=strSplit(loadDevice.imageArray,",");

	
	local shareResult=sharingImageAction(
		imageArray,
		loadDevice.SendMsg);
	if(shareResult) then
		toast("朋友圈发送成功");
	else
		toast("朋友圈发送失败");
	end
	mSleep(3*radix);

end

ToastAwait(loadDevice.RestTime,"距离脚本结束");

toast("脚本'"..loadDevice.Name.."'已结束运行");
mSleep(2*radix);


openAppBid('com.touchsprite.ios');
