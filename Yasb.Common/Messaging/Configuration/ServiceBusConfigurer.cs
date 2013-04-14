using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    
    public  class ServiceBusConfigurer<TConnectionConfiguration> 
    {
        public ServiceBusConfigurer()
        {
            Built = new ServiceBusConfiguration<TConnectionConfiguration>();
        }
        public ServiceBusConfigurer<TConnectionConfiguration> WithEndPointConfiguration(Action<EndPointConfigurer> action)
        {
            var endPointConfigurer = new EndPointConfigurer();
            action(endPointConfigurer);
            Built.EndPointConfiguration = endPointConfigurer.Built;
            return this;
        }
        public ServiceBusConfigurer<TConnectionConfiguration> WithMessageHandlersAssembly(Assembly assembly)
        {
            Built.MessageHandlersAssembly = assembly;
            return this;
        }
        public ServiceBusConfigurer<TConnectionConfiguration> ConfigureConnections<TConnectionConfigurer>(Action<TConnectionConfigurer> action)
            where TConnectionConfigurer : IConnectionConfigurer<TConnectionConfiguration>
        {
            var connectionConfigurer = Activator.CreateInstance<TConnectionConfigurer>();
            action(connectionConfigurer);
            connectionConfigurer.Connections.ToList().ForEach(Built.AddConnection);
            return this;
        }


        public ServiceBusConfiguration<TConnectionConfiguration> Built { get; private set; }
    }
}
