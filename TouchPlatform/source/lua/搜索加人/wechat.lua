require("TSLib");	--触动精灵函数扩展库
require("basic");

--检查聊天消息条数
function CheckMessage()
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy) then
		toast("主界面异常");
		return "";
	end
	
	if not multiColor({{103,1048,0xff3b30},{122,1075,0xff3b30}},fuzzy) then
		return "";
	end
	
	local whitelist = "123456789"
	--a = old_ocrText(103, 1048, 122, 1075, 20, whitelist);
	local msgcount = ocrText(103, 1048, 122, 1075, 10, whitelist);
	mSleep(500);
	
	return trim(msgcount);
end

--检查好友申请条数
function CheckFriendApply()
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy) then
		toast("主界面异常");
		return "";
	end
	
	if not multiColor({{263,1050,0xff3b30},{285,1075,0xff3b30}},fuzzy) then
		return "";
	end
	
	local whitelist = "123456789"
	local applycount = ocrText(263, 1050, 285, 1075, 10, whitelist);
	mSleep(500);
	
	return trim(applycount);
end

--检查朋友圈是否有更新
function CheckTimeLine()
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy) then
		toast("主界面异常");
		return "";
	end
	
	if not multiColor({{417,1060,0xf43531},{434,1060,0xf43531},{425,1052,0xf43531}}) then
		return "";
	end
	
	return "有新的朋友圈消息";
end


function addFriendsByList(customerColl,applys)
	
	mSleep(2000*radix);
	if table.getn(customerColl)  == 0 then 
		return "";
	end
	
	
	--微信主界面
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy) then
		toast("微信主界面异常");
		return "";
	end
	
	mSleep(2000*radix);
	click(84,1080,30*radix);
	mSleep(500*radix);
	
	if not multiColor({{587,71,0xffffff},{588,96,0xffffff},{575,85,0xffffff}},fuzzy) then
		toast("主界面异常");
		mSleep(500*radix);
		return "";
	end
	
	--点击+
	click(586,86,30*radix);
	mSleep(2000*radix);
	
	--点击添加朋友
	click(503,278,30*radix);
	mSleep(3000*radix);
	
	--点击输入框(进入输入微信号或者手机号的输入框界面)
	click(231,202,30*radix);
	mSleep(2000*radix);
	
	mSleep(2*1000*radix); 
	
	local successline=0;
	local ii=0;
	for i, v in pairs(customerColl) do
		
		local friendlist=unserialize(readFileString("/var/mobile/Media/TouchSprite/res/friendlist.txt"));
		if friendlist==nil then
			friendlist={};
		end
		
		--找不到的手机号
		local nonelist=unserialize(readFileString("/var/mobile/Media/TouchSprite/res/nonelist.txt"));
		if nonelist==nil then
			nonelist={};
		end
		
		--执行一次任务，执行到哪一步
		local tempii=readFileString("/var/mobile/Media/TouchSprite/res/tempii.txt");
		if tempii==nil then
			tempii=tostring(ii);
		else
			ii=tonumber(tempii);
			toast("已执行"..tempii.."次");
			mSleep(1*1000*radix); 
		end
		
		--查询该手机号是否在数组中
		--if not IsInTable(v,friendlist) and not IsInTable(v,nonelist) then
		if not IsInTable(v,friendlist) then
		
			--toast(v);
			
			mSleep(4*1000*radix); 
			
			
			--【个数】 添加10个后 暂停600s 
			ii=ii+1;
			if ii%10==0 then
				mSleep(1*1000*radix); 
			end
			
			--添加20个后 暂停3000s
			if ii%20==0 then
				mSleep(2*1000*radix); 
			end
			
			--【自动停止】 一次执行超过100次自动停止
			if ii>=100 then
				toast("此次脚本执行已超过"..ii.."次，自动停止");
				lua_exit();
			end
			
			writeFileString("/var/mobile/Media/TouchSprite/res/tempii.txt",tostring(ii))
			
			
			inputText(v);
			mSleep(3*1000*radix); 
			
			--界面正常，绿色放大镜（可能搜索不到该人）
			if multiColor({{77,155, 0x2ba245},{77,225, 0x2ba245},{40,189,0x2ba245}},fuzzy) then
				click(231,202,30*radix);
				mSleep(5*1000*radix);
				
				
				--出现查找失败的情况，点击确认
				if multiColor({{336,437,0x999999},{326,587,0xefefef},{292,648,0x007aff}},fuzzy) then
					click(292,648,30*radix);
					mSleep(2*1000*radix);
					
					--点击X去掉输入的手机号
					click(503,83,30*radix);
					mSleep(2*1000*radix);
					
					
					toast("手机号查找失败无结果");
					mSleep(1*1000*radix); 
					
					table.insert(nonelist,v);
					writeFileString("/var/mobile/Media/TouchSprite/res/nonelist.txt",serialize(nonelist))
					
				end
				
				
				--logs("已点击放大镜进入资料页面"..v);
				
				--如果进入详细资料的页面
				if multiColor({{268,78,0xffffff},{341,85,0xffffff},{374,85,0x3b3a3f}},fuzzy) then
					
					--点击添加到通讯录按钮（位置不固定）
					if multiColor({{355,875,0x06bf04}},fuzzy) then
						click(355,875,30*radix);
					elseif multiColor({{355,668,0x06bf04}},fuzzy) then
						click(355,668,30*radix);
					elseif multiColor({{355,760,0x06bf04}},fuzzy) then
						click(355,760,30*radix);
					elseif multiColor({{355,940,0x06bf04}},fuzzy) then
						click(355,940,30*radix);
					elseif multiColor({{355,1110,0x06bf04}},fuzzy) then
						click(355,1110,30*radix);
					else
						toast("未找到添加按钮");
						mSleep(4*1000*radix);
					end
					
					mSleep(4*1000*radix);
					
					logs("已点击添加到通讯录按钮"..v);
					
					
					--发送朋友验证的页面
					if multiColor({{570,250,0xcccccc},{580,250, 0xffffff},{589,250,0xcccccc}},fuzzy) then
						click(578,250,30*radix);
						mSleep(5*1000*radix);
						
						inputText(applys);
						mSleep(5*1000*radix); 
						
						
						--如果是绿色的发送按钮，点击
						if multiColor({{595,81,0x20d81f},{567,83,0x20d81f}},fuzzy) then
							
							
							
							--[[将已经发送过的手机号记录下来
							local friendlist=unserialize(readFileString("/var/mobile/Media/TouchSprite/res/friendlist.txt"));
							if friendlist==nil then
								friendlist={};
							end]]
							
							--查询某个值是否在数组中
							if not IsInTable(v,friendlist) then
								table.insert(friendlist,v);
								
								--点击发送申请按钮
								click(587,82,30*radix);
								mSleep(2*1000*radix);
								toast("成功对手机号"..v.."发送好友申请");
								mSleep(3*1000*radix);
								
								
								writeFileString("/var/mobile/Media/TouchSprite/res/friendlist.txt",serialize(friendlist))
								
							else
								toast("该手机号"..v.."已忽略");
								Backer(1);
							end
							mSleep(6*1000*radix);
							
							
						else
							toast("异常未找到发送按钮");
							mSleep(5*1000*radix);
						end
						
						
						--成功发送申请
						successline=successline+1;
						
						
					else
						toast("还未进入添加验证的页面");
						mSleep(5*1000*radix);
					end
					
					
					
					--如果出现发送失败（太频繁）
					if multiColor({{116,483,0xe2e2e3},{265,501,0x000000},{311,501,0x000000},{329,501,0x000000}},fuzzy) then
						--点击确定
						click(337,648,30*radix);
						mSleep(1*1000*radix);
						
						Backer(2);
						
						BackRight(1);
						
						Backer(2);
						
						--跳出循环(不再加人)
						break;
					end
					
					--返回上一级，到输入微信号的界面
					Backer(1);
					

				
					mSleep(4*1000*radix); 
					
				else
					Backer(1);
					--logs("按钮颜色不匹配"..v);
					--toast("按钮颜色不匹配");
					mSleep(4*1000*radix); 
				end
			else
				toast("手机号可能暂未注册微信");
				mSleep(1*1000*radix); 
				
				table.insert(nonelist,v);
				writeFileString("/var/mobile/Media/TouchSprite/res/nonelist.txt",serialize(nonelist))
			end
			
			mSleep(3*1000*radix); 
			
			--如果成功返回到开始输入微信号的界面上（点击输入框让其获取焦点）
			if multiColor({{497,83,0x8e8e93},{503,83,0xffffff},{510,83,0x8e8e93}},fuzzy) then
				
				--点击“X”去掉输入框的内容
				click(497,83,30*radix);
				
				--点击输入框获取焦点
				click(197,89,30*radix);
				mSleep(500*radix); 
			end
			
			
			logs("已成功发送"..v.."的好友申请");
			
			--【间隔】：  添加一个手机号的间隔时间  60S
			mSleep(1*1000*radix);
		else
			toast("手机号已跳过");
			mSleep(1*1000*radix); 
		end
	
    end 
	
	BackRight(1);
	Backer(1);

	
	mSleep(2*1000*radix);
	return successline;
end


function addFriendsByContacts(count,v)
	
	--微信主界面
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23, 1075, 0xf8f8f8},{530, 1037, 0xb2b2b2}},fuzzy) then
		toast("微信主界面异常");
		return "";
	end
	
	mSleep(1000*radix);
	click(84,1080,30*radix);
	mSleep(1000*radix);
	
	if not multiColor({{587,71,0xffffff},{588,96,0xffffff},{575,85,0xffffff}},fuzzy) then
		toast("主界面异常");
		mSleep(500*radix);
		return "";
	end
	
	--点击+
	click(586,86,30*radix);
	mSleep(1000*radix);
	
	--点击添加朋友
	click(503,278,30*radix);
	mSleep(2000*radix);
	
	
	--点击手机联系人
	click(228,790,30*radix);
	
	mSleep(2*1000*radix); 
	
	
	
    local DeviceId = getDeviceID(); 
	
	--local Position={[DeviceId]={["DeviceId"]=DeviceId,["x"]="0",["y"]="0",["color"]="0xf0f0f6"}};
	--writeFileString("/var/mobile/Media/TouchSprite/res/wechat.txt",serialize(Position))
	
	local pstring=unserialize(readFileString("/var/mobile/Media/TouchSprite/res/wechat.txt"));
	
	--writeFile("/var/mobile/Media/TouchSprite/res/wechat.txt",Position)
	
	if pstring == nil then
		pstring={[DeviceId]={["DeviceId"]=DeviceId,["x"]=600,["y"]=315,["color"]="0x06bf04",["len"]=0}};
	end
	local deviceInfo=pstring[DeviceId];
	
	
	--按钮高度，按钮间最小间距，按钮的x坐标，开始循环的最小Y坐标，索引
	local btnHeight=60; btnMinSpace=110; btnX=600; local btnMinY=172; local index=1;
	
	--滑动到上次添加的 位置
	local slidelen=deviceInfo.len;
	if slidelen>0 then
		while slidelen>440 do
			touchDown(1, 320, btnMinY+440); --在 (150, 550) 按下
			mSleep(1000);
			touchMove(1, 320, btnMinY); --移动到 (150, 600)
			mSleep(1000);
			touchUp(1, 320, btnMinY);
			mSleep(300);
			
			slidelen=slidelen-440;
		end
		touchDown(1, 320, btnMinY+slidelen); --在 (150, 550) 按下
		mSleep(1000);
		touchMove(1, 320, btnMinY); --移动到 (150, 600)
		mSleep(1000);
		touchUp(1, 320, btnMinY);
		mSleep(300);
		
		toast("已回到上次的位置");
		mSleep(1000);
	end

	
		
	--toast(pstring[DeviceId].x);
	
	--moveTo(deviceInfo.x,deviceInfo.y,deviceInfo.x,deviceInfo.y+deviceInfo.len)
	
	while index<=count
	do
		--moveTo(320,700,320,150)		--往下滑动1000像素
		
		prevcolor1=tostring(getColor(30, btnMinY+btnHeight/2));
		prevcolor2=tostring(getColor(80, btnMinY+btnHeight/2));
		
		--从按钮那一列开始循环 判断是否是按钮
		for y=btnMinY,btnMinY+btnMinSpace,1 do
			if multiColor({{btnX,y,0x1b9e19},{btnX,y+btnHeight-1,0x1b9e19}}) then
				
				--避免发送2次邀请
				if tostring(getColor(30,y+btnHeight/2))==prevcolor1 and tostring(getColor(80,y+btnHeight/2))==prevcolor2 then
					toast("重复...");
				else
					--toast(pstring[DeviceId].len.."检测到按钮");
					--点击添加按钮进入发送 验证页面
					click(btnX,y+btnHeight/2,2000*radix);
					mSleep(3*1000*radix);
				end
				
				--发送朋友验证的页面
				if multiColor({{570,250,0xcccccc},{580,250, 0xffffff},{589,250,0xcccccc}}) then
					click(578,250,30*radix);
					mSleep(2*1000*radix);
					
					inputText(v);
					mSleep(2*1000*radix); 
					
					--点击发送的按钮
					click(590,82,30*radix);
					mSleep(1000*radix);
					
					
					--成功发送申请
					index=index+1;
					Backer(1);
				end
				
				
				--某些人点击添加按钮即可添加成功，这里做一下判断
				--如果到详情页面，就返回上一页。
				if multiColor({{573,84,0xffffff},{581,84,0x3b3a3f},{589,84,0xffffff}}) then
					Backer(1);
				end
				
				
				
				mSleep(1000*radix); 
			end
			
		end
		mSleep(1000*radix); 
		
		--循环查找一段按钮后，滑动相同的距离 再次查找
		--moveTo(pstring[DeviceId].x,btnMinY+btnMinSpace,pstring[DeviceId].x,btnMinY,100,500)
		touchDown(1, 320, btnMinY+btnMinSpace); --在 (150, 550) 按下
		mSleep(1000);
		touchMove(1, 320, btnMinY); --移动到 (150, 600)
		mSleep(800);
		touchUp(1, 320, btnMinY);
		pstring[DeviceId].len=pstring[DeviceId].len+btnMinSpace;
		
		
		--是否已到底部（到了底部，从上面到屏幕下方循环查找按钮）
		mSleep(1000);
		if tostring(prevcolor1)==tostring(getColor(30, btnMinY+btnHeight/2)) and 
		   tostring(prevcolor2)==tostring(getColor(80, btnMinY+btnHeight/2)) then
			--toast("已到底部")
			
			for y=btnMinY+btnMinSpace,1130,1 do
				if multiColor({{btnX,y,0x1b9e19},{btnX,y+btnHeight-1,0x1b9e19}}) then
					if tostring(getColor(30,y+btnHeight/2))==prevcolor1 and tostring(getColor(80,y+btnHeight/2))==prevcolor2 then
					toast("重复...");
				else
					--toast(pstring[DeviceId].len.."检测到按钮");
					--点击添加按钮进入发送 验证页面
					click(btnX,y+btnHeight/2,2000*radix);
					mSleep(3*1000*radix);
				end
				
				--发送朋友验证的页面
				if multiColor({{570,250,0xcccccc},{580,250, 0xffffff},{589,250,0xcccccc}}) then
					click(578,250,30*radix);
					mSleep(2*1000*radix);
					
					inputText(v);
					mSleep(2*1000*radix); 
					
					--点击发送的按钮
					click(590,82,30*radix);
					mSleep(1000*radix);
					
					
					--成功发送申请
					index=index+1;
					Backer(1);
					
					
				end
				
				
				--某些人点击添加按钮即可添加成功，这里做一下判断
				--如果到详情页面，就返回上一页。
				if multiColor({{573,84,0xffffff},{581,84,0x3b3a3f},{589,84,0xffffff}}) then
					Backer(1);
				end
				
				
				mSleep(1000*radix); 
				end
			end
		
			
			index=count+1;   --跳出循环
		end
		
		
		mSleep(1*1000*radix); 
	end
	
	Backer(2);
	
	
	writeFileString("/var/mobile/Media/TouchSprite/res/wechat.txt",serialize(pstring))
	
end



function addFriendsByNewFriends(count,v)
	
	--微信主界面
	if not multiColor({{23,83,0x3b3a3f},{114,83,0x3b3a3f},{23,1075,0xf8f8f8},{530,1037,0xb2b2b2}},fuzzy) then
		toast("微信主界面异常");
		return "";
	end
	
	mSleep(1000*radix);
	click(237,1081,30*radix);
	mSleep(1000*radix);
	
	local index=1;
	while multiColor({{568,171,0xc5c5c5},{36,261,0xfa9d3b},{29,362,0x2ba245}},fuzzy) and index<=10 do
		--toast("微信通讯录界面异常");
		moveTo(290,937,300,300,100);
		
		index=index+1;
		mSleep(1.5*1000*radix);
	end

	--点击橙色的新的朋友
	click(36,261,30*radix);
	mSleep(1000*radix);
	
end


--[[添加一条通讯录，打开微信查看是否有推荐消息]]
function AddFriendsByNewContracts(customerColl)
	
	for i, v in pairs(customerColl) do
		mSleep(1*1000*radix);
		
		--先添加通讯录好友
		openAppBid("com.apple.MobileAddressBook");
		
		
		
	end
	mSleep(1*1000*radix);
end

function SleepToast(time)
	for index=1,array,time do
		toast(array);
	end
end



