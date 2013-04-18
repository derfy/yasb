using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class ConnectionsRepository<TConnectionConfiguration>
    {
        private Dictionary<string, TConnectionConfiguration> _connections = new Dictionary<string, TConnectionConfiguration>();
        internal ConnectionsRepository()
        {

        }
        internal void AddConnection(KeyValuePair<string,TConnectionConfiguration> connectionPair)
        {
            _connections.Add(connectionPair.Key,connectionPair.Value);
        }

        public TConnectionConfiguration GetConnectionByName(string connectionName)
        {
            TConnectionConfiguration connection = default(TConnectionConfiguration);
            if(!_connections.TryGetValue(connectionName,out connection))
                throw new ApplicationException(string.Format("No connection named {0} has been configured", connectionName));
            return connection;
        }
    }
}
