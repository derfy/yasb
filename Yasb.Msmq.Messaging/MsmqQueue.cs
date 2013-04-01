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
        private MsmqEndPoint _endPoint;
        private IMessageFormatter _formatter;
        public MsmqQueue(MsmqEndPoint endPoint,IMessageFormatter formatter)
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
            throw new NotImplementedException();
        }


        public bool TryGetEnvelope(TimeSpan delta, out MessageEnvelope envelope)
        {
            throw new NotImplementedException();
        }


        public void Push(MessageEnvelope envelope)
        {
            var message = new Message(envelope) { Formatter = _formatter };
            using (var tx = new TransactionScope())
            {
                _internalQueue.Send(message, MessageQueueTransactionType.Automatic);
                tx.Complete();
            }
        }
        public MessageEnvelope WrapInEnvelope(IMessage message, IEndPoint fromEndPoint)
        {
            return new MessageEnvelope(message, Guid.NewGuid().ToString(), fromEndPoint, LocalEndPoint);
        }




        public IEndPoint LocalEndPoint
        {
            get { return _endPoint; }
        }
    }
    
}
