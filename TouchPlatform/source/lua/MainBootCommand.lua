require("TSLib");	--触动精灵函数扩展库
require("basic");

--开机启动脚本

vibrator();             --振动
mSleep(1000);

mSleep(20*radix);

lockDevice(); 
mSleep(3000);
flag = deviceIsLock();
if flag == 0 then
    dialog("未锁定",3);
else
    unlockDevice(); --解锁屏幕
end
unlockDevice();


mSleep(3000);

WelcomeInfo("开机自启动任务");
mSleep(3000);

setWifiEnable(true);    --打开 Wifi
mSleep(2000);

setCellularDataEnable(false);   --关闭蜂窝网络
mSleep(3000);


vibrator();             --振动
mSleep(1000);

--启动触动精灵
runApp("com.touchsprite.ios");
mSleep(5000);
--启动MyWi7
runApp("com.intelliborn.MyWi");
mSleep(5000);

pressHomeKey(0);    --Home 键
pressHomeKey(1);
