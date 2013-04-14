using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using System.Net;

namespace Yasb.Redis.Messaging.Configuration
{
    public class FluentRedisConnectionConfigurer : IConnectionConfigurer<EndPoint>
    {
        private Dictionary<string, EndPoint> _connections = new Dictionary<string, EndPoint>();
        public FluentRedisConnectionConfigurer()
        {

        }
      
        public FluentRedisConnectionConfigurer WithConnection(string connectionName, string host = "localhost", int port = 6379)
        {
            var ipAddress = IPAddress.Parse(host);
            _connections[connectionName] = new IPEndPoint(ipAddress, port);
            return this;
        }

        public IEnumerable<KeyValuePair<string, EndPoint>> Connections
        {
            get { return _connections.AsEnumerable<KeyValuePair<string, EndPoint>>(); }
        }
    }
}
