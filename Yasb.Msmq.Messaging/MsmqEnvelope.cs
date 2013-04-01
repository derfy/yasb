using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Msmq.Messaging
{
    public class MsmqEnvelope : MessageEnvelope
    {
        public MsmqEnvelope(IMessage message, string id, IEndPoint from,IEndPoint to)
            : base(message, id, from, to)
        {}
    }
}
