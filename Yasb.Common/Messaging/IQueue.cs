using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue:IDisposable
    {
        void Initialize();
        MessageEnvelope GetMessage(TimeSpan delta);
        bool TrySetMessageInProgress(Guid envelopeId);
        void SetMessageCompleted(Guid envelopeId);
        void SetMessageError(Guid envelopeId);
        void Push(MessageEnvelope envelope);
    }
}
