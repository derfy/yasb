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
    public class MessagesReceiver : IWorker,IDisposable
    {
        Func<Type, IEnumerable<IHandleMessages>> _handlerRegistrar;
        private IQueue _queue;
        private MessageDispatcher _messageDispatcher;
        public MessagesReceiver(IQueue queue,  Func<Type, IEnumerable<IHandleMessages>> handlerRegistrar)
        {
            _queue = queue;
            _handlerRegistrar = handlerRegistrar;
           _messageDispatcher=new MessageDispatcher();
        }

        public virtual void Execute(CancellationToken token)
        {
            
            var delta = new TimeSpan(0, 0, 5);
            _queue.Initialize();
            while (true)
            {
                token.ThrowIfCancellationRequested();
                var envelope = _queue.GetMessage(delta);
                if (envelope == null || !_queue.TrySetMessageInProgress(envelope.Id))
                    continue;
                try
                {
                    var handlers = _handlerRegistrar(envelope.ContentType);
                    var mhi = _messageDispatcher.GetHandlersFor(envelope.ContentType);
                    foreach (var handler in handlers)
                    {
                        mhi.HandleMethod(handler, envelope.Message);
                    }
                    _queue.SetMessageCompleted(envelope.Id);
                }
                catch (Exception ex)
                {
                    throw new MessageHandlerException(envelope.Id, ex.Message); ;
                }
            }

        }

        public void OnException(Exception ex)
        {
            var handlerException = ex as MessageHandlerException;
            if (handlerException == null)
                return;
            Console.WriteLine("error on processing message : " + handlerException.Message);
            _queue.SetMessageError(handlerException.EnvelopeId);
        }

        public void Dispose()
        {
          
        }
    }
}
