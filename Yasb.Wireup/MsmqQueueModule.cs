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
    public class MsmqQueueModule : CommonModule<EndPointConfiguration<MsmqConnectionConfiguration>>
    {
        public MsmqQueueModule(EndPointConfiguration<MsmqConnectionConfiguration> queueConfiguration)
            : base(queueConfiguration,"queue")
        {
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterWithScope<IMessageFormatter>(componentScope => new JsonMessageFormatter<MessageEnvelope>(componentScope.Resolve<ISerializer>()));
            builder.RegisterWithScope<IQueue>((componentScope, parameters) =>
            {
                var endPoint = parameters.Named<BusEndPoint>("endPoint");
                var connection = Configuration.GetConnectionByName(endPoint.ConnectionName);
                var connectionString = string.Format(@"{0}\{1}$\{2}", connection.Host, connection.IsPrivate ? "Private":"Public",endPoint.QueueName);
                return new MsmqQueue(connectionString, componentScope.Resolve<IMessageFormatter>());
            });
        }
    }
}
