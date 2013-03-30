using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Yasb.Common.Messaging
{

    public struct AddressInfo 
    {
        private string _host;
        private int _port;
        public AddressInfo(string host,int port)
        {
            _host = host;
            _port = port;
        }

        public string Host { get { return _host; } }
        public int Port { get { return _port; } }
        public override string ToString()
        {
            return string.Format("{0}:{1}", Host, Port);
        }

        public EndPoint ToEndPoint() 
        {
            var ipAddress = IPAddress.Parse(Host);
            return new IPEndPoint(ipAddress, Port);

        }

        public static AddressInfo CreateForm(EndPoint endPoint)
        {
            var ipEndPoint = (IPEndPoint)endPoint;
            return new AddressInfo(ipEndPoint.Address.ToString(), ipEndPoint.Port);
        }
    }
    public  class BusEndPoint : DnsEndPoint
    {
      
       
        private BusEndPoint(string host, int port, string queueName="local"):this(host,port)
        {
           
            QueueName = queueName;
        }


        public BusEndPoint(string host, int port)
            : base(host, port)
        {
        }
        

        public static BusEndPoint Parse(string endPoint)
        {
            var array = endPoint.Split(':');
            if (array.Length < 3)
                throw new ApplicationException("endpoint is not valid");
            return new BusEndPoint(array[0], int.Parse(array[1]), array[2]);
        }

        

        //public AddressInfo AddressInfo { 
        //    get {
               
        //        return new AddressInfo(Host, Port);
        //    } 
        //}

        public string Name { get;  set; }
        
        public string QueueName { get;  set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}",Host,Port,QueueName);
        }
        public override bool Equals(object obj)
        {
            var endPoint = obj as BusEndPoint;
            if (endPoint == null) return false;
            var isEqual= Host==endPoint.Host && Port==endPoint.Port && QueueName==endPoint.QueueName;
            return isEqual;
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode() ^ Port.GetHashCode() ^ QueueName.GetHashCode();
        }

    }
}
