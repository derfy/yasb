using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
   
    public class MessageEnvelope 
    {
       
        public MessageEnvelope()
        {

        }

        public MessageEnvelope(string envelopeId, IMessage message, string from, string to, string messageHandlerTypeName)
        {
            Id = envelopeId;
            Message = message;
            From = from;
            To = to;
            HandlerType = Type.GetType(messageHandlerTypeName);
        }

        public MessageEnvelope(string envelopeId, IMessage message, string from, string to, Type handlerType)
        {
            Id = envelopeId;
            Message = message;
            From = from;
            To = to;
            HandlerType = handlerType;
        }
        public IMessage Message { get; private set; }

        public string From { get; private set; }
        public string To { get; private set; }
        public string Id { get;  set; }

        public Type HandlerType { get; private set; }
        public Type ContentType { get { return Message.GetType(); } }
        public int RetriesNumber { get;set; }

        public long? StartTimestamp { get; set; }

        public string LastErrorMessage { get; set; }

        public long LastCreateOrUpdateTimestamp { get; set; }
        
        public DateTime? StartTime { get {
            if (!StartTimestamp.HasValue)
                return null;
            return DateTime.MinValue.Add(new TimeSpan(StartTimestamp.Value));
        } }
    }
}
