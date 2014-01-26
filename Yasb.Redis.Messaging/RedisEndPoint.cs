using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Yasb.Common.Messaging.EndPoints;
using Yasb.Redis.Messaging.Configuration;

namespace Yasb.Redis.Messaging
{
    public class RedisEndPoint : QueueEndPoint
    {
        public RedisEndPoint()
        {

        }
        public RedisEndPoint(string host,string queueName,int port=6379) : base(host,queueName)
        {
            Port = port;
        }
        public virtual int Port { get;  set; }

       
        public virtual EndPoint Address {
            get
            {
                var ipAddress = Dns.GetHostAddresses(Host).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
                return new IPEndPoint(ipAddress,Port);
            }  
        }
        public string Value { get { return string.Format("{0}:{1}:{2}", Host, Port, QueueName); } }

        
    }
}
