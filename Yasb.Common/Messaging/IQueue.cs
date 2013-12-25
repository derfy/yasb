using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue<TEndPoint> 
    {
        bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId, DateTime now);
        void SetMessageInError(string envelopeId,string errorMessage);
        void Push(MessageEnvelope endPoint);
        void Clear();
        TEndPoint LocalEndPoint { get; }
    }
}
