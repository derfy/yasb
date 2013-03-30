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
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Client;

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
            var configuration = new ServiceBusConfiguration<RedisEndPoint, RedisEndPointConfiguration>();
            configurator(configuration);
            _builder.RegisterOneInstanceForObjectKey<EndPoint, IRedisSocketAsyncEventArgsPool>((endPoint) => new RedisSocketAsyncEventArgsPool(10, endPoint));
            _builder.RegisterModule(new RedisModule(configuration));
            return this;
        }
        public IResolver Resolver()
        {
            return new AutofacResolver(_builder.Build());
        }



      
    }
}
