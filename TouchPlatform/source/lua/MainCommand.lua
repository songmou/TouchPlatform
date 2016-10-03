require("TSLib");	--触动精灵函数扩展库
require("basic");


local sz = require("sz");

jsonstring=httpGet(httpUrl.."Dynamiclua");
local tb = sz.json.decode(jsonstring);
local config=tb.data;

writeFileString("/var/mobile/Media/TouchSprite/lua/CommandExtend.lua",config);


require("CommandExtend");
