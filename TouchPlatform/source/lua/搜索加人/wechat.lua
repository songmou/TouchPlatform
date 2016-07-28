require("TSLib");	--触动精灵函数扩展库
require("basic");
require("config");

function AddFriendInPage(phoneNum,IsSendMsg,WelcomeMsg)
	mSleep(1*radix); 

	--如果不是在输入号码的界面上，则重启应用进入到该界面
	if not multiColor({{522,80,0xdadbdf},{561,74,0x06bf04},{620,110,0xefeff4}},fuzzy) then
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
	if multiColor({{522,80,0xdadbdf},{561,74,0x06bf04},{620,110,0xefeff4}},fuzzy) then
		--点击“X”去掉输入框的内容
		click(497,83,30);

		--点击输入框获取焦点
		click(197,89,30);
		
		mSleep(1*radix); 
		inputText(tostring(phoneNum));
		mSleep(3*radix); 
		
		
		
		--绿色放大镜（可能搜索不到该人）
		if multiColor({{77,155, 0x2ba245},{77,225, 0x2ba245},{40,189,0x2ba245}},fuzzy) then
			--点击绿色放大镜
			click(231,202,30);
			mSleep(5*radix);
			
			--出现查找失败的情况，点击确认
			if multiColor({{336,437,0x999999},{326,587,0xefefef},{292,648,0x007aff}},fuzzy) then
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
		if multiColor({{268,78,0xffffff},{341,85,0xffffff},{374,85,0x3b3a3f}},fuzzy) then
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
			if multiColor({{570,250,0xcccccc},{580,250, 0xffffff},{589,250,0xcccccc}},fuzzy) then
				click(578,250,30);
				mSleep(5*radix);
				
				inputText(WelcomeMsg);
				mSleep(5*radix); 
				
				--如果这个号码没有被添加过，可以发送好友申请
				if getDetailStatus(phoneNum) then
				
					--如果是绿色的发送按钮，点击
					if multiColor({{595,81,0x20d81f},{567,83,0x20d81f}},fuzzy) then
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
			if multiColor({{116,483,0xe2e2e3},{265,501,0x000000},{311,501,0x000000},{329,501,0x000000}},fuzzy) then
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
