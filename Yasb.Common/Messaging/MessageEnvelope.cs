using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

       // public TEndPoint From { get;  set; }

        public string Id
        {
            get { return GetHeader<string>("Id"); }
            set { SetHeader("Id", value); }
        }
        
        public int RetriesNumber
        {
            get { return GetHeader<int>("RetriesNumber"); }
            set { SetHeader("RetriesNumber", value); }
        }

        public string LastErrorMessage
        {
            get { return GetHeader<string>("LastErrorMessage"); }
            set { SetHeader("LastErrorMessage", value); }
        }

        public long? StartTimestamp
        {
            get { return GetHeader<long?>("StartTimestamp"); }
            set { SetHeader("StartTimestamp", value); }
        }

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
            object value=default(TValue);
            return _headers.TryGetValue(name,out value) ? (TValue)value : default(TValue);
        }

        public Type HandlerType { get; private set; }


    }
}
