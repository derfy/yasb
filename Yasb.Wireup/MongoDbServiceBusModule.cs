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
    public class MongoDbServiceBusModule : ServiceBusModule<MongoDbEndPoint,MongoDbEndPointConfiguration> 
    {
        public MongoDbServiceBusModule(EndPointsConfigurer<MongoDbEndPointConfiguration> serviceBusConfiguration)
            : base(serviceBusConfiguration)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            //builder.RegisterWithScope<ISubscriptionService<MongoDbEndPointConfiguration>>((componentScope, parameters) =>
            //{
            //    var localEndPoint = Configuration.EndPoints["local"];
            //     return MongoDbFactory.CreateSubscriptionService(localEndPoint.Built);
            //});
        }
    }
}
