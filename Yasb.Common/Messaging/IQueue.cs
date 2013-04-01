using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue : IMessagesWrapper
    {
        void Initialize();
        bool TryGetEnvelope(TimeSpan delta,out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId);
        void Push(MessageEnvelope envelope);
        IEndPoint LocalEndPoint { get; }
    }
}
