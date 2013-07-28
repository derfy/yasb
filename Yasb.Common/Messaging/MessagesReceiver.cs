using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization;
using Yasb.Common;

namespace Yasb.Common.Messaging
{
    
    public class MessagesReceiver<TConnection> : IWorker<MessageEnvelope>, IDisposable 
    {
        private IQueue<TConnection> _queue;
        private IMessageDispatcher _messageDispatcher;
        public MessagesReceiver(AbstractQueueFactory<TConnection> queueFactory,IMessageDispatcher messageDispatcher)
        {
            _queue = queueFactory.CreateFromEndPointName("local");
            _messageDispatcher = messageDispatcher;
        }

        public virtual MessageEnvelope Execute()
        {
            var delta = new TimeSpan(0, 0, 5);
            MessageEnvelope envelope = null;
            if (!(_queue.TryDequeue(DateTime.UtcNow, delta, out envelope)))
                return null;
            if (!_messageDispatcher.TryDispatch(envelope))
                return null;
            return envelope;
        }
        public void OnCompleted(MessageEnvelope env)
        {
           _queue.SetMessageCompleted(env.Id, DateTime.UtcNow);
        }
        public void OnException(Exception ex)
        {
            var handlerException = ex as MessageHandlerException;
            if (handlerException == null)
                return;
            _queue.SetMessageInError(handlerException.EnvelopeId,handlerException.Message);
        }

        public void Dispose()
        {
          
        }
    }
}
