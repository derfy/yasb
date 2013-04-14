using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IServiceBusConfiguration {
        Assembly MessageHandlersAssembly { get; }
        EndPointConfiguration EndPointConfiguration { get;}
    }
    public class ServiceBusConfiguration<TConnectionConfiguration> : IServiceBusConfiguration 
    {
        private Dictionary<string, TConnectionConfiguration> _connections = new Dictionary<string, TConnectionConfiguration>();
        private List<BusEndPoint> _endPoints = new List<BusEndPoint>();
        internal ServiceBusConfiguration()
        {

        }
        public Assembly MessageHandlersAssembly { get; internal set; }
        public EndPointConfiguration EndPointConfiguration { get; internal set; }
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
