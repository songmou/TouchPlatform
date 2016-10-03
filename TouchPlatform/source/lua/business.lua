require("TSLib");	--触动精灵函数扩展库
require("basic");

--[[
******************************************
*************发送朋友圈***START***********
******************************************
]]

--从微信首页进入 朋友圈
function intoShareline()
	if not multiColor({
			{33 ,79  , 0x39383e},
			{106,109 , 0x434246},
			{497,85  , 0x3b3a3f}},fuzzy) 
	then
		toast("不是在微信主页");
		return false;
	end
	
	--点   发现
	touchDown(1, 399, 1075);
	mSleep(3*radix); 
	touchUp(1, 399, 1073);
	mSleep(3*radix); 
	
	--通过朋友圈的图标判断是不是在发现的界面
	if not multiColor({
			{38,199,0xffc817},
			{53,217,0x66d020},
			{56,184,0xfa5452},
			{71,203,0x5283f0}},fuzzy) 
	then
		toast("朋友圈界面异常");
		return false;
	end
	
	--点朋友圈
	touchDown(1, 261, 205);
	mSleep(20); 
	touchUp(1, 260, 205);
	mSleep(1*radix); 
	
	if not multiColor({
			{573,87,0xffffff},
			{608,86,0xffffff},
			{590,69,0xffffff},
			{590,99,0xffffff}},fuzzy) then
		toast("未能成功进入朋友圈");
		return false;
	end
end


--发送朋友圈纯文本内容
function sharingTxtAction(content)
	mSleep(0.5*radix);
	if string.len(content)  == 0 then 
		return false;
	end
	
	intoShareline();
	
	--点朋友圈相机
	touchDown(1, 573, 87);
	mSleep(3*radix); 
	touchUp(1, 573, 85);
	mSleep(2*radix); 
	
	if multiColor({
			{47,333,0xf9f7fa},
			{592,339,0xf9f7fa},
			{309,708,0xaeacaf}},90) 
	then
		--出现提示
		touchDown(1, 321, 752);
		mSleep(20); 
		touchUp(1, 320, 752);
		mSleep(1*radix); 
	end
	
	--输入发送的内容
	inputText(content);
	mSleep(1*radix); 
	
	
	touchDown(1, 346,303);
	mSleep(200);
	touchMove(1, 331,140); 
	mSleep(200);
	touchUp(1, 320, 140);
	mSleep(2*radix);
	
	--需要勾选发送到QQ空间
	if multiColor({
			{35,743,0xc9c9c9},
			{65,743,0xc9c9c9},
			{72,743,0xefeff4}},90) 
	then
		--如果出现qq空间的图标
		--toast("检查到图标");
		click(51,748,40);
		mSleep(1*radix);

	end
	
	
	--发送
	touchDown(1, 587, 83);
	mSleep(20); 
	touchUp(1, 587, 80);
	mSleep(1*radix); 
	
	return true;
end


--发送朋友圈图文内容
function sharingImageAction(urlColl,content)
	mSleep(1*radix);
	if string.len(content)  == 0 then 
		--return false;
		content="";
	end

	mSleep(2*radix);
	
	intoShareline();
	
	--将图片保存在相册
	for i, v in pairs(urlColl) do  
		
		local ResPath="/var/mobile/Media/TouchSprite/lua/images/"..v;
		--toast(ResPath);
		--mSleep(2*radix);
		
		saveImageToAlbum(ResPath);
		mSleep(1*radix);
    end 
	toast("图片保存成功");
	mSleep(1*radix);
	
	
	--点朋友圈相机
	--click(573,87,100);
	--mSleep(4*radix);
	
	init("0", 0);
	luaExitIfCall(true);
	mSleep(768);


	
	--点朋友圈相机
	touchDown(5, 597, 61);
	mSleep(99);
	touchUp(5, 601, 57);
	mSleep(1170);
	
	--如果出现“拍照记录生活” 
	if multiColor({
		{  212,  384, 0x000000},
		{  212,  406, 0x000000},
		{  590,   86, 0x4c4c4c},
		{  275,  401, 0x000000},
	},90) then
		click(328,757,100);
		mSleep(2*radix);
	end
	
	
	--选择手机相册
	--click(320,979,100);
	--mSleep(2*radix);

	touchDown(4, 227, 984);
	mSleep(115);
	touchUp(4, 232, 981);
	
	
	--第一次打开的时候会出现微信请求访问照片的询问
	--点击好
	if multiColor({
		{   46,   90, 0x262628},
		{  569,   83, 0x999999},
		{  446,  717, 0x007aff}
	},90) then
		click(453,721,100);
		mSleep(2*radix);
	end
	
	
	mSleep(2*radix);
	
	--是否出现选择相册（需要点击一次）
	if multiColor({
		{  292,  241, 0xd9d9d9},
		{  431,  241, 0xd9d9d9},
		{  431,  355, 0xd9d9d9},
		{  292,  355, 0xd9d9d9},
	},90) then
		click(325,188,100);
		mSleep(2*radix);
	end

	
	--local list = getList("/var/mobile/Media/DCIM/100APPLE/");
	--local ImgCount=table.getn(list);
	
	local list100 = getList("/var/mobile/Media/DCIM/100APPLE/");
	local list101 = getList("/var/mobile/Media/DCIM/101APPLE/");
	local list102 = getList("/var/mobile/Media/DCIM/102APPLE/");
	local list103 = getList("/var/mobile/Media/DCIM/103APPLE/");
	local list104 = getList("/var/mobile/Media/DCIM/104APPLE/");
	local list105 = getList("/var/mobile/Media/DCIM/105APPLE/");
	
	local ImgCount=table.getn(list100)+table.getn(list101)+table.getn(list102)+table.getn(list103)+table.getn(list104)+table.getn(list105);

	toast(ImgCount);
	mSleep(2*radix);
	
	local ShareCount=table.getn(urlColl);
	
	mSleep(2*radix);

	--index是所有图片的序号
	for index=ImgCount-ShareCount+1,ImgCount,1 do
		
		if ImgCount<=20 then
			local x=127; local y=169;			--小于20张图片时候的第一张图片坐标
			
			if math.floor(index%4)>0 then			--第1-3列的图片
				x=x+(math.floor(index%4)-1)*158;
			else									--第4列的图片			
				x=x+(math.floor(index%4)+4-1)*158;
			end
			y=y+(math.floor((index-1)/4))*158;

			click(x,y,300);
			toast(x..","..y);
			mSleep(1*radix);
			
		else
			local x=601; local y=929;			--大于20张图片时候的最后一张图片坐标
			
			if math.floor(index%4)>0 then			--第1-3列的图片
				x=x-(4-math.floor(index%4))*158;
			else									--第4列的图片			
				x=x-math.floor(index%4)*158;
			end
			--y=y-(math.floor((ImgCount-index)/4))*158;
			y=y-(math.floor((ImgCount-1)/4)-math.floor((index-1)/4))*158;

			click(x,y,300);
			--logs(x..","..y);
			mSleep(2*radix);
			
		end
	
	end

	mSleep(2*radix);

	--点击完成
	if multiColor({
			{540,1091,0x09bb07},
			{504,1091,0x09bb07}}) 
	then
		click(523,1088,300);
		mSleep(1*radix); 
	else
		toast("选择图片异常");
		return false;
	end
	
	mSleep(2*radix);
	
	--输入发送的内容
	mSleep(1.5*radix); 
	click(262,174,300);
	mSleep(2*radix); 
	inputText(content);
	mSleep(2*radix); 
	
	
		
	touchDown(1, 346,303);
	mSleep(200);
	touchMove(1, 331,140); 
	mSleep(200);
	touchUp(1, 320, 140);
	mSleep(2*radix);
	--需要勾选发送到QQ空间
	if multiColor({
			{35,743,0xc9c9c9},
			{65,743,0xc9c9c9},
			{72,743,0xefeff4}},90) 
	then
		--如果出现qq空间的图标
		--toast("检查到图标");
		click(51,748,40);
		mSleep(1*radix);

	end
	
	
	--发送
	click(587,80,300);
	mSleep(1*radix); 

	
	return true;
end






--[[
******************************************
*************搜索加人***START*************
******************************************
]]

function AddFriendInPage(phoneNum,IsSendMsg,WelcomeMsg)
	mSleep(1*radix); 

	--如果不是在输入号码的界面上，则重启应用进入到该界面
	if not multiColor({
			{522,80 ,0xdadbdf},
			{561,74 ,0x06bf04},
			{620,110,0xefeff4}},fuzzy) then
		toast("微信加人界面异常");
		
		RebootApp("com.tencent.xin");
		
		mSleep(3*radix); 
		
		--在主页点击 聊天列表的界面
		mSleep(2*radix);
		click(84,1080,30);
		mSleep(2*radix);

		
		--点击+
		click(586,86,30);
		mSleep(2*radix);

		--点击添加朋友
		click(503,278,30);
		mSleep(3*radix);
		
		--点击输入框(进入输入微信号或者手机号的输入框界面)
		click(231,202,30);
		mSleep(2*radix);
	end
	
	
	--在输入号码的界面上
	if multiColor({
			{522,80 ,0xdadbdf},
			{561,74 ,0x06bf04},
			{620,110,0xefeff4}},fuzzy) then
		--点击“X”去掉输入框的内容
		click(497,83,30);

		--点击输入框获取焦点
		click(197,89,30);
		
		mSleep(1*radix); 
		inputText(tostring(phoneNum));
		mSleep(3*radix); 
		
		
		
		--绿色放大镜（可能搜索不到该人）
		if multiColor({
				{77,155, 0x2ba245},
				{77,225, 0x2ba245},
				{40,189,0x2ba245}},fuzzy) then
			--点击绿色放大镜
			click(231,202,30);
			mSleep(5*radix);
			
			--出现查找失败的情况，点击确认
			if multiColor({
					{336,437,0x999999},
					{326,587,0xefefef},
					{292,648,0x007aff}},fuzzy) then
				click(292,648,30);
				mSleep(2*radix);
				
				--点击X去掉输入的手机号
				click(503,83,30);
				mSleep(2*radix);
				
				toast("手机号查找失败无结果");
				mSleep(1*radix); 
				
				--给号码添加标记
				phonenumSign(phoneNum,"查找失败");
				
				return;
			end
		end
		
		--如果这个号码被添加过，返回
		if not getDetailStatus(phoneNum) then
			return;
		end
		
		
		--进入该微信号的信息页面
		if multiColor({
				{268,78,0xffffff},
				{341,85,0xffffff},
				{374,85,0x3b3a3f}},fuzzy) then
			--点击添加到通讯录按钮（位置不固定）
			if multiColor({{355,875,0x06bf04}},fuzzy) then
				click(355,875,30);
			elseif multiColor({{355,668,0x06bf04}},fuzzy) then
				click(355,668,30);
			elseif multiColor({{355,760,0x06bf04}},fuzzy) then
				click(355,760,30);
			elseif multiColor({{355,940,0x06bf04}},fuzzy) then
				click(355,940,30);
			elseif multiColor({{355,1110,0x06bf04}},fuzzy) then
				click(355,1110,30);
			else
				toast("未找到添加按钮");
				
				--给号码添加标记
				phonenumSign(phoneNum,"未找到添加按钮");
				
			end
			
			phonenumSign(phoneNum,"已点击添加按钮");
			
			mSleep(2*radix);
			
			--发送朋友验证的页面
			if multiColor({
					{570,250,0xcccccc},
					{580,250,0xffffff},
					{589,250,0xcccccc}},fuzzy) 
			then
				click(578,250,30);
				mSleep(5*radix);
				
				inputText(WelcomeMsg);
				mSleep(5*radix); 
				
				--如果这个号码没有被添加过，可以发送好友申请
				if getDetailStatus(phoneNum) then
				
					--如果是绿色的发送按钮，点击
					--[[if multiColor({
							{595,81,0x20d81f},
							{567,83,0x20d81f}},fuzzy) then]]
					if true then
						--点击发送申请按钮
						click(587,82,30);
						mSleep(3*radix);
						
						logs("成功对手机号发送好友申请:"..phoneNum);
						
						toast("成功对手机号"..phoneNum.."发送好友申请");
						mSleep(3*radix);
							
						--给号码添加标记
						phonenumSign(phoneNum,"已发送申请");
					end
				
				end
			end
			
			--如果出现发送失败（太频繁）
			if multiColor({
					{116,483,0xe2e2e3},
					{265,501,0x000000},
					{311,501,0x000000},
					{329,501,0x000000}},fuzzy) then
				--点击确定
				click(337,648,30);
				mSleep(1*radix);
				
				Backer(2);
				
				BackRight(1);
				
				Backer(2);
				
				--跳出循环(不再加人)
				logs("发送频繁,已退出");
				
				--给号码添加标记
				phonenumSign(phoneNum,"发送频繁");
				
				lua_exit(); 
			end
			
			Backer(1);
		else
			--给号码添加标记
			phonenumSign(phoneNum,"查找失败");
		end
			
	end
	
end





--[[
******************************************
*************通讯录加粉***START***********
******************************************
]]


--此次执行已经添加过的联系人
local addlist={};

function scanScreen()
	
	keepScreen(true);   --打开保持屏幕
	
	for y=172,1075,1 do	
		--添加按钮的顶点横坐标，底部横坐标
		local topPointX=565;
		
		--联系人后面的数字编号 左顶点坐标，底部坐标
		local T_Point1={["x"]=144,["y"]=y};
		local T_Point2={["x"]=225,["y"]=y+24};
		
		if multiColor({
			{topPointX , y		, 0x1d9d1c},
			{topPointX , y+59	, 0x1d9d1c},
			{topPointX , y+30	, 0x1aad19}},fuzzy)
		then
			
			local whitelist = "1234567890";
			local valueNo = ocrText(T_Point1.x,T_Point1.y, T_Point2.x,T_Point2.y, 20, whitelist);
			
			toast("找到编号："..valueNo);
			configSign(valueNo,"通讯录已找到")
			mSleep(1*radix);
			
			--排除刚刚已经添加过的
			if not IsInTable(valueNo,addlist) then
				
				click(topPointX,y+30,30);
				mSleep(4*radix);
			
				--发送朋友验证的页面
				if multiColor({{570,250,0xcccccc},{580,250, 0xffffff},{589,250,0xcccccc}},fuzzy) then
					touchDown(9, 605, 235);
					mSleep(110);
					touchUp(9, 609, 238);
					
					mSleep(2*radix);
					
					inputText(valueNo);
					mSleep(5*radix); 
					
					--如果这个号码没有被添加过，可以发送好友申请
					if getDetailStatus(valueNo) then
					
						--如果是绿色的发送按钮，点击
						if multiColor({{595,81,0x20d81f},{567,83,0x20d81f}},fuzzy) then
							--点击发送申请按钮
							--click(587,82,30);
							--mSleep(3*radix);
							Backer(1);
							
							logs("成功对联系人发送好友申请:"..valueNo);
							
							toast("成功发送申请:"..valueNo);
							mSleep(1*radix);
								
							--给号码添加标记
							configSign(valueNo,"已发送申请");
							
							--发送申请的联系人添加到 列表
							table.insert(addlist,valueNo);
						end
					else
						toast("该号码被其他设备添加过，已跳过");
						mSleep(2*radix);
						Backer(1);
					end
				else
					configSign(valueNo,"直接通过")
					Backer(1);
				end
			end
		
			
			
			mSleep(1*radix);
			
		end
		

	end
	keepScreen(false);   --关闭保持屏幕
	
end


function addFriendsByContacts()
	
	if multiColor({
		{   65,  314, 0xffffff},
		{   65,  432, 0xffffff},
		{   65,  525, 0xffffff},
		{   65,  630, 0xffffff},
	},fuzzy) then
		toast("没有匹配到通讯录好友");
		mSleep(1*radix);
		return ;
	end
	

	--连续3次某个点的颜色符合要求，则判断已经滑到底了
	local looper=1;
	local pointArray={
		["color1"]=getColor(30,810),
		["color2"]=getColor(30,860),
		["color3"]=getColor(30,910)
	};

	::scanpoint::
	
	--扫描屏幕
	scanScreen();
	
	mSleep(1*radix);
	while looper<3 do
		local slidelen=1075;
		touchDown(1, 175, slidelen); --按下
		mSleep(50);
		
		while slidelen>128 do
			touchMove(1, 180, slidelen-12); --移动
			mSleep(40);
			touchMove(1, 175, slidelen-8); --移动
			mSleep(50);
			
			slidelen=slidelen-20;
		end
		
		touchMove(1, 175, 128);
		mSleep(150);
		touchUp(1, 180, 128);   --抬起
		mSleep(400);
		
		mSleep(1000);
		
		local currArray={
			["color1"]=getColor(30,810),
			["color2"]=getColor(30,860),
			["color3"]=getColor(30,910)
		};
		if currArray.color1==pointArray.color1 
			and currArray.color2==pointArray.color2
			and currArray.color3==pointArray.color3
		then
			pointArray=currArray;
			looper=looper+1;
		else
			pointArray=currArray;
			--判断上次滑动的页面与这次是否相同（是否到底）
			--如果没有到底，就继续添加通讯录
			goto scanpoint;
		end
		

	end
	
	
end





--[[
******************************************
*************朋友圈点赞***START***********
******************************************
]]


function alike()
	if not multiColor({
		{   26,   84, 0xffffff},
		{  322,   93, 0xffffff},
		{  589,   86, 0xffffff},
	},fuzzy) then
		toast("不是在微信朋友圈界面");
		mSleep(1*radix);
		return false;
	end
	
	touchDown(1, 100, 880); 
	mSleep(300);
	touchMove(1, 102, 560);
	mSleep(450);
	touchMove(1, 100, 360);
	mSleep(300);
	touchMove(1, 102, 228);
	mSleep(600);
	touchUp(1, 100, 228);   --抬起
	
	
	mSleep(1*radix);
	
	--头像顶点的坐标		
	local fisrtPoint={["x"]=0,["y"]=0};	
	--“横线”坐标
	local endPoint={["x"]=0,["y"]=0};
	
	--“横线”上面点赞图标的坐标
	local likePoint={["x"]=0,["y"]=0};
	
	local looper=1;
	
	
	
	--头像的x1坐标，x2坐标
	local h1=18; local h2=101;
	--点赞图标的x1,x2,x3坐标
	local tip1=600; local tip2=608; local tip3=615; local tip4=584;
		
	
	::scantag::
	
	keepScreen(true);   --打开保持屏幕
	
	for y=128,1135,1 do		--从屏幕的开始循环到屏幕的底部
		
		--找到特定的头像
		if fisrtPoint.x==0 --and getColor(h1, y)==0xc68286 and getColor(h2, y)==0x211306 
			and multiColor({
					{h1,y,0xc68286},
					{h2,y,0x211306},
					{70,y,0x1f0e07}},fuzzy)
		then
			fisrtPoint.x=h1; fisrtPoint.y=y;
			toast("找到了头像坐标:"..fisrtPoint.x..","..fisrtPoint.y);
			--logs("找到了头像坐标:"..fisrtPoint.x..","..fisrtPoint.y);
			mSleep(2*radix);
		end
		
		
		----先找到头像，再开找点赞图标
		if fisrtPoint.x~=0 and likePoint.x==0 
			--and getColor(tip1, y)==0x97aad0 
			--and getColor(tip2, y)==0xffffff 
			--and getColor(tip4, y)==0x97aad0 
			and multiColor({
					{tip1,y,0x97aad0},
					{tip2,y,0xffffff},
					{tip3,y,0x97aad0},
					{tip4,y,0x97aad0},
					{587 ,y,0x97aad0}
					},
				fuzzy)
		then
			likePoint.x=tip1; likePoint.y=y;
			toast("找到了点赞坐标:"..likePoint.x..","..likePoint.y);
			--logs("找到了点赞坐标:"..likePoint.x..","..likePoint.y);
			mSleep(2*radix);
		end
		
		
		--先找到头像和点赞图标，再开始找横线
		if fisrtPoint.x~=0 and likePoint.x~=0 and endPoint.x==0 
			--and getColor(h1, y)==0xdfdfdd 
			--and getColor(h2, y)==0xdfdfdd 
			--and getColor(tip1, y)==0xdfdfdd 
			and multiColor({
					{h1,y,0xdfdfdd},
					{h2,y,0xdfdfdd},
					{tip1,y,0xdfdfdd}},
				95)
		then
			endPoint.x=tip1; endPoint.y=y;
			toast("找到了横线坐标:"..endPoint.x..","..endPoint.y);
			--logs("找到了横线坐标:"..endPoint.x..","..endPoint.y);
			mSleep(2*radix);
		end
		
		
	end
	
	keepScreen(false);   --关闭保持屏幕
	
		
	
	--是否找到了点赞按钮
	if likePoint.x~=0 then
		click(likePoint.x,likePoint.y,50);
		mSleep(1*radix);
	
		--判断是否点赞过
		if multiColor({
			{339,likePoint.y,0xffffff},
			{336,likePoint.y,0x4c5154},
			{327,likePoint.y,0xffffff}},fuzzy) then
		
			click(290,likePoint.y,50);
			toast("点赞完毕："..looper);
			mSleep(2*radix);
			
			fisrtPoint.x=0; fisrtPoint.y=0;
			likePoint.x=0; likePoint.y=0;
			--endPoint.x=0; endPoint.y=0;
		end
	end
	
	
	
	--如果找到了头像，没有点赞按钮，滑动头像到顶部
	if fisrtPoint.x~=0 and likePoint.x==0 
		and endPoint.x==0
	then
		local slidelen=fisrtPoint.y;
		touchDown(1, 175, slidelen); --按下
		mSleep(50);
		
		while slidelen>150 do
			touchMove(1, 180, slidelen-20); --移动
			mSleep(40);
			touchMove(1, 175, slidelen-40); --移动
			mSleep(30);
			
			slidelen=slidelen-40;
		end
		
		touchMove(1, 175, 150);
		mSleep(150);
		touchUp(1, 180, 152);   --抬起
		mSleep(400);
		
		goto scantag;
	end
	
	
	
	
	
	endPoint.x=0; endPoint.y=0;
	looper=looper+1;
	
	touchDown(1, 100, 880); 
	mSleep(120);
	touchMove(1, 102, 560);
	mSleep(450);
	touchMove(1, 100, 360);
	mSleep(300);
	touchMove(1, 102, 228);
	mSleep(600);
	touchUp(1, 100, 228);   --抬起
	mSleep(1000);
	
	if looper<=15 then
		goto scantag;
	end
	
end





