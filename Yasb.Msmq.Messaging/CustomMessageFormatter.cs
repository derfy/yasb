using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Xml;

namespace Yasb.Msmq.Messaging
{
    public class CustomMessageFormatter : IMessageFormatter 
    {
        private AbstractXmlSerializer<MessageEnvelope> _serializer;
        public CustomMessageFormatter(AbstractXmlSerializer<MessageEnvelope> serializer)
        {
            _serializer = serializer;
        }
        public bool CanRead(Message message)
        {
            return true;
        }

        public object Read(Message msg)
        {
           return _serializer.Deserialize(msg.BodyStream);
        }

        public void Write(Message msg, object obj)
        {
            //Create a new MemoryStream object passing the buffer.
            msg.BodyStream = new MemoryStream();
            _serializer.Serialize(msg.BodyStream, (MessageEnvelope)obj); 

        }

        public object Clone()
        {
            return new CustomMessageFormatter(_serializer);
        }
    }
}
