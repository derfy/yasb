using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConfigurator<TEndPoint, TEndPointConfiguration>
        where TEndPoint : IEndPoint
        where TEndPointConfiguration : EndPointConfiguration<TEndPoint>
    {
        IConfigurator<TEndPoint, TEndPointConfiguration> Bus(Action<ServiceBusConfiguration<TEndPoint, TEndPointConfiguration>> configurator);
        IResolver Resolver();
    }
}
