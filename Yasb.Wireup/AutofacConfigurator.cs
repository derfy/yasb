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
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Client;

namespace Yasb.Wireup
{
    public class Resolver : IResolver<Resolver>
    {
       
        private ILifetimeScope _lifetimeScope;
        private ServiceBusConfiguration<EndPoint> _serviceBusConfiguration;
        public Resolver(ILifetimeScope lifetimeScope, ServiceBusConfiguration<EndPoint> serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
            _lifetimeScope = lifetimeScope;
        }
        public IServiceBus Bus()
        {
            return _lifetimeScope.Resolve<IServiceBus>();
        }
        public IQueue GetLocalQueue()
        {
            var factory = _lifetimeScope.Resolve<QueueFactory>();
            return factory(_serviceBusConfiguration.EndPointConfiguration.LocalEndPoint);
        }
        public IQueue GetQueueByName(string endPointName)
        {
            var endPoint = _serviceBusConfiguration.EndPointConfiguration.NamedEndPoints.Where(e => e.Name == endPointName).FirstOrDefault();
            if(endPoint==null)
                throw new ApplicationException(string.Format("No endPoint with name {0}",endPointName));
            var factory = _lifetimeScope.Resolve<QueueFactory>();
            return factory(endPoint);
        }
        public RedisClient GetRedisClientByEndPoint(BusEndPoint endPoint)
        {
            var connection = _serviceBusConfiguration.GetConnectionByName(endPoint.ConnectionName);
            return _lifetimeScope.Resolve<RedisClient>(TypedParameter.From<EndPoint>(connection));
        }
        public ScriptsCache ScriptsCacheFor(BusEndPoint endPoint)
        {
            return _lifetimeScope.Resolve<ScriptsCache>(TypedParameter.From<BusEndPoint>(endPoint));
        }
    }



    public class AutofacConfigurator : IConfigurator<Resolver, EndPoint> 
    {
       protected ContainerBuilder Builder{get;private set;}
        
        public AutofacConfigurator()
        {
            Builder = new ContainerBuilder();
        }

        public IConfigurator<Resolver, EndPoint> ConfigureServiceBus(Action<ServiceBusConfigurer<EndPoint>> busConfigurer)
        {
            var serviceBusConfigurer = new ServiceBusConfigurer<EndPoint>();
            busConfigurer(serviceBusConfigurer);
            Builder.RegisterModule(new CommonModule(serviceBusConfigurer.Built));
            Builder.RegisterModule(new RedisModule(serviceBusConfigurer.Built));

            
            Builder.Register<Resolver>(c =>
            {
                var lifetimeScope = c.Resolve<ILifetimeScope>();
                return new Resolver(c.Resolve<ILifetimeScope>().BeginLifetimeScope("bus"), serviceBusConfigurer.Built);
            });
            
            return this;
        }

         public Resolver Configure()
        {
            return Builder.Build().Resolve<Resolver>();
        }

    }
}
