using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue<TEndPointConfiguration> 
    {
        bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId, DateTime now);
        void SetMessageInError(string envelopeId,string errorMessage);
        void Push(IMessage message);
        void Clear();
        TEndPointConfiguration LocalEndPointConfiguration { get; }
    }
}
