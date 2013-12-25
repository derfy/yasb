using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging;
using Yasb.Common.Messaging.EndPoints.MongoDb;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.Wireup
{
    public class MongoDbServiceBusModule : ServiceBusModule<MongoDbEndPoint, MongoDbSerializationConfiguration> 
    {
        public MongoDbServiceBusModule(ServiceBusConfiguration<MongoDbEndPoint, MongoDbSerializationConfiguration> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService<MongoDbEndPoint>>((componentScope, parameters) =>
            {
                var localEndPoint = Configuration.EndPoints["local"];
                 return MongoDbFactory.CreateSubscriptionService(localEndPoint);
            });
        }
    }
}
