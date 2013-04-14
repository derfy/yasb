using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConfigurator<TResolver, TConnectionConfiguration> 
        where TResolver : IResolver<TResolver>
    {
        IConfigurator<TResolver, TConnectionConfiguration> ConfigureServiceBus(Action<ServiceBusConfigurer<TConnectionConfiguration>> busConfigurer);
        TResolver Configure();
    }
}
