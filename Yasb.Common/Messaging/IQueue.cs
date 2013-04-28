using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue 
    {
        bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId);
        void Push(MessageEnvelope envelope);

        string LocalEndPoint { get; }
    }
}
