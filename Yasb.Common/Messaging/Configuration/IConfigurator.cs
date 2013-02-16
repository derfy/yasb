using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConfigurator
    {
        IConfigurator Bus(Action<IServiceBusConfiguration> configurator);
        IResolver Resolver();
    }
}
