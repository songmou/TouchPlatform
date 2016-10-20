require("TSLib");
require("basic");
require("config");
require("business");


pressHomeKey(0);    --Home 键
pressHomeKey(1);

closeApp("com.tencent.xin");

WelcomeInfo("通讯录加人");
--toast("开始执行脚本：通讯录加人");
mSleep(2*radix);

init("0");

local SleepNum=getRandom();
if(SleepNum==nil) then SleepNum=3000; end
mSleep(SleepNum);
AddContacts();
mSleep(8000-SleepNum);

runApp("com.tencent.xin"); 
mSleep(5*radix);

mSleep(1*radix);
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


--点击手机联系人
click(200,781,30);
mSleep(3*radix);



addFriendsByContacts();


toast("脚本结束运行")
mSleep(2*radix);


openAppBid('com.touchsprite.ios');
mSleep(4*radix);
openAppBid("com.tencent.xin");
