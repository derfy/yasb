using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    
    public  class ServiceBusConfigurer<TConnection> 
    {
        public ServiceBusConfigurer()
        {
            Built = new ServiceBusConfiguration<TConnection>();
        }
        public ServiceBusConfigurer<TConnection> WithEndPointConfiguration(Action<ServiceBusEndPointConfigurer<TConnection>> action)
        {
            var endPointConfigurer = new ServiceBusEndPointConfigurer<TConnection>();
            action(endPointConfigurer);
            Built.EndPointConfiguration = endPointConfigurer.Built;
            return this;
        }
        public ServiceBusConfigurer<TConnection> WithMessageHandlersAssembly(Assembly assembly)
        {
            Built.MessageHandlersAssembly = assembly;
            return this;
        }
        public ServiceBusConfigurer<TConnection> ConfigureConnections<TConnectionConfigurer>(Action<TConnectionConfigurer> action)
            where TConnectionConfigurer : IConnectionConfigurer<TConnection>
        {
            var connectionConfigurer = Activator.CreateInstance<TConnectionConfigurer>();
            action(connectionConfigurer);
            connectionConfigurer.Connections.ToList().ForEach(Built.EndPointConfiguration.AddConnection);
            return this;
        }


        public ServiceBusConfiguration<TConnection> Built { get; private set; }
    }
}
