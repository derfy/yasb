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
using Yasb.Common.Serialization;
using Yasb.Common.Serialization.MessageDeserializers;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Wireup
{
    public class ServiceBusModule<TEndPoint, TSerializer> : ScopedModule<ServiceBusConfiguration<TEndPoint, TSerializer>> 
   
    {
        public ServiceBusModule(ServiceBusConfiguration<TEndPoint, TSerializer> configuration)
            : base(configuration, "bus")
        {
        }

        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<WorkerPool<MessageEnvelope>>(componentScope => new WorkerPool<MessageEnvelope>(componentScope.Resolve<IWorker<MessageEnvelope>>())).As<IWorkerPool<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<MessagesReceiver<TEndPoint>>((componentScope, parameters) =>
            {
                var queueFactory = componentScope.Resolve<IQueueFactory<TEndPoint>>();
                var localQueue = queueFactory.CreateQueue(Configuration.EndPoints["local"]);
                return new MessagesReceiver<TEndPoint>(localQueue, componentScope.Resolve<IMessageDispatcher>());
            }).As<IWorker<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<TSerializer>((componentScope, parameters) =>
            {
                return Configuration.Serializer;
            })
            .As<ISerializer>()
            .InstancePerMatchingLifetimeScope(Scope);

          

            builder.RegisterWithScope<ServiceBus<TEndPoint>>((componentScope, parameters) =>
            {
                return new ServiceBus<TEndPoint>(Configuration.EndPoints, componentScope.Resolve<IQueueFactory<TEndPoint>>(), componentScope.Resolve<ISubscriptionService<TEndPoint>>(), componentScope.Resolve<IWorkerPool<MessageEnvelope>>());
            }).As<IServiceBus<TEndPoint>>().InstancePerMatchingLifetimeScope(Scope);



            builder.RegisterWithScope<Func<Type, IEnumerable<IHandleMessages>>>((componentScope, p) =>
            {
                return type =>
                {
                    var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                    var enumerableGenericType = typeof(IEnumerable<>).MakeGenericType(genericType);
                    return componentScope.Resolve(enumerableGenericType) as IEnumerable<IHandleMessages>;
                };

            }).InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<MessageDispatcher>((componentScope, parameters) =>
            {
                return new MessageDispatcher(componentScope.Resolve<Func<Type, IEnumerable<IHandleMessages>>>());
            }).As<IMessageDispatcher>().InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<Func<Type, IMessageDeserializer>>((componentScope) => type =>
            {
                if (!componentScope.IsRegisteredWithKey<IMessageDeserializer>(type))
                {
                    var genericType = typeof(DefaultMessageDeserializer<>).MakeGenericType(type);
                    return Activator.CreateInstance(genericType) as IMessageDeserializer;
                }
                return componentScope.ResolveKeyed<IMessageDeserializer>(type); 
            }).InstancePerMatchingLifetimeScope(Scope);

            //builder.RegisterType<SubscriptionMessageDeserializer<TConnection>>().Keyed<IMessageDeserializer>(typeof(SubscriptionMessage<TConnection>))
            //   .As<IMessageDeserializer>()
            //   .InstancePerMatchingLifetimeScope(Scope);
           // builder.RegisterWithScope<MessageEnvelopeConverter>(componentScope => new MessageEnvelopeConverter(componentScope.Resolve<Func<Type, IMessageDeserializer>>())).As<JsonConverter>();
        }
    }
}
