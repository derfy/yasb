using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public  class ServiceBusConfiguration
    {
        private List<IEndPoint> _namedEndPointsList = new List<IEndPoint>();
      
        public ServiceBusConfiguration()
        {
        }


        public IEndPoint LocalEndPoint { get; private set; }

        public Assembly MessageHandlersAssembly { get; private set; }

        public IEndPoint[] NamedEndPoints { get { return _namedEndPointsList.ToArray(); } }

        public ServiceBusConfiguration WithLocalEndPoint<TEndPointConfiguration>(string endPoint)
            where TEndPointConfiguration : EndPointConfiguration<TEndPointConfiguration>
        {
            var configuration = CreateEndPointConfiguration<TEndPointConfiguration>(endPoint);
            configuration.Built.Name = "local";
            LocalEndPoint = configuration.Built;
            _namedEndPointsList.Add(LocalEndPoint);
            return this;
        }

        public ServiceBusConfiguration WithEndPoint<TEndPointConfiguration>(string endPoint, Action<TEndPointConfiguration> configurer)
            where TEndPointConfiguration : EndPointConfiguration<TEndPointConfiguration>
        {
            var configuration = CreateEndPointConfiguration<TEndPointConfiguration>(endPoint);
            configurer(configuration);
            _namedEndPointsList.Add(configuration.Built);
            return this;
        }

       
        public ServiceBusConfiguration WithMessageHandlersAssembly(Assembly assembly)
        {
            MessageHandlersAssembly = assembly;
            return this;
        }
        protected TEndPointConfiguration CreateEndPointConfiguration<TEndPointConfiguration>(string endPoint)
            where TEndPointConfiguration : EndPointConfiguration<TEndPointConfiguration>
        {
            var endPointConfiguration = Activator.CreateInstance<TEndPointConfiguration>();
            endPointConfiguration.Built = endPointConfiguration.CreateEndPoint(endPoint);
            return endPointConfiguration;
        }

    }
}
