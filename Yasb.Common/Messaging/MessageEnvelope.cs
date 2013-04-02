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
       
        public MessageEnvelope(IMessage message, string id, IEndPoint from, IEndPoint to)
        {
            Message = message;
            Id = id;
            From = from;
            To = to;
        }
        public IMessage Message { get; private set; }

        public IEndPoint From { get; private set; }
        public IEndPoint To { get; private set; }
        public Type ContentType { get { return Message.GetType(); } }

        public string Id { get; private set; }

        public int RetriesNumber { get;set; }

        public long? StartTime { get; set; }

        public DateTime? StartTimestamp { get {
            if (!StartTime.HasValue)
                return null;
            return DateTime.MinValue.Add(new TimeSpan(StartTime.Value));
        } }
    }
}
