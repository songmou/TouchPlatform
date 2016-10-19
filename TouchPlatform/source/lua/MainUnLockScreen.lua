
flag = deviceIsLock();
if flag == 0 then
    dialog("未锁定",3);
else
    unlockDevice(); --解锁屏幕
end