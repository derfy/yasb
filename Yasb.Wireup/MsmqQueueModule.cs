using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using System.Messaging;
using Yasb.Msmq.Messaging.Serialization;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging.Configuration.Msmq;

namespace Yasb.Wireup
{
    public class MsmqQueueModule : ScopedModule<QueueConfiguration<MsmqConnection>>
    {
        public MsmqQueueModule(QueueConfiguration<MsmqConnection> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<IMessageFormatter>(componentScope => new JsonMessageFormatter<MessageEnvelope>(componentScope.Resolve<ISerializer>()));

            builder.RegisterWithScope<AbstractQueueFactory<MsmqConnection>>((componentScope, parameters) =>
            {
                return new MsmqQueueFactory(Configuration, componentScope.Resolve<IMessageFormatter>());
            }).InstancePerMatchingLifetimeScope(Scope);
            
         
        }
    }
}
