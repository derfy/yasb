using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Messaging
{
    public class BusEndPoint<TConnection> : BusEndPoint
    {
       
        public BusEndPoint(string connectionName, string queueName, string endPointName):base(connectionName,queueName,endPointName)
        {
          
        }

        public BusEndPoint(TConnection address, string queueName, string endPointName=null)
        {

        }
        public TConnection Address { get; set; }

        public override string Value
        {
            get { throw new NotImplementedException(); }
        }
    }
    public abstract class BusEndPoint 
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
       
        public abstract string Value { get ; }
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
