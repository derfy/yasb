using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConfigurator<TResolver>where TResolver : IResolver<TResolver>
    {
        TResolver Configure(Action<ServiceBusConfiguration> configurator);
    }
}
