using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class FluentConnectionConfigurer<TConnection> : IConnectionConfigurer<TConnection>
    {
        ConnectionsConfiguration<TConnection> _connections = new ConnectionsConfiguration<TConnection>();

        protected void AddConnection(string connectionName, TConnection connection)
        {
            _connections.AddConnection(connectionName,connection);
        }

        public ConnectionsConfiguration<TConnection> Built { get { return _connections; } }
       
    }
}
