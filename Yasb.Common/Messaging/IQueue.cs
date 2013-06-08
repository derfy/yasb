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
        void SetMessageInError(string envelopeId,string errorMessage);
        void Push(IMessage message, string from);
        void Clear();
        string LocalEndPoint { get; }
    }
}
