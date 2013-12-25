using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Yasb.Common.Messaging.EndPoints;
using Yasb.Common.Serialization;

namespace Yasb.Common.Messaging.Configuration
{
    public class ServiceBusConfiguration<TEndPoint,  TSerializer>
    {
        public ServiceBusConfiguration()
        {
            EndPoints = new EndPointsRepository<TEndPoint>();
            
        }


        public TSerializer Serializer { get; internal set; }

        public EndPointsRepository<TEndPoint> EndPoints { get; private set; }
    }
}
