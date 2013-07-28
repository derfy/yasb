using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;
using System.Net.Sockets;
using Yasb.Common.Messaging.Connections;

namespace Yasb.Common.Messaging.Configuration.CommonConnectionConfigurers
{
    public class FluentIPEndPointConfigurer : FluentConnectionConfigurer<RedisConnection>
    {
       
        public FluentIPEndPointConfigurer WithConnection(string connectionName, string host = "localhost", int port = 6379)
        {    
            base.AddConnection(connectionName, new RedisConnection(host, port));
            return this;
        }

    }
}
