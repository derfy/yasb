using System;
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
        private MessageQueue _internalQueue;
        private BusEndPoint _endPoint;
        private IMessageFormatter _formatter;
        public MsmqQueue(BusEndPoint endPoint,IMessageFormatter formatter)
        {
            _endPoint = endPoint;
            _formatter = formatter;
        }
        public void Initialize()
        {
            if (!MessageQueue.Exists(_endPoint.Value))
              MessageQueue.Create(_endPoint.Value, true);
            _internalQueue = new MessageQueue(_endPoint.Value) { Formatter=_formatter};
            _internalQueue.MessageReadPropertyFilter.SenderId = true;
        }



        public void SetMessageCompleted(string envelopeId)
        {
            _internalQueue.ReceiveByCorrelationId(envelopeId);
        }


        public bool TryGetEnvelope(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            using (MessageEnumerator enumerator = _internalQueue.GetMessageEnumerator2())
            {
                while (enumerator.MoveNext())
                {
                    var message = enumerator.Current;
                    envelope = message.Body as MessageEnvelope;
                    envelope.RetriesNumber++;
                    if (envelope.StartTimestamp == null || now.Subtract(timoutWindow).Ticks > envelope.StartTimestamp)
                    {
                        envelope.Id = message.Id;
                        envelope.StartTimestamp = now.Ticks;
                        using (var tx = new TransactionScope())
                        {
                            message = _internalQueue.ReceiveById(envelope.Id);
                            message.CorrelationId = envelope.Id;
                            message.Body = envelope;
                            _internalQueue.Send(message, MessageQueueTransactionType.Automatic);
                            tx.Complete();
                            return true;
                        }
                        
                    }
                    
                }
                return false;
                
            }
           
        }


        public void Push(MessageEnvelope envelope)
        {
            envelope.Id = Guid.NewGuid().ToString();
            var message = new Message(envelope) { Formatter = _formatter };
            using (var tx = new TransactionScope())
            {
                _internalQueue.Send(message, MessageQueueTransactionType.Automatic);
                tx.Complete();
            }
        }
        public MessageEnvelope WrapInEnvelope(IMessage message, BusEndPoint fromEndPoint)
        {
            return new MessageEnvelope(message,  fromEndPoint, LocalEndPoint);
        }




        public BusEndPoint LocalEndPoint
        {
            get { return _endPoint; }
        }
    }
    
}
