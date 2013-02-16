using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public  class BusEndPoint
    {
        public BusEndPoint(string host, int port, string queueName)
        {
            Host = host;
            Port = port;
            QueueName = queueName;
        }
        

        public static BusEndPoint Parse(string endPoint)
        {
            var array = endPoint.Split(':');
            if (array.Length < 3)
                throw new ApplicationException("endpoint is not valid");
            return new BusEndPoint(array[0], int.Parse(array[1]), array[2]);
        }

        public string Host { get; private set; }
        public int Port { get; private set; }
        public string QueueName { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}",Host,Port,QueueName);
        }

    }
}
