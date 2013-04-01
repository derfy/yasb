using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;

namespace Yasb.Msmq.Messaging.Serialization
{
    public class MsmqEnvelopeConverter : MessageEnvelopeConverter<MsmqEnvelope>
    {
        protected override MsmqEnvelope CreateEnvelope(IMessage message, string id, IEndPoint from, IEndPoint to)
        {
            return new MsmqEnvelope(message, id, from, to);
        }
    }
}
