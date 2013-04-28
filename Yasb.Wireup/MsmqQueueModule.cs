using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Yasb.Common.Messaging.Configuration;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Msmq.Messaging;
using System.Messaging;
using Yasb.Msmq.Messaging.Serialization;
using Yasb.Common.Serialization;

namespace Yasb.Wireup
{
    public class MsmqQueueModule : CommonModule<QueueConfiguration<MsmqConnection>>
    {
        public MsmqQueueModule(QueueConfiguration<MsmqConnection> queueConfiguration, string scope)
            : base(queueConfiguration,scope)
        {
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<IMessageFormatter>(componentScope => new JsonMessageFormatter<MessageEnvelope>(componentScope.Resolve<ISerializer>()));

            builder.RegisterWithScope<IQueueFactory>((componentScope, parameters) =>
            {
                return new MsmqQueueFactory(Configuration, componentScope.Resolve<IMessageFormatter>());
            }).InstancePerMatchingLifetimeScope(Scope);
            
         
        }
    }
}
