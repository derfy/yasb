using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Yasb.Common.Messaging;
using System.Transactions;
using System.Threading.Tasks;
using Yasb.Common.Messaging.EndPoints.Msmq;

namespace Yasb.Msmq.Messaging
{
    public class MsmqQueue : IQueue<MsmqEndPoint>
    {
        private IMessageFormatter _formatter;
        public MsmqQueue(MsmqEndPoint localEndPoint, IMessageFormatter formatter)
        {
            LocalEndPoint = localEndPoint;
            _formatter = formatter;
            Initialize();
        }
        public void Initialize()
        {
            if (!MessageQueue.Exists(LocalEndPoint.Value))
                MessageQueue.Create(LocalEndPoint.Value, true);
        }



        public void SetMessageCompleted(string envelopeId, DateTime now)
        {
            using (var internalQueue = new MessageQueue(LocalEndPoint.Value) { Formatter = _formatter })
            {
                WithMessageQueueTransaction(tx =>
                {
                    //Console.WriteLine("Completing " + envelopeId);
                    var msg=internalQueue.ReceiveByCorrelationId(envelopeId, tx);
                    var env = msg.Body as MessageEnvelope;
                    //Console.WriteLine(string.Format("Completed {0} after number of retries {1} ", envelopeId, env.RetriesNumber));
                    return null;
                });
            }
        }

        public void SetMessageInError(string envelopeId,string errorMessage)
        {
            using (var internalQueue = new MessageQueue(LocalEndPoint.Value) { Formatter = _formatter })
            {
                WithMessageQueueTransaction(tx =>
                {
                    var msg = internalQueue.ReceiveByCorrelationId(envelopeId, tx);
                    var env = msg.Body as MessageEnvelope;
                    env.StartTimestamp = null;
                    env.LastErrorMessage = errorMessage;
                    internalQueue.Send(msg, tx);
                    return null;
                });
            }
        }
        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            using (var internalQueue = new MessageQueue(LocalEndPoint.Value) { Formatter = _formatter })
            {
                Task<Message> ts = Task.Factory.FromAsync<Message>(internalQueue.BeginPeek(TimeSpan.FromMilliseconds(10)), internalQueue.EndPeek);
                var onCompletionTask = ts.ContinueWith<MessageEnvelope>(t =>
                {
                    if (t.Exception != null)
                    {
                        t.Exception.Handle(e =>
                        {
                            var mqe = e as MessageQueueException;
                            return mqe != null && mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout;
                        });
                        return null;
                    }
                    var message = t.Result;
                    var env = message.Body as MessageEnvelope;


                    if (env.StartTime.HasValue)
                    {
                        if (now.Subtract(env.StartTime.Value) < timoutWindow)
                        {
                            return WithMessageQueueTransaction(tx =>
                            {
                                internalQueue.Send(message, tx);
                                internalQueue.ReceiveById(message.Id, tx);
                                return null;
                            });
                        }
                        env.LastErrorMessage = "Operation Timed Out";
                    }

                    env.StartTimestamp = now.Ticks;
                    env.RetriesNumber++;
                    return WithMessageQueueTransaction(tx =>
                    {
                        var msg = internalQueue.ReceiveById(message.Id, tx);
                        if (env.RetriesNumber >= 5) {
                            return null; 
                        }
                        msg.CorrelationId = env.Id;
                        msg.Body = env;
                        internalQueue.Send(msg, tx);
                        return env;
                    });
                });
                onCompletionTask.Wait();
                envelope = onCompletionTask.Result;
               
            }
            return envelope!=null;
        }




        public void Push(MessageEnvelope envelope)
        {
            envelope.Id= string.Format("{0}\\{1}", Guid.NewGuid(), 0);;
            var msmqMessage = new System.Messaging.Message(envelope) { Formatter = _formatter };
            using (var internalQueue = new MessageQueue(LocalEndPoint.Value) { Formatter = _formatter })
            {

                internalQueue.Send(msmqMessage, MessageQueueTransactionType.Single);
            }
        }
        public void Clear()
        {
            using (var internalQueue = new MessageQueue(LocalEndPoint.Value) { Formatter = _formatter })
            {
                internalQueue.Purge();
            }
        }
        private MessageEnvelope WithMessageQueueTransaction(Func<MessageQueueTransaction,MessageEnvelope> func)
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
                catch (InvalidOperationException ex)
                {
                    if (tx.Status == MessageQueueTransactionStatus.Pending)
                    {
                        tx.Abort();
                    }
                }
                return null;
            }
        }


        public MsmqEndPoint LocalEndPoint { get; private set; }


       


      
    }
    
}
