using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using System.Messaging;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Xml;
using Yasb.Msmq.Messaging.Configuration;
namespace Yasb.Msmq.Messaging
{
    public class MsmqQueueFactory :IQueueFactory<MsmqEndPointConfiguration>
    {
        private AbstractXmlSerializer<MessageEnvelope> _envelopeSerializer;
        public MsmqQueueFactory(AbstractXmlSerializer<MessageEnvelope> envelopeSerializer)
        {
            _envelopeSerializer = envelopeSerializer;
        }
        public IQueue<MsmqEndPointConfiguration> CreateQueue(MsmqEndPointConfiguration endPoint)
        {
            return new MsmqQueue(endPoint,_envelopeSerializer);
        }
    }
}
