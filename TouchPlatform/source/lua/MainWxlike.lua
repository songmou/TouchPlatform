require("TSLib");	--触动精灵函数扩展库
require("basic");
require("business");


pressHomeKey(0);    --Home 键
pressHomeKey(1);


local luaName="朋友圈互相点赞";
toast("开始执行脚本："..luaName);
mSleep(2*radix);

--USB模式下Y坐标有40px的高度差
--UsbHeight=getUsbHeight();

RebootApp("com.tencent.xin");

init("0");

mSleep(2*radix);
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

intoShareline();


alike();



mSleep(2*radix);
toast("脚本'"..luaName.."'已结束运行");
mSleep(2*radix);


openAppBid('com.touchsprite.ios');
