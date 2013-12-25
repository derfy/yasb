using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Tests.Configuration
{
    public class TestSerializationConfiguration { }
    public class TestEndPointConfiguration : IEndPointConfiguration<TestEndPoint>
    {
        private string _queueName;
        private string _hostName;
        public TestEndPointConfiguration WithHostName(string hostName)
        {
            _hostName = hostName;
            return this;
        }



        public TestEndPointConfiguration WithQueueName(string queueName)
        {
            _queueName = queueName;
            return this;
        }

        public TestEndPoint Built
        {
            get { return new TestEndPoint(_hostName, _queueName); }
        }
    }
    public class TestEndPoint : QueueEndPoint,IEndPoint {
        public TestEndPoint(string host,string queueName):base(host,queueName)
        {

        }
        public  string Value
        {
            get { return string.Format("{0}:{1}:{2}", Host,Port, QueueName); }
        }

        public int Port { get; set; }
    }
    
}
