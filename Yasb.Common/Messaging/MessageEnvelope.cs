using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Yasb.Common.Tests.Messages;

namespace Yasb.Common.Messaging
{
    public class MessageEnvelope
    {
        private Dictionary<string, object> _headers = new Dictionary<string, object>();
        public MessageEnvelope()
        {

        }
        public MessageEnvelope(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; set; }

        public string Id { get; set; }

        public int RetriesNumber { get; set; }

        public string LastErrorMessage { get; set; }

        public long? StartTimestamp { get; set; }

        public DateTime? StartTime
        {
            get
            {
                if (!StartTimestamp.HasValue)
                    return null;
                return DateTime.MinValue.Add(new TimeSpan(StartTimestamp.Value));
            }
        }
        public void SetHeader<TValue>(string name, TValue value)
        {
            _headers[name] = value;
        }

        public TValue GetHeader<TValue>(string name)
        {
            object value = default(TValue);
            return _headers.TryGetValue(name, out value) ? (TValue)value : default(TValue);
        }


        
    }
   
}
