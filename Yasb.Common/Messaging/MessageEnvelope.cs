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

        public MessageEnvelope(IMessage message, string from, string to)
        {
            Message = message;
            From = from;
            To = to;
        }
        public IMessage Message { get; private set; }

        public string From { get; private set; }
        public string To { get; private set; }
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
