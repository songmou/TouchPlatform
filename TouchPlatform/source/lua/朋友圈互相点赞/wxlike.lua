require("TSLib");	--触动精灵函数扩展库
require("basic");

--从微信首页进入 朋友圈  并刷新
function intoShareline()
	if not multiColor({{33,79, 0x39383e},{106,109, 0x434246},{497,85, 0x3b3a3f}},fuzzy) then
		toast("不是在微信主页");
		return false;
	end
	
	--点   发现
	touchDown(1, 399, 1075);
	mSleep(3*radix); 
	touchUp(1, 399, 1073);
	mSleep(1*radix); 
	
	--通过朋友圈的图标判断是不是在发现的界面
	if not multiColor({{38,199,0xffc817},{53,217,0x66d020},{56,184,0xfa5452},{71,203,0x5283f0}},fuzzy) then
		toast("朋友圈界面异常");
		return false;
	end
	
	--点朋友圈
	touchDown(1, 261, 205);
	mSleep(20); 
	touchUp(1, 260, 205);
	mSleep(1*radix); 
	
	if not multiColor({{573,87,0xffffff},{608,86,0xffffff},{590,69,0xffffff},{590,99,0xffffff}},fuzzy) then
		toast("未能成功进入朋友圈");
		return false;
	end
	
		
	touchDown(1, 150, 350); --在 (150, 550) 按下
	mSleep(200);
	touchMove(1, 150, 800); --移动到 (150, 600)
	mSleep(500);
	touchUp(1, 150, 800);   --在 (150, 600) 抬起

	mSleep(5*radix); 	--等待5s刷新朋友圈
end


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
	
	touchDown(1, 326, 880); 
	mSleep(300);
	touchMove(1, 330, 560);
	mSleep(450);
	touchMove(1, 325, 360);
	mSleep(300);
	touchMove(1, 333, 228);
	mSleep(600);
	touchUp(1, 333, 228);   --抬起
	
	
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
	
	touchDown(1, 326, 880); 
	mSleep(150);
	touchMove(1, 330, 560);
	mSleep(450);
	touchMove(1, 325, 360);
	mSleep(300);
	touchMove(1, 333, 228);
	mSleep(600);
	touchUp(1, 333, 228);   --抬起
	mSleep(1000);
	
	if looper<=15 then
		goto scantag;
	end
	
end



