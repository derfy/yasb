using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
   
    public class MessageEnvelope :IEnvelope
    {
        public MessageEnvelope()
        {

        }
       
        public MessageEnvelope(IMessage message, IEndPoint from, IEndPoint to)
        {
            Message = message;
            From = from;
            To = to;
        }
        public IMessage Message { get; private set; }

        public IEndPoint From { get; private set; }
        public IEndPoint To { get; private set; }
        public Type ContentType { get { return Message.GetType(); } }

        public string Id { get;  set; }

        public int RetriesNumber { get;set; }

        public long? StartTimestamp { get; set; }

        public string LastErrorMessage { get; set; }

        public DateTime? StartTime { get {
            if (!StartTimestamp.HasValue)
                return null;
            return DateTime.MinValue.Add(new TimeSpan(StartTimestamp.Value));
        } }
    }
}
