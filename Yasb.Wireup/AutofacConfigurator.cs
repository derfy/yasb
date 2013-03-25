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
using Yasb.Redis.Messaging.Configuration;
using Yasb.Redis.Messaging;
using Newtonsoft.Json;
using Yasb.Redis.Messaging.Serialization;

namespace Yasb.Wireup
{

    public class AutofacConfigurator : IConfigurator<RedisEndPoint,RedisEndPointConfiguration>
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

            public void Dispose()
            {
                _container.Dispose();
            }
        }
        private ContainerBuilder _builder;
        
        public AutofacConfigurator()
        {
            _builder = new ContainerBuilder();
        }
        public IConfigurator<RedisEndPoint, RedisEndPointConfiguration> Bus(Action<ServiceBusConfiguration<RedisEndPoint, RedisEndPointConfiguration>> configurator)
        {
            _builder.RegisterType<Serializer>().WithParameter(TypedParameter.From<JsonConverter[]>(new JsonConverter[]{new RedisEndPointConverter(),new MessageEnvelopeConverter<RedisEndPoint>()})).As<ISerializer>();
            var configuration = new ServiceBusConfiguration<RedisEndPoint, RedisEndPointConfiguration>();
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
