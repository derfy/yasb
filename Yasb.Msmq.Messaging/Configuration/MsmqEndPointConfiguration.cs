using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Msmq.Messaging.Configuration
{
    public class MsmqEndPointConfiguration
    {
        private string _hostName="localhost";
        private bool _isPrivate = true;
        public MsmqEndPointConfiguration()
        {

        }
        public MsmqEndPointConfiguration(string hostName, string queueName)
        {
            _hostName = hostName;
            QueueName = queueName;
        }
        public MsmqEndPointConfiguration WithHostName(string hostName)
        {
            _hostName = hostName;
            return this;
        }



        public MsmqEndPointConfiguration WithQueueName(string queueName)
        {
            QueueName = queueName;
            return this;
        }
        internal string Host { get { return _hostName == "localhost" ? "." : _hostName; } }

        internal string QueueName { get; private set; }
        

        internal string Path { get { return string.Format(@"{0}\{1}$\{2}", Host, _isPrivate ? "private" : "public", QueueName); } }



        public string Value { get { return string.Format(@"{0}:{1}:{2}", Host, _isPrivate ? "private" : "public", QueueName); } }

        
    }
}
