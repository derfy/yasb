using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IServiceBusConfiguration<TEndPoint,TEndPointConfiguration>  
        where TEndPoint : IEndPoint
        where TEndPointConfiguration : EndPointConfiguration<TEndPoint>
    {
        IServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithLocalEndPoint(string endPoint);
        IServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithMessageHandlersAssembly(Assembly assembly);
        IServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithEndPoint(string endPoint, Action<TEndPointConfiguration> configurer);

        TEndPoint LocalEndPoint { get; }

        Assembly MessageHandlersAssembly { get; }

        TEndPoint[] NamedEndPoints { get; }

    }
}
