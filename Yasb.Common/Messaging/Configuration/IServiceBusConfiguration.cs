using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IServiceBusConfiguration
    {
        IServiceBusConfiguration WithLocalEndPoint(string host,int port,string queueName);
        IServiceBusConfiguration WithMessageHandlersAssembly(Assembly assembly);
    }
}
