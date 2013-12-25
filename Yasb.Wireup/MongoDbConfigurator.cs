using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging.Configuration;
using Yasb.MongoDb.Messaging;
using Yasb.Common.Messaging.EndPoints.MongoDb;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.Wireup
{
    public class MongoDbConfigurator : AbstractConfigurator<MongoDbEndPoint, MongoDbSerializationConfiguration>
    {
        protected override void RegisterServiceBusModule(Autofac.ContainerBuilder builder, ServiceBusConfiguration<MongoDbEndPoint, MongoDbSerializationConfiguration> serviceBusConfiguration)
        {
            builder.RegisterModule(new MongoDbServiceBusModule(serviceBusConfiguration));
        }

        protected override void RegisterQueueModule(Autofac.ContainerBuilder builder, ServiceBusConfiguration<MongoDbEndPoint, MongoDbSerializationConfiguration> queueConfiguration)
        {
           builder.RegisterModule(new MongoDbQueueModule(queueConfiguration, "queue"));
        }
    }
}
