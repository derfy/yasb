using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{

    public class EndPointConfigurer<TConnection>
    {
        
        public EndPointConfigurer()
        {
            Built = new EndPointConfiguration<TConnection>();
        }


        public EndPointConfigurer<TConnection> WithLocalEndPoint(string connectionName, string queueName)
        {
            Built.LocalEndPoint = new BusEndPoint(connectionName, queueName,"local");
            Built.AddNamedEndPoint(Built.LocalEndPoint);
            return this;
        }

        public EndPointConfigurer<TConnection> WithEndPoint(string connectionName, string queueName, string endPointName)
        {
            var endPoint = new BusEndPoint(connectionName, queueName, endPointName);
            Built.AddNamedEndPoint(endPoint);
            return this;
        }


        public EndPointConfigurer<TConnection> ConfigureConnections<TConnectionConfigurer>(Action<TConnectionConfigurer> action)
            where TConnectionConfigurer : IConnectionConfigurer<TConnection>
        {
            var connectionConfigurer = Activator.CreateInstance<TConnectionConfigurer>();
            action(connectionConfigurer);
            connectionConfigurer.Connections.ToList().ForEach(Built.AddConnection);
            return this;
        }
        public EndPointConfiguration<TConnection> Built { get; private set; }
        
    }
    public class ServiceBusEndPointConfigurer<TConnection>:EndPointConfigurer<TConnection>
    {
        public ServiceBusEndPointConfigurer<TConnection> WithLocalEndPoint(string connectionName, string queueName)
        {
            Built.LocalEndPoint = new BusEndPoint(connectionName, queueName, "local");
            Built.AddNamedEndPoint(Built.LocalEndPoint);
            return this;
        }
    }
}
