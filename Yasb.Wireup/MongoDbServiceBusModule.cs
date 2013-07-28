using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging;

namespace Yasb.Wireup
{
    public class MongoDbServiceBusModule : ServiceBusModule<MongoDbConnection> 
    {
        public MongoDbServiceBusModule(ServiceBusConfiguration<MongoDbConnection> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<ISubscriptionService<MongoDbConnection>>((componentScope, parameters) =>
            {
                var localEndPointInfo = Configuration.EndPointConfiguration.GetEndPointInfoByName("local");
                var connection = Configuration.ConnectionConfiguration.GetConnectionByName(localEndPointInfo.ConnectionName);
                return MongoDbFactory.CreateSubscriptionService(connection);
            });
        }
    }
}
