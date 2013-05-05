using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Yasb.Common.Messaging;
using System.Transactions;
using System.Threading.Tasks;

namespace Yasb.Msmq.Messaging
{
    public class MsmqQueue : IQueue
    {
        private IMessageFormatter _formatter;
        public MsmqQueue(string localEndPoint,IMessageFormatter formatter)
        {
            LocalEndPoint = localEndPoint;
            _formatter = formatter;
            Initialize();
        }
        public void Initialize()
        {
            if (!MessageQueue.Exists(LocalEndPoint))
                MessageQueue.Create(LocalEndPoint, true);
        }



        public void SetMessageCompleted(string envelopeId)
        {
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                internalQueue.ReceiveByCorrelationId(envelopeId,MessageQueueTransactionType.Single);
            }
        }


        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var tcs = new TaskCompletionSource<MessageEnvelope>();
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                Task<Message> ts = Task.Factory.FromAsync<Message>(internalQueue.BeginPeek(TimeSpan.FromMilliseconds(10)), internalQueue.EndPeek);
                ts.ContinueWith(antecedent =>
                {
                    antecedent.Exception.Handle(e => {
                        var mqe=e as MessageQueueException;
                        return mqe!=null && mqe.MessageQueueErrorCode==MessageQueueErrorCode.IOTimeout; 
                    });
                    tcs.SetResult(null);
                }, TaskContinuationOptions.OnlyOnFaulted);
                ts.ContinueWith(t => {
                    var message = t.Result;
                    var newEnvelope = message.Body as MessageEnvelope;
                    if (newEnvelope.StartTime.HasValue)
                    {
                        if (now.Subtract(newEnvelope.StartTime.Value) < timoutWindow)
                        {
                            tcs.SetResult(null);
                            return;
                        }
                        newEnvelope.LastErrorMessage = "Operation Timed Out";
                    }
                    else {
                        newEnvelope.Id = message.Id;
                    }
                    tcs.SetResult(WithMessageQueueTransaction(tx =>
                    {
                        var msg = internalQueue.ReceiveById(message.Id, tx);
                        if (newEnvelope.RetriesNumber >= 5) return null;
                        newEnvelope.StartTimestamp = now.Ticks;
                        newEnvelope.RetriesNumber++;
                        msg.CorrelationId = newEnvelope.Id;
                        msg.Body = newEnvelope;
                        internalQueue.Send(msg, tx);
                        return newEnvelope;
                    }));
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                tcs.Task.Wait();
                envelope = tcs.Task.Result;
            }
            return envelope!=null;
        }

       
        public void Push(MessageEnvelope envelope)
        {
            envelope.Id = Guid.Empty.ToString();
            var message = new Message(envelope) { Formatter = _formatter };
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                internalQueue.Send(message, MessageQueueTransactionType.Single);
            }
        }
        public void Clear()
        {
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                internalQueue.Purge();
            }
        }
        private MessageEnvelope WithMessageQueueTransaction(Func<MessageQueueTransaction, MessageEnvelope> func)
        {
            using (var tx = new MessageQueueTransaction())
            {
                try
                {
                    tx.Begin();
                    var result = func(tx);
                    tx.Commit();
                    return result;
                }
                catch (InvalidOperationException)
                {
                    if (tx.Status == MessageQueueTransactionStatus.Pending)
                    {
                        tx.Abort();
                    }
                }
                return null;
            }
        }


        public string LocalEndPoint { get; private set; }
    }
    
}
