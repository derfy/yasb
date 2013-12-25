using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.EndPoints.Msmq;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{

    public class MsmqServiceBusModule : ServiceBusModule<MsmqEndPoint,MsmqSerializationConfiguration> 
    {
        public MsmqServiceBusModule(ServiceBusConfiguration<MsmqEndPoint, MsmqSerializationConfiguration> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<MsmqSubscriptionService>((componentScope, parameters) =>
            {
                var localEndPoint = Configuration.EndPoints["local"];
                return new MsmqSubscriptionService(localEndPoint);
            })
           .As<ISubscriptionService<MsmqEndPoint>>();
           
        }
    }
}
