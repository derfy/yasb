using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public interface IQueue<TConnection> 
    {
        bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope);
        void SetMessageCompleted(string envelopeId, DateTime now);
        void SetMessageInError(string envelopeId,string errorMessage);
        MessageEnvelope CreateMessageEnvelope(IMessage message, QueueEndPoint<TConnection> from,string messageHandler);
        void Push(MessageEnvelope envelope);
        void Clear();
        QueueEndPoint<TConnection> LocalEndPoint { get; }
    }
}
