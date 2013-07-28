using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging;
using Yasb.MongoDb.Messaging.Serialization;

namespace Yasb.Wireup
{
    public class MongoDbQueueModule : ScopedModule<QueueConfiguration<MongoDbConnection>>
    {
        public MongoDbQueueModule(QueueConfiguration<MongoDbConnection> queueConfiguration, string scope)
            : base(queueConfiguration, scope)
        {
           
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterWithScope<AbstractQueueFactory<MongoDbConnection>>((componentScope, parameters) =>
            {
                return new MongoDbQueueFactory(Configuration);
            }).InstancePerMatchingLifetimeScope(Scope);

            SerializerRegisterer.Register<MessageEnvelope>();
        }
    }
}
