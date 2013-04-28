﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Yasb.Common.Messaging;
using System.Transactions;

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
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                var message = internalQueue.Peek();
                var newEnvelope = message.Body as MessageEnvelope;
                newEnvelope.RetriesNumber++;
                if (newEnvelope.StartTime.HasValue && now.Subtract(newEnvelope.StartTime.Value) > timoutWindow)
                {
                    newEnvelope.LastErrorMessage = "Operation Timed Out";
                }
                else
                {
                    newEnvelope.Id = message.Id;
                    newEnvelope.StartTimestamp = now.Ticks;
                    envelope = WithMessageQueueTransaction(tx =>
                    {
                        var msg = internalQueue.ReceiveById(newEnvelope.Id,tx);
                        if(newEnvelope.RetriesNumber>5)
                        {
                            return null;
                        }
                        msg.CorrelationId = newEnvelope.Id;
                        msg.Body = newEnvelope;
                        internalQueue.Send(msg, tx);
                        return newEnvelope;
                    });
                }    
                
            }
            return envelope!=null;
        }

       
        public void Push(MessageEnvelope envelope)
        {
            envelope.Id = Guid.NewGuid().ToString();
            var message = new Message(envelope) { Formatter = _formatter };
            using (var internalQueue = new MessageQueue(LocalEndPoint) { Formatter = _formatter })
            {
                internalQueue.Send(message, MessageQueueTransactionType.Single);
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
