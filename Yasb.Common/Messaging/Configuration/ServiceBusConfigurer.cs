using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Yasb.Common.Messaging.EndPoints;

namespace Yasb.Common.Messaging.Configuration
{

    public class ServiceBusConfigurer<TEndPoint, TSerializer> 
    {
        public ServiceBusConfigurer()
        {
            Built = new ServiceBusConfiguration<TEndPoint,TSerializer>();
        }
        public ServiceBusConfigurer<TEndPoint, TSerializer> EndPoints<TEndPointConfiguration>(Action<EndPointsConfigurer<TEndPoint, TEndPointConfiguration>> endPointConfigurationBuilder)
            where TEndPointConfiguration : IEndPointConfiguration<TEndPoint>
        {
            var endPointsConfigurer = new EndPointsConfigurer<TEndPoint, TEndPointConfiguration>(Built.EndPoints);
            endPointConfigurationBuilder(endPointsConfigurer);
            return this;
        }
        public ServiceBusConfigurer<TEndPoint, TSerializer> Serializer<TSerializerConfiguration>(Action<TSerializerConfiguration> serializerConfigurer=null)
            where TSerializerConfiguration : ISerializerConfiguration<TSerializer>
        {

            var serializationConfiguration = Activator.CreateInstance<TSerializerConfiguration>();
            Built.Serializer = serializationConfiguration.Built;
            return this;
        }
        public ServiceBusConfiguration<TEndPoint, TSerializer> Built { get; private set; }
       
       
    }
}
