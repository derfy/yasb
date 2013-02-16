using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;

namespace Yasb.Wireup
{
    
    public  class AutofacConfigurator : IConfigurator
    {
        private class AutofacResolver : IResolver
        {
            private IContainer _container;
            public AutofacResolver(IContainer container)
            {
                _container = container;
            }
            public T InstanceOf<T>()
            {
                return _container.Resolve<T>();
            }
        }
        private ContainerBuilder _builder;
        
        public AutofacConfigurator()
        {
            _builder = new ContainerBuilder();
        }
        public IConfigurator Bus(Action<IServiceBusConfiguration> configurator)
        {
             _builder.RegisterType<Serializer>().As<ISerializer>();
            var configuration = new ServiceBusConfiguration();
            configurator(configuration);
            _builder.RegisterModule(new RedisModule(configuration));
            return this;
        }
        public IResolver Resolver()
        {
            return new AutofacResolver(_builder.Build());
        }

        
    }
}
