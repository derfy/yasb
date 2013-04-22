using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class ConnectionsRepository<TConnection>
    {
        private Dictionary<string, TConnection> _connections = new Dictionary<string, TConnection>();
        internal ConnectionsRepository()
        {

        }
        internal void AddConnection(KeyValuePair<string,TConnection> connectionPair)
        {
            _connections.Add(connectionPair.Key,connectionPair.Value);
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
