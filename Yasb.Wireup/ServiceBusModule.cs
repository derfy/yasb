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

namespace Yasb.Wireup
{
    public class ServiceBusModule<TConnection> : ScopedModule<ServiceBusConfiguration< TConnection>>
    {
        public ServiceBusModule(ServiceBusConfiguration<TConnection> configuration)
            : base(configuration, "bus")
        {
        }

        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
           
          
            builder.RegisterWithScope<MessageHandlerFactory>((componentScope, p) =>
            {
                return type =>
                {
                    var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                    var enumerableGenericType = typeof(IEnumerable<>).MakeGenericType(genericType);
                    return componentScope.Resolve(enumerableGenericType) as IEnumerable<IHandleMessages>;
                };

            }).InstancePerMatchingLifetimeScope(Scope);

            
           
           

            builder.RegisterWithScope<WorkerPool<MessageEnvelope>>(componentScope => new WorkerPool<MessageEnvelope>(componentScope.Resolve<IWorker<MessageEnvelope>>())).As<IWorkerPool<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);



            if (Configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(Configuration.MessageHandlersAssembly)
                      .AsClosedTypesOf(typeof(IHandleMessages<>))
                      .AsImplementedInterfaces();
            }

            builder.RegisterWithScope<MessageDispatcher<TConnection>>((componentScope, parameters) =>
            {
                return new MessageDispatcher<TConnection>(componentScope.Resolve<MessageHandlerFactory>());
            }).As<IMessageDispatcher>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<MessagesReceiver<TConnection>>((componentScope, parameters) =>
            {
                return new MessagesReceiver<TConnection>(componentScope.Resolve<AbstractQueueFactory<TConnection>>(), componentScope.Resolve<IMessageDispatcher>());
            }).As<IWorker<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<ServiceBus<TConnection>>((componentScope, parameters) =>
            {
                return new ServiceBus<TConnection>(componentScope.Resolve<AbstractQueueFactory<TConnection>>(), componentScope.Resolve<ISubscriptionService<TConnection>>(),componentScope.Resolve<IWorkerPool<MessageEnvelope>>(), componentScope.Resolve<MessageHandlerFactory>());
            }).As<IServiceBus<TConnection>>().InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<MessageDeserializerFactory>((componentScope) => type => {
                if (!componentScope.IsRegisteredWithKey<IMessageDeserializer>(type))
                {
                    var genericType = typeof(DefaultMessageDeserializer<>).MakeGenericType(type);
                    return Activator.CreateInstance(genericType) as IMessageDeserializer;
                }
                return componentScope.ResolveKeyed<IMessageDeserializer>(type); 
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterType<SubscriptionMessageDeserializer<TConnection>>().Keyed<IMessageDeserializer>(typeof(SubscriptionMessage<TConnection>))
               .As<IMessageDeserializer>()
               .InstancePerMatchingLifetimeScope(Scope);
            builder.RegisterWithScope<MessageEnvelopeConverter>(componentScope => new MessageEnvelopeConverter(componentScope.Resolve<MessageDeserializerFactory>())).As<JsonConverter>();
        }
    }
}
