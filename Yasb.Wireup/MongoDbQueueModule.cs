using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging;
using Yasb.MongoDb.Messaging.Serialization;
using Yasb.Common.Messaging.EndPoints.MongoDb;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.Wireup
{
    public class MongoDbQueueModule : ScopedModule<ServiceBusConfiguration<MongoDbEndPoint, MongoDbSerializationConfiguration>>
    {
        public MongoDbQueueModule(ServiceBusConfiguration<MongoDbEndPoint, MongoDbSerializationConfiguration> queueConfiguration, string scope)
            : base(queueConfiguration, scope)
        {
           
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<MongoDbQueueFactory>((componentScope, parameters) =>
            {
                return new MongoDbQueueFactory();
            }).As<IQueueFactory<MongoDbEndPoint>>().InstancePerMatchingLifetimeScope(Scope);

            SerializerRegisterer.Register<MessageEnvelope>();
        }
    }
}
