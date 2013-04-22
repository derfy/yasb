using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class FluentConnectionConfigurer<TConnection> : IConnectionConfigurer<TConnection>
    {
        private Dictionary<string, TConnection> _connections = new Dictionary<string, TConnection>();


        public IEnumerable<KeyValuePair<string, TConnection>> Connections
        {
            get { return _connections.AsEnumerable<KeyValuePair<string, TConnection>>(); }
        }

        protected void AddConnection(string connectionName, TConnection connection)
        {
            _connections[connectionName] = connection;
        }
       
    }
}
