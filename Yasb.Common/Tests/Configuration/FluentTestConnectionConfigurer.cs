using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Tests.Configuration
{
    public class FluentTestConnectionConfigurer : IConnectionConfigurer<TestConnectionConfiguration>
    {
        private Dictionary<string, TestConnectionConfiguration> _connections = new Dictionary<string,TestConnectionConfiguration>();
        public FluentTestConnectionConfigurer()
        {

        }
       
        public FluentTestConnectionConfigurer WithConnection(string connectionName, string host = "localhost", int port = 6379)
        {
            _connections[connectionName]=new TestConnectionConfiguration(host, port);
            return this;
        }

        public IEnumerable<KeyValuePair<string, TestConnectionConfiguration>> Connections
        {
            get { return _connections.AsEnumerable<KeyValuePair<string, TestConnectionConfiguration>>(); }
        }
    }
}
