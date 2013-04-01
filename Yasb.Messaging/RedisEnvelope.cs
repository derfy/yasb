using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging
{
    public class RedisEnvelope : MessageEnvelope
    {
        public RedisEnvelope(IMessage message, string id, IEndPoint from, IEndPoint to):base(message,id,from,to)
        {}
    }
}
