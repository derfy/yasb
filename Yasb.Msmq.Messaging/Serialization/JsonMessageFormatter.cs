using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;
using Yasb.Common.Serialization;

namespace Yasb.Msmq.Messaging.Serialization {
    public class JsonMessageFormatter<T> : IMessageFormatter
    {
        
        private readonly ISerializer _serializer;

       
        public JsonMessageFormatter(ISerializer serializer)
        {
           _serializer = serializer;
        }


        public bool CanRead(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var stream = message.BodyStream;

            return stream != null
                && stream.CanRead
                && stream.Length > 0;
        }

        public object Clone()
        {
            return new JsonMessageFormatter<T>(_serializer);
        }

        public object Read(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (CanRead(message) == false)
                return null;

            return _serializer.Deserialize<T>(message.BodyStream);
            
        }

        public void Write(Message message, object obj)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (obj == null)
                throw new ArgumentNullException("obj");

            if(!(obj is T))
                throw new InvalidCastException("obj");

            var bytes = _serializer.Serialize<T>((T)obj);

            message.BodyStream = new MemoryStream(bytes);

            //Need to reset the body type, in case the same message
            //is reused by some other formatter.
            message.BodyType = 0;
        }
    }
}

