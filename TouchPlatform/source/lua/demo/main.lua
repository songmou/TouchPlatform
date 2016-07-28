require("TSLib");	--触动精灵函数扩展库
require("basic");

local sz = require("sz")
local json = sz.json

jsonstring=httpGet("http://192.168.1.196/Lua/GetConfig?luaType=搜索加人")

--dialog(str)

local tb = json.decode(jsonstring);

dialog(tb.data.netWay, 0);


mSleep(3000);