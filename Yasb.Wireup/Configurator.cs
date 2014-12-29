using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;
using Autofac.Core;

namespace Yasb.Wireup
{
    public class Configurator {
        public static Configurator<TEndPoint> Configure<TEndPoint>() 
        {
            return new Configurator<TEndPoint>();
        }
    }
    public class Configurator<TEndPoint>
    {
        internal Configurator()
        {

        }
        ContainerBuilder _builder = new ContainerBuilder();
        public IServiceBus<TEndPoint> Bus()
        {
            var lifetimeScope = _builder.Build().BeginLifetimeScope("bus");
            return lifetimeScope.Resolve<IServiceBus<TEndPoint>>();

        }

        internal void RegisterModule(IModule module)
        {
            _builder.RegisterModule(module);
        }

        
    }

   
}
