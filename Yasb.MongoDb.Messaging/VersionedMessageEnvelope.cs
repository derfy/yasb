using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.MongoDb.Messaging
{
    public class VersionedMessageEnvelope : MessageEnvelope
    {
        public VersionedMessageEnvelope(IMessage msg,string from,string to):base(msg,from,to)
        {

        }
        public int Version { get; set; }

        internal static VersionedMessageEnvelope CopyFrom(MessageEnvelope envelope)
        {
            return new VersionedMessageEnvelope(envelope.Message, envelope.From, envelope.To) { Id=envelope.Id, StartTimestamp=envelope.StartTimestamp };
        }
    }
}
