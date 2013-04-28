using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class ConnectionsConfiguration<TConnection>
    {
        private Dictionary<string, TConnection> _connections = new Dictionary<string, TConnection>();
        internal ConnectionsConfiguration()
        {

        }
        internal void AddConnection(string connectionName,TConnection connection)
        {
            _connections[connectionName]=connection;
        }

        public TConnection GetConnectionByName(string connectionName)
        {
            TConnection connection = default(TConnection);
            if(!_connections.TryGetValue(connectionName,out connection))
                throw new ApplicationException(string.Format("No connection named {0} has been configured", connectionName));
            return connection;
        }

    }
}
