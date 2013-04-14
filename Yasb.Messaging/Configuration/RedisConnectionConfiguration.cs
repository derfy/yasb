using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging.Configuration
{
    public class RedisConnectionConfiguration 
    {
        public RedisConnectionConfiguration(string host, int port)
        {
             EndPoint = Parse(host, port);
        }

        private IPEndPoint Parse(string host, int port)
        {
            var ipAddress = IPAddress.Parse(host);
            return new IPEndPoint(ipAddress, port);
        }
        public IPEndPoint EndPoint { get;private set; }
       
        
    }
}
