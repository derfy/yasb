using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yasb.Common.Messaging;
using Yasb.Common;

namespace Yasb.Common.Messaging
{

    public class MessagesReceiver<TEndPoint> : IWorker, IDisposable 
    {
        private IQueue<TEndPoint> _queue;
        public MessagesReceiver(IQueue<TEndPoint> queue)
        {
            _queue = queue;
        }

        public virtual void Execute()
        {
            var delta = new TimeSpan(0, 0, 5);
            MessageEnvelope envelope = null;
            if (!(_queue.TryDequeue(DateTime.UtcNow, delta, out envelope)))
                return ;
            _queue.SetMessageCompleted(envelope.Id, DateTime.UtcNow);
        }
        
        public void OnException(Exception ex)
        {
            var handlerException = ex as MessageHandlerException;
            if (handlerException == null)
                return;
            _queue.SetMessageInError(handlerException.EnvelopeId,handlerException.Message);
        }
        public void OnCanceled()
        {
        }
        public void Dispose()
        {
          
        }


        
    }
}
