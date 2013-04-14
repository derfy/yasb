﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;

namespace Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers
{
    public class FluentIPEndPointConfigurer : FluentConnectionConfigurer<EndPoint>
    {
       
        public FluentIPEndPointConfigurer WithConnection(string connectionName, string host = "localhost", int port = 6379)
        {
            var ipAddress = IPAddress.Parse(host);
            base.AddConnection(connectionName,new IPEndPoint(ipAddress, port));
            return this;
        }

    }
}