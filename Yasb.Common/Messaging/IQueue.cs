using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue 
    {
        bool TryGetEnvelope(DateTime now,TimeSpan timoutWindow,out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId);
        void Push(MessageEnvelope envelope);
    }
}
