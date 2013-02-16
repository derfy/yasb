using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public class MessageHandlerException : Exception
    {
        public MessageHandlerException(Guid envelopeId,string message):base(message)
        {
            EnvelopeId = envelopeId;
        }
        public Guid EnvelopeId { get; private set; }
    }
}
