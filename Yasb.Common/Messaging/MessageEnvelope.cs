using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
   
    public class MessageEnvelope
    {
        
        public MessageEnvelope(IMessage message, Guid id, BusEndPoint from, BusEndPoint to)
        {
            Message = message;
            Id = id;
            From = from;
            To = to;
        }
        public IMessage Message { get; private set; }

        public BusEndPoint From { get; private set; }
        public BusEndPoint To { get; private set; }
        public Type ContentType { get { return Message.GetType(); } }

        public Guid Id { get; private set; }
    }
}
