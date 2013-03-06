using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IServiceBusConfiguration
    {
        IServiceBusConfiguration WithLocalEndPoint(Action<EndPointConfiguration> configurer);
        IServiceBusConfiguration WithMessageHandlersAssembly(Assembly assembly);
        IServiceBusConfiguration WithEndPoint(string endPointName, Action<EndPointConfiguration> configurer);
        BusEndPoint GetEndPointByName(string endPointName);
    }
}
