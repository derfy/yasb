using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging;
using Autofac;
using Yasb.Common.Messaging;
using System.Messaging;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.Common.Tests.Messages;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Xml;
using Yasb.Common.Messaging.Serialization.Json;

namespace Yasb.Wireup
{

    public class MsmqServiceBusModule : ServiceBusModule<MsmqEndPoint,MsmqEndPointConfiguration> 
    {
        public MsmqServiceBusModule(EndPointsConfigurer<MsmqEndPointConfiguration> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
           // builder.RegisterWithScope<MsmqSubscriptionService>((componentScope, parameters) =>
           // {
           //     Configuration.SubscriptionServiceConfiguration.LocalEndPointConfiguration = Configuration.EndPoints["local"];
           //     return new MsmqSubscriptionService(Configuration.SubscriptionServiceConfiguration);
           // })
           //.As<ISubscriptionService<MsmqEndPointConfiguration>>();

            

            builder.RegisterWithScope<Func<Type, AbstractXmlSerializer<IMessage>>>((componentScope, parameters) =>
            {
                return (type) => new DefaultXmlMessageSerializer(type);
            }).InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<XmlMessageEnvelopeSerializer>((componentScope, parameters) =>
            {
                return new XmlMessageEnvelopeSerializer(componentScope.Resolve<Func<Type, AbstractXmlSerializer<IMessage>>>());
            }).As<AbstractXmlSerializer<MessageEnvelope>>().InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<MsmqQueueFactory>((componentScope, parameters) =>
            {
                return new MsmqQueueFactory(componentScope.Resolve<AbstractXmlSerializer<MessageEnvelope>>());
            }).As<IQueueFactory<MsmqEndPointConfiguration>>().InstancePerMatchingLifetimeScope(Scope);
        }
    }
}
