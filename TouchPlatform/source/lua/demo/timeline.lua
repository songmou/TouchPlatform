require("TSLib");	--触动精灵函数扩展库
require("basic");

--从微信首页进入 朋友圈
function intoShareline()
	if not multiColor({{33,79, 0x39383e},{106,109, 0x434246},{497,85, 0x3b3a3f}}) then
		toast("不是在微信主页");
		return false;
	end
	
	--点   发现
	touchDown(1, 399, 1075);
	mSleep(20*radix); 
	touchUp(1, 399, 1073);
	mSleep(1*1000*radix); 
	
	--通过朋友圈的图标判断是不是在发现的界面
	if not multiColor({{38,199,0xffc817},{53,217,0x66d020},{56,184,0xfa5452},{71,203,0x5283f0}}) then
		toast("朋友圈界面异常");
		return false;
	end
	
	--点朋友圈
	touchDown(1, 261, 205);
	mSleep(20*radix); 
	touchUp(1, 260, 205);
	mSleep(1*1000*radix); 
	
	if not multiColor({{573,87,0xffffff},{608,86,0xffffff},{590,69,0xffffff},{590,99,0xffffff}}) then
		toast("未能成功进入朋友圈");
		return false;
	end
end


--发送朋友圈纯文本内容
function sharingTxtAction(content)
	mSleep(500*radix);
	if string.len(content)  == 0 then 
		return false;
	end
	
	intoShareline();
	
	--点朋友圈相机
	touchDown(1, 573, 87);
	mSleep(3000*radix); 
	touchUp(1, 573, 85);
	mSleep(1*1000*radix); 
	
	if multiColor({{47,333,0xf9f7fa},{592,339,0xf9f7fa},{309,708,0xaeacaf}}) then
		--出现提示
		touchDown(1, 321, 752);
		mSleep(20*radix); 
		touchUp(1, 320, 752);
		mSleep(1*600*radix); 
	end
	
	--输入发送的内容
	inputText(content);
	mSleep(1*600*radix); 
	
	
	touchDown(1, 346,353); --在 (150, 550) 按下
	mSleep(200);
	touchMove(1, 331,140); --移动到 (150, 600)
	mSleep(200);
	touchUp(1, 320, 140);
	mSleep(2000);
	--需要勾选发送到QQ空间
	if multiColor({{35,743,0xc9c9c9},{65,743,0xc9c9c9},{72,743,0xefeff4}}) then
		--如果出现qq空间的图标
		--toast("检查到图标");
		click(51,748,40);
		mSleep(1000);

	end
	
	
	--发送
	touchDown(1, 587, 83);
	mSleep(20*radix); 
	touchUp(1, 587, 80);
	mSleep(1*1000*radix); 
	
	return true;
end

--发送朋友圈图文内容
function sharingImageAction(urlColl,content)
	mSleep(500*radix);
	if string.len(content)  == 0 then 
		--return false;
		content="";
	end
	
	intoShareline();
	
	--将图片保存在相册
	for i, v in pairs(urlColl) do  
		--local ResPath="/var/mobile/Media/TouchSprite/res/";
		--toast(filename);
		
		saveImageToAlbum(v);
		mSleep(800*radix);
    end 
	mSleep(1000*radix);
	
	
	--点朋友圈相机
	click(573,87,100*radix);
	mSleep(1*1000*radix);
	
	--选择手机相册
	click(320,979,100*radix);
	mSleep(1*1000*radix);
	
	--是否出现选择相册（需要点击一次）
	if multiColor({
		{  292,  241, 0xd9d9d9},
		{  431,  241, 0xd9d9d9},
		{  431,  355, 0xd9d9d9},
		{  292,  355, 0xd9d9d9},
	}) then
		click(325,188,100*radix);
		mSleep(1*1000*radix);
	end

	
	local list = getList("/var/mobile/Media/DCIM/100APPLE/");
	local ImgCount=table.getn(list);
	local ShareCount=table.getn(urlColl);

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

			click(x,y,300*radix);
			toast(x..","..y);
			mSleep(1*800*radix);
			
		else
			local x=601; local y=929;			--大于20张图片时候的最后一张图片坐标
			
			if math.floor(index%4)>0 then			--第1-3列的图片
				x=x-(4-math.floor(index%4))*158;
			else									--第4列的图片			
				x=x-math.floor(index%4)*158;
			end
			--y=y-(math.floor((ImgCount-index)/4))*158;
			y=y-(math.floor((ImgCount-1)/4)-math.floor((index-1)/4))*158;

			click(x,y,300*radix);
			logs(x..","..y);
			mSleep(1*800*radix);
			
		end
	
	end

	--点击完成
	if multiColor({{540,1091,0x09bb07},{504,1091,0x09bb07}}) then
		click(523,1088,300*radix);
		mSleep(1*600*radix); 
	else
		toast("选择图片异常");
		return false;
	end
	
	--输入发送的内容
	mSleep(1.5*1000*radix); 
	click(262,174,300*radix);
	mSleep(2*1000*radix); 
	inputText(content);
	mSleep(2*1000*radix); 
	
	
	--发送
	click(587,80,300*radix);
	mSleep(1*1000*radix); 

	
	return true;
end
