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
    public class ServiceBusModule<TEndPoint,TEndPointConfiguration> : ScopedModule<EndPointsConfigurer<TEndPointConfiguration>> 
        where TEndPointConfiguration : IEndPointConfiguration<TEndPoint>
   
    {
        public ServiceBusModule(EndPointsConfigurer<TEndPointConfiguration> configuration)
            : base(configuration, "bus")
        {
        }
        
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);


            builder.Register<Func<string, TEndPoint>>(c => endpoint => Configuration.GetEndPointByName(endpoint).Built);
            builder.RegisterWithScope<MessagesReceiver<TEndPoint>>((componentScope, parameters) =>
            {
                var queueFactory = componentScope.Resolve<IQueueFactory<TEndPoint>>();
                var endPointFactory = componentScope.Resolve<Func<string, TEndPoint>>();
                var localEndPoint = endPointFactory("local");
                var localQueue = queueFactory.CreateQueue(localEndPoint);
                return new MessagesReceiver<TEndPoint>(localQueue);
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<ServiceBus<TEndPoint>>((componentScope, parameters) =>
            {
                var endPointFactory = componentScope.Resolve<Func<string, TEndPoint>>();
                return new ServiceBus<TEndPoint>(endPointFactory, componentScope.Resolve<IQueueFactory<TEndPoint>>(), componentScope.Resolve<ISubscriptionService<TEndPoint>>(), componentScope.Resolve<MessagesReceiver<TEndPoint>>());
            }).As<IServiceBus<TEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

            
        }
    }
}
