﻿using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchPlatform
{
    public class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}