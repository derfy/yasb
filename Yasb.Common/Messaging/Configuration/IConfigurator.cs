using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public interface IConfigurator<TResolver, TConnection> 
        where TResolver : IResolver<TResolver>
    {
        IConfigurator<TResolver, TConnection> Bus(Action<ServiceBusConfigurer<TConnection>> busConfigurer);
        TResolver GetResolver();
    }
}
