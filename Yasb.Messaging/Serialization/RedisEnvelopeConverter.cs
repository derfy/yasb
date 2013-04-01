using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging.Serialization
{
    public class RedisEnvelopeConverter : MessageEnvelopeConverter<RedisEnvelope>
    {
        protected override RedisEnvelope CreateEnvelope(IMessage message, string id, IEndPoint from, IEndPoint to)
        {
            return new RedisEnvelope(message, id, from, to);
        }
    }
}
