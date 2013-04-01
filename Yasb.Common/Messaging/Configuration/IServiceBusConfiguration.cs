using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IServiceBusConfiguration<TEndPointConfiguration> 
        where TEndPointConfiguration : EndPointConfiguration
    {
        IServiceBusConfiguration<TEndPointConfiguration> WithLocalEndPoint(string endPoint);
        IServiceBusConfiguration< TEndPointConfiguration> WithMessageHandlersAssembly(Assembly assembly);
        IServiceBusConfiguration<TEndPointConfiguration> WithEndPoint(string endPoint, Action<TEndPointConfiguration> configurer);

        IEndPoint LocalEndPoint { get; }

        Assembly MessageHandlersAssembly { get; }

        IEndPoint[] NamedEndPoints { get; }

    }
}
