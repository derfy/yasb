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
    public delegate IEnumerable<IHandleMessages> MessageHandlerFactory(Type type);
    public class MessagesReceiver : IWorker, IDisposable 
    {
        private MessageHandlerFactory _handlerRegistrar;
        private IQueue _queue;
        private MessageDispatcher _messageDispatcher;
        public MessagesReceiver(IQueue queue, MessageHandlerFactory handlerRegistrar)
        {
            _queue = queue;
            _handlerRegistrar = handlerRegistrar;
           _messageDispatcher=new MessageDispatcher();
        }

        public virtual void Execute(CancellationToken token)
        {
            var delta = new TimeSpan(0, 0, 5);
            token.ThrowIfCancellationRequested();

            using (var task = Task.Factory.StartNew<MessageEnvelope>(() =>
            {
                MessageEnvelope envelope = null;
                if (!(_queue.TryDequeue(DateTime.UtcNow, delta, out envelope)))
                    return null;
                var handlers = _handlerRegistrar(envelope.ContentType).ToArray();
                var mhi = _messageDispatcher.GetHandleMethodDelegate(envelope.ContentType);
                foreach (var handler in handlers)
                {
                    try { mhi(handler, envelope.Message); }
                    catch (Exception ex)
                    {
                        throw new MessageHandlerException(envelope.Id, ex.Message);
                    }
                }
                return envelope;
            }, token))
            {
                var continuationTasks = new[]{ 
                    task.ContinueWith(t =>
                    {
                        var env = t.Result;
                        if (env != null)
                        {
                            _queue.SetMessageCompleted(env.Id);
                        }
                    }, TaskContinuationOptions.OnlyOnRanToCompletion),
                    task.ContinueWith(t =>
                    {
                        t.Exception.Handle(ex =>
                        {
                            var exception = ex as MessageHandlerException;
                            if (exception != null)
                            {
                                _queue.SetMessageInError(exception.EnvelopeId, exception.Message);
                            }
                            return true;
                        });

                    }, TaskContinuationOptions.OnlyOnFaulted)};
                Task.WaitAny(continuationTasks);
            }
           
        }

        public void OnException(Exception ex)
        {
            var handlerException = ex as MessageHandlerException;
            if (handlerException == null)
                return;
           // Console.WriteLine("On exception   .......  " + handlerException.EnvelopeId+" "+ handlerException.StackTrace);
            _queue.SetMessageInError(handlerException.EnvelopeId,handlerException.Message);
        }

        public void Dispose()
        {
          
        }
    }
}
