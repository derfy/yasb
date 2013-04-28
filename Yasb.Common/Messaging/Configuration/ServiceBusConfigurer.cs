using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{

    public class ServiceBusConfigurer<TConnection> 
    {
        public ServiceBusConfigurer()
        {
            Built = new ServiceBusConfiguration<TConnection>();
        }
        public ServiceBusConfigurer<TConnection> WithEndPointConfiguration(Action<ServiceBusEndPointConfigurer> action)
        {
            var endPointConfigurer = new ServiceBusEndPointConfigurer();
            action(endPointConfigurer);
            Built.EndPointConfiguration = endPointConfigurer.Built;
            return this;
        }

        public ServiceBusConfigurer<TConnection> ConfigureConnections<TConnectionConfigurer>(Action<TConnectionConfigurer> action)
            where TConnectionConfigurer : IConnectionConfigurer<TConnection>
        {
            var connectionConfigurer = Activator.CreateInstance<TConnectionConfigurer>();
            action(connectionConfigurer);
            Built.ConnectionConfiguration = connectionConfigurer.Built;
            return this;
        }

        public ServiceBusConfigurer<TConnection> WithMessageHandlersAssembly(Assembly assembly)
        {
            Built.MessageHandlersAssembly = assembly;
            return this;
        }

        public ServiceBusConfiguration<TConnection> Built { get; private set; }
    }
}
