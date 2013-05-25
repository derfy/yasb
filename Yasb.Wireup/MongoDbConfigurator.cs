using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Autofac;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class MongoDbConfigurator : AbstractConfigurator<MongoDbConnection>
    {
        protected override void RegisterServiceBusModule(Autofac.ContainerBuilder builder, Common.Messaging.Configuration.ServiceBusConfiguration<MongoDbConnection> serviceBusConfiguration)
        {
            builder.RegisterModule(new CommonModule<QueueConfiguration<MongoDbConnection>>(serviceBusConfiguration, "bus"));
            builder.RegisterModule(new MongoDbQueueModule(serviceBusConfiguration, "bus"));
            builder.RegisterModule(new MongoDbServiceBusModule(serviceBusConfiguration));
        }

        protected override void RegisterQueueModule(Autofac.ContainerBuilder builder, Common.Messaging.Configuration.QueueConfiguration<MongoDbConnection> queueConfiguration)
        {
            builder.RegisterModule(new CommonModule<QueueConfiguration<MongoDbConnection>>(queueConfiguration, "queue"));
            builder.RegisterModule(new MongoDbQueueModule(queueConfiguration, "queue"));
        }
    }
}
