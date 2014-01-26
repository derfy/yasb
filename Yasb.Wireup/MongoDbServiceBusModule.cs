using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class MongoDbServiceBusModule : ServiceBusModule<MongoDbEndPointConfiguration, SubscriptionServiceConfiguration> 
    {
        public MongoDbServiceBusModule(ServiceBusConfiguration<MongoDbEndPointConfiguration, SubscriptionServiceConfiguration> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService<MongoDbEndPointConfiguration>>((componentScope, parameters) =>
            {
                var localEndPoint = Configuration.EndPoints["local"];
                 return MongoDbFactory.CreateSubscriptionService(localEndPoint);
            });
        }
    }
}
