using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{
    public class MsmqServiceBusModule : ServiceBusModule<MsmqConnection> 
    {
        public MsmqServiceBusModule(ServiceBusConfiguration<MsmqConnection> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService>((componentScope, parameters) =>
            {
                var localEndPointInfo = Configuration.EndPointConfiguration.GetEndPointInfoByName("local");
                var connection = Configuration.ConnectionConfiguration.GetConnectionByName(localEndPointInfo.ConnectionName);
                return new MsmqSubscriptionService(localEndPointInfo.QueueName);
            }).InstancePerMatchingLifetimeScope(Scope);
        }
    }
}
