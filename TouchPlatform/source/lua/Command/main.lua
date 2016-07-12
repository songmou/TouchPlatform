require("TSLib");	--触动精灵函数扩展库
require("basic");
require("Extend");


local sz = require("sz");

jsonstring=httpGet(httpUrl.."Dynamiclua");
local tb = sz.json.decode(jsonstring);
local config=tb.data;

writeFileString("/var/mobile/Media/TouchSprite/lua/Command/Extend.lua",config);


if(nil==finishTag) then
	lua_restart(); 
end
