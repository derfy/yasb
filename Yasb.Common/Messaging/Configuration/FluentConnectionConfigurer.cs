using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class FluentConnectionConfigurer<TConnectionConfiguration> : IConnectionConfigurer<TConnectionConfiguration>
    {
        private Dictionary<string, TConnectionConfiguration> _connections = new Dictionary<string, TConnectionConfiguration>();


        public IEnumerable<KeyValuePair<string, TConnectionConfiguration>> Connections
        {
            get { return _connections.AsEnumerable<KeyValuePair<string, TConnectionConfiguration>>(); }
        }

        protected void AddConnection(string connectionName, TConnectionConfiguration connection)
        {
            _connections[connectionName] = connection;
        }
       
    }
}
