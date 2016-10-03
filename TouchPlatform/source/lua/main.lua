require("TSLib");	--触动精灵函数扩展库
require("basic");

--[[
--是否出现选择相册（需要点击一次）
if multiColor({
	{  292,  241, 0xd9d9d9},
	{  431,  241, 0xd9d9d9},
	{  431,  355, 0xd9d9d9},
	{  292,  355, 0xd9d9d9},
},90) then
	click(325,188,100);
	toast("OK")
	mSleep(2*radix);
else
	
	toast("False")
	mSleep(2*radix);
end
]]