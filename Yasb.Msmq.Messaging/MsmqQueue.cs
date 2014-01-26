using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Yasb.Common.Messaging;
using System.Transactions;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Yasb.Common.Tests.Messages;
using System.Threading;
using Yasb.Common.Messaging.Serialization;
using Yasb.Common.Messaging.Serialization.Xml;
using Yasb.Msmq.Messaging.Configuration;

namespace Yasb.Msmq.Messaging
{
    internal enum MessageIdType { Id,Correlation }
    public class MsmqQueue : IQueue<MsmqEndPointConfiguration>
    {
        private IMessageFormatter _formatter;
        public MsmqQueue(MsmqEndPointConfiguration localEndPointConfiguration, AbstractXmlSerializer<MessageEnvelope> envelopeSerializer)
        {
            LocalEndPointConfiguration = localEndPointConfiguration;
            _formatter = new CustomMessageFormatter(envelopeSerializer);
            Initialize();
        }
        public void Initialize()
        {
            if (!MessageQueue.Exists(LocalEndPointConfiguration.Path))
                MessageQueue.Create(LocalEndPointConfiguration.Path, true);
        }


        public MsmqEndPointConfiguration LocalEndPointConfiguration { get; private set; }

        public void SetMessageCompleted(string envelopeId, DateTime now)
        {
            using (var internalQueue = new MessageQueue(LocalEndPointConfiguration.Path) )
            {
                internalQueue.ReceiveByCorrelationId(envelopeId);
                Console.WriteLine("Message {0} was Completed at : {1}", envelopeId, now);
            }
            
        }

        public void SetMessageInError(string envelopeId,string errorMessage)
        {
            TryRequeue(envelopeId,MessageIdType.Correlation, msg =>
            {
                var env = msg.Body as MessageEnvelope;
                env.StartTimestamp = null;
                env.LastErrorMessage = errorMessage;
            });
            
        }
        
        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            Message message = null;
            if (!TryPeekMessage(out message))
            { 
                return false; 
            }
            MessageEnvelope env = message.Body as MessageEnvelope;
            int retriesNumber = env.RetriesNumber;
            string lastErrorMessage = env.LastErrorMessage;
            if (env.StartTimestamp.HasValue)
            {
                if (now.Subtract(env.StartTime.Value) < timoutWindow)
                {
                    TryRequeue(message.Id, MessageIdType.Id);
                    return false;
                }
                lastErrorMessage = "Operation Timed Out";
            }

            retriesNumber++;
            if (retriesNumber >= 5)
            {
                //TODO: Should deadeletter this message
                return false;
            }
            var result = TryRequeue(message.Id, MessageIdType.Id, msg =>
            {
                env = msg.Body as MessageEnvelope;
                env.Id = msg.Id;
                msg.CorrelationId = msg.Id;
                env.StartTimestamp = now.Ticks;
                env.RetriesNumber = retriesNumber;
                env.LastErrorMessage = lastErrorMessage;
            });
            if (result)
            {
                envelope = env;
                return true;
            }
            return false;
        }

      

        public void Push(IMessage message)
        {
            using (var internalQueue = new MessageQueue(LocalEndPointConfiguration.Path) { Formatter = _formatter })
            {
                var envelope = new MessageEnvelope(message);
                var msmqMessage = new Message(envelope);
                msmqMessage.Formatter = _formatter;
                internalQueue.Send(msmqMessage, MessageQueueTransactionType.Single);
            }
            
            
        }
        public void Clear()
        {
            using (var internalQueue = new MessageQueue(LocalEndPointConfiguration.Path) { Formatter = _formatter })
            {
                internalQueue.Purge();
            }
        }
      
        private bool TryRequeue(string messageId,MessageIdType messageIdType, Action<Message> action=null)
        {

            using (var tx = new MessageQueueTransaction())
            {
                try
                {
                    using (var internalQueue = new MessageQueue(LocalEndPointConfiguration.Path) { Formatter = _formatter })
                    {
                        tx.Begin();
                        var msg = messageIdType == MessageIdType.Correlation ? internalQueue.ReceiveByCorrelationId(messageId, tx) : internalQueue.ReceiveById(messageId, tx);
                        if (action != null)
                            action(msg);
                        internalQueue.Send(msg, tx);
                        tx.Commit();
                        return true;
                    }
                   
                }
                catch (InvalidOperationException ex)
                {
                    //Console.WriteLine("Message {0} was not found : {1}",messageId,ex.Message);
                    if (tx.Status == MessageQueueTransactionStatus.Pending)
                    {
                        tx.Abort();
                    }
                    return false;
                }
            }
        }
        private bool TryPeekMessage(out Message message)
        {
            message = null;
            using (var internalQueue = new MessageQueue(LocalEndPointConfiguration.Path) )
            {

                try
                {
                    message= internalQueue.Peek(TimeSpan.FromMilliseconds(10));
                    message.Formatter = _formatter;
                    return true;

                }
                catch (MessageQueueException mqe)
                {
                    if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        return false;
                    throw;
                }
            }
        }

       



        
      
    }
    
}
