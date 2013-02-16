using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        
        public IServiceBusConfiguration WithLocalEndPoint(string host,int port,string queueName)
        {
            EndPoint = new BusEndPoint(host,port,queueName);
            return this;
        }

        public IServiceBusConfiguration WithMessageHandlersAssembly(Assembly assembly)
        {
            MessageHandlersAssembly = assembly;
            return this;
        }

        public BusEndPoint EndPoint { get; private set; }

        public Assembly MessageHandlersAssembly { get; private set; }

    }
}
