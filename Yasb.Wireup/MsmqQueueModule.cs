using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using System.Messaging;
using Yasb.Msmq.Messaging.Serialization;
using Yasb.Common.Serialization;
using Newtonsoft.Json;
using Yasb.Common.Serialization.MessageDeserializers;
using Yasb.Common.Messaging.EndPoints.Msmq;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{
    public class MsmqQueueModule : ScopedModule<ServiceBusConfiguration<MsmqEndPoint, MsmqSerializationConfiguration>>
    {
        public MsmqQueueModule(ServiceBusConfiguration<MsmqEndPoint, MsmqSerializationConfiguration> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            //builder.RegisterWithScope<ISerializer>((componentScope, parameters) =>
            //{
            //    return new JsonNetSerializer();
            //}).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<IMessageFormatter>(componentScope => new JsonMessageFormatter<MessageEnvelope>(componentScope.Resolve<ISerializer>()));

            builder.RegisterWithScope<MsmqQueueFactory>((componentScope, parameters) =>
            {
                return new MsmqQueueFactory(componentScope.Resolve<IMessageFormatter>());
            }).As<IQueueFactory<MsmqEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<Func<Type, IMessageDeserializer>>((componentScope) => type =>
            {
                if (!componentScope.IsRegisteredWithKey<IMessageDeserializer>(type))
                {
                    var genericType = typeof(DefaultMessageDeserializer<>).MakeGenericType(type);
                    return Activator.CreateInstance(genericType) as IMessageDeserializer;
                }
                return componentScope.ResolveKeyed<IMessageDeserializer>(type);
            }).InstancePerMatchingLifetimeScope(Scope);
           // builder.RegisterWithScope<MessageEnvelopeConverter>(componentScope => new MessageEnvelopeConverter(componentScope.Resolve<Func<Type, IMessageDeserializer>>())).As<JsonConverter>();
            
         
        }
    }
}
