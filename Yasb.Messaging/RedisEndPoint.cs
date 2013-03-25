using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using Yasb.Common.Messaging;

namespace Yasb.Redis.Messaging
{
    public  class RedisEndPoint : IEndPoint
    {

        private IPAddress _ipAddress;
        private int _port;
        public RedisEndPoint(string endPoint)
            : this(Parse(endPoint))
        {
        }

        private RedisEndPoint(RedisEndPoint endPoint)
            : this(endPoint._ipAddress, endPoint._port, endPoint.QueueName)
        { 
        }

        private RedisEndPoint(IPAddress ipAddress, int port, string queueName)
        {
            _ipAddress = ipAddress;
            _port = port;
            QueueName = queueName;
        }
       
        public static RedisEndPoint Parse(string endPoint)
        {
            var array = endPoint.Split(':');
            if (array.Length < 3)
                throw new ApplicationException("endpoint is not valid");
            var host = Dns.GetHostAddresses(array[0]).Where(addr=>addr.AddressFamily==AddressFamily.InterNetwork).FirstOrDefault();
            if (host == null)
                throw new ApplicationException("Provided host is not valid");
            var port = 0;
            if(!int.TryParse(array[1],out port))
                 throw new ApplicationException("port must be a valid number");
            if(string.IsNullOrWhiteSpace(array[2]))
                throw new ApplicationException("queueName cannot be empty");
            return new RedisEndPoint(host,port, array[2]);
        }

        public string Value { get { return string.Format("{0}:{1}:{2}", _ipAddress, _port, QueueName); } }

        public string Name { get; set; }
        
        public string QueueName { get; private  set; }

        public EndPoint ToIPEndPoint() { return new IPEndPoint(_ipAddress, _port); }
        

        public override bool Equals(object obj)
        {
            var endPoint = obj as RedisEndPoint;
            if (endPoint == null) return false;
            return _ipAddress.Equals(endPoint._ipAddress) && _port == endPoint._port && QueueName == endPoint.QueueName;
        }

        public override int GetHashCode()
        {
            return _ipAddress.GetHashCode() ^ _port.GetHashCode() ^ QueueName.GetHashCode();
        }

    }
}
