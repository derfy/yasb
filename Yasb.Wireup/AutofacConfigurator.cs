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
    public class RedisResolver : IResolver<RedisResolver>
    {
       
        private ILifetimeScope _lifetimeScope;
        private ServiceBusConfiguration _busConfiguration;
        public RedisResolver(ILifetimeScope lifetimeScope,ServiceBusConfiguration busConfiguration)
        {
            _lifetimeScope = lifetimeScope;
            _busConfiguration = busConfiguration;
        }
        public IServiceBus Bus()
        {
            return _lifetimeScope.Resolve<IServiceBus>();
        }
        public IQueue GetLocalQueue()
        {
            var factory = _lifetimeScope.Resolve<Func<IEndPoint, IQueue>>();
            return factory(_busConfiguration.LocalEndPoint);
        }
        public IQueue GetQueueByName(string endPointName)
        {
            var endPoint=_busConfiguration.NamedEndPoints.Where(e=>e.Name==endPointName).FirstOrDefault();
            if(endPoint==null)
                throw new ApplicationException(string.Format("No endPoint with name {0}",endPointName));
            var factory = _lifetimeScope.Resolve<Func<IEndPoint, IQueue>>();
            return factory(endPoint);
        }
        public RedisClient GetRedisClientByEndPoint(RedisEndPoint endPoint)
        {
            return _lifetimeScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(endPoint.ToIPEndPoint()));
        }
        public ScriptsCache ScriptsCacheFor(IEndPoint endPoint)
        {
            return _lifetimeScope.Resolve<ScriptsCache>(TypedParameter.From<IEndPoint>(endPoint));
        }
    }
    public class AutofacConfigurator : IConfigurator<RedisResolver>
    {
        
        private ContainerBuilder _builder;
        
        public AutofacConfigurator()
        {
            _builder = new ContainerBuilder();
        }
       

        public RedisResolver Configure(Action<ServiceBusConfiguration> configurator)
        {
            var configuration = new ServiceBusConfiguration();
            configurator(configuration);
            _builder.RegisterOneInstanceForObjectKey<EndPoint, RedisClient>((endPoint,context) => new RedisClient(context.Resolve<RedisSocket>(TypedParameter.From<EndPoint>(endPoint))));
            _builder.RegisterModule(new RedisModule(configuration));
            _builder.Register<RedisResolver>(c => new RedisResolver(c.Resolve<ILifetimeScope>().BeginLifetimeScope("bus"), configuration));
            return _builder.Build().Resolve<RedisResolver>();
        }
    }
}
