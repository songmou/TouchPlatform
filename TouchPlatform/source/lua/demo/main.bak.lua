require("TSLib");	--触动精灵函数扩展库
require("basic");

require("timeline");
require("wechat");

width, height = getScreenSize();--根据分辨率调用不同的lua文件
if width == 640 and height == 1136 then
										--如果分辨率为640x1136,则5c
else
	toast("当前脚本不支持您的分辨率");
	lua_exit();
end


--setWifiEnable(false);

--要打开的微信，通过需要依次打开，或者通过数组打开
--array=1;				--array={"1","2","3","4"}
--for index=1,array,1 do
array={"com.tencent.xin"}
for index, v in pairs(array) do

	--local AppIndex=FormatIndex(index);							--要执行的微信序号
	--local AppPreBidName="im.pre.";
	--local AppBidName=AppPreBidName..AppIndex;						--字符串连接用..
	local AppBidName=v;
	local AppBidNameCN="微信";

	--toast("打开"..AppBidNameCN);
	openAppBid(AppBidName);
	
	
	local IsHome=InitAppHome();		--初始化到主页
	if not IsHome then				--如果未能成功回到主页，则从新打开
		closeApp(AppBidName);
		
		mSleep(1*1000*radix);
		
		openAppBid(AppBidName);
	end
	mSleep(1*1000*radix);
	
	
	--logs("已初始化"..AppBidName);
	--toast("已初始化"..AppBidNameCN);
	
	--[[click(443,929,300*radix);
	mSleep(1*1000*radix);
	click(601,929,300*radix);]]
	
	
	--DO Action
	
	--朋友圈发送
	
	
	--local shareResult=sharingTxtAction("微信朋友圈文本测试内容");
	--[[local shareResult=sharingImageAction(
		{"404.jpg","header.jpg"},
		"微信朋友圈文本测试内容");
	if(shareResult) then
		toast("朋友圈发送成功");
	end
	mSleep(1*1000*radix);
	]]
	
	
	
	
	--[[--文字识别
	recognize = old_ocrText(159, 49, 455, 110, 1); --OCR 
	mSleep(1000); 
	dialog("识别出的字符："..recognize, 0);
	]]
	
	
	--[[
	--检查聊天消息条数
	local msgcount=CheckMessage();
	if msgcount~="" then
		toast("有"..msgcount.."条微信消息");
	end
	mSleep(1*1000);

	
	--检查好友申请条数
	local applycount=CheckFriendApply();
	if applycount~="" then
		toast("有"..applycount.."条好友申请消息");
	end
	mSleep(1*1000);
	
	--检查朋友圈是否有更新
	local timeline=CheckTimeLine();
	if timeline~="" then
		toast(timeline);
	end
	mSleep(1*1000);
	]]
	
	
	--通过自己输入微信号或者手机号添加好友
	local successline=addFriendsByList(
		{
		"18717171225"
		},
		"您好，我是戴维尼董事长聂文彪，请添加微信为您服务，鞠躬感谢！"
	);
	if successline~="" then
		toast("已成功发送"..successline.."条好友申请");
		logs(AppBidName.." 已成功发送"..successline.."条好友申请");
	end
	mSleep(3*1000);
	
	
	
	
	--writeFileString("/var/mobile/Media/TouchSprite/res/wechat.txt","")
	--[[local successContactline=addFriendsByNewFriends(
		1,
		"您好，我是戴维尼董事长聂文彪，请添加微信为您服务，鞠躬感谢！");
	]]
	
	
	--[[AddFriendsByNewContracts(
		{
		"18717171225"
		}
	);
	]]

	
--[[
	mSleep(1*1000);
	closeApp(AppBidName);
	toast("已关闭"..AppBidNameCN);
	mSleep(2000);
]]

end

mSleep(1*1000*radix);

--setWifiEnable(true);

toast("操作完成");
