using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
   
    public  class BusEndPoint 
    {
        public BusEndPoint()
        {

        }
        public BusEndPoint(string connectionName, string queueName, string endPointName = null)
        {
            ConnectionName = connectionName;
            QueueName = queueName;
            Name = endPointName;
        }

        public BusEndPoint(string endPointValue)
        {
            var endPoint = BusEndPoint.Parse(endPointValue);
            ConnectionName = endPoint.ConnectionName;
            QueueName = endPoint.QueueName;
        }

        private static BusEndPoint Parse(string endPointValue)
        {
            var endPointValues = endPointValue.Split(':');
            if (endPointValues.Length != 2)
                throw new ApplicationException("EndPoint is not valid");
            return new BusEndPoint(endPointValues[0], endPointValues[1]);
        }
        public string Value { get { return string.Format("{0}:{1}", ConnectionName, QueueName); } }
        public string Name { get; private set; }
        public string ConnectionName { get; private set; }
        public string QueueName { get; private set; }

        public override bool Equals(object obj)
        {
            var endPoint = obj as BusEndPoint;
            if (endPoint == null || endPoint.GetType() != typeof(BusEndPoint))
                return false;
            return endPoint.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
