using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Common.Messaging;
using Yasb.Common;
using Newtonsoft.Json;
using Yasb.Common.Messaging.EndPoints;
using Yasb.Redis.Messaging.Serialization.MessageDeserializers;

namespace Yasb.Wireup
{
    public class ServiceBusModule<TEndPoint, TSubscriptionServiceConfiguration> : ScopedModule<ServiceBusConfiguration<TEndPoint, TSubscriptionServiceConfiguration>> 
   
    {
        public ServiceBusModule(ServiceBusConfiguration<TEndPoint, TSubscriptionServiceConfiguration> configuration)
            : base(configuration, "bus")
        {
        }
        
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);

            
            builder.RegisterWithScope<MessagesReceiver<TEndPoint>>((componentScope, parameters) =>
            {
                var queueFactory = componentScope.Resolve<IQueueFactory<TEndPoint>>();
                var localQueue = queueFactory.CreateQueue(Configuration.EndPoints["local"]);
                return new MessagesReceiver<TEndPoint>(localQueue);
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<ServiceBus<TEndPoint>>((componentScope, parameters) =>
            {
                return new ServiceBus<TEndPoint>(Configuration.EndPoints, componentScope.Resolve<IQueueFactory<TEndPoint>>(), componentScope.Resolve<ISubscriptionService<TEndPoint>>(), componentScope.Resolve<MessagesReceiver<TEndPoint>>());
            }).As<IServiceBus<TEndPoint>>().InstancePerMatchingLifetimeScope(Scope);
            
            
        }
    }
}
