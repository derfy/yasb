using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public  class ServiceBusConfiguration<TEndPoint, TEndPointConfiguration> //: IServiceBusConfiguration<TEndPoint, TEndPointConfiguration>
        where TEndPoint : IEndPoint
        where TEndPointConfiguration : EndPointConfiguration<TEndPoint>
    {
        private List<TEndPoint> _namedEndPointsList = new List<TEndPoint>();
      
        public ServiceBusConfiguration()
        {
        }


        public TEndPoint LocalEndPoint { get; private set; }

        public Assembly MessageHandlersAssembly { get; private set; }

        public TEndPoint[] NamedEndPoints { get { return _namedEndPointsList.ToArray(); } }

        public ServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithLocalEndPoint(string endPoint)
        {
            var configuration = CreateEndPointConfiguration(endPoint);
            configuration.Built.Name = "local";
            LocalEndPoint = configuration.Built;
            _namedEndPointsList.Add(LocalEndPoint);
            return this;
        }

        public ServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithEndPoint(string endPoint, Action<TEndPointConfiguration> configurer)
        {
            var configuration = CreateEndPointConfiguration(endPoint);
            configurer(configuration);
            _namedEndPointsList.Add(configuration.Built);
            return this;
        }

       
        public ServiceBusConfiguration<TEndPoint, TEndPointConfiguration> WithMessageHandlersAssembly(Assembly assembly)
        {
            MessageHandlersAssembly = assembly;
            return this;
        }
        protected  TEndPointConfiguration CreateEndPointConfiguration(string endPoint) {
            var endPointConfiguration = Activator.CreateInstance<TEndPointConfiguration>();
            endPointConfiguration.Built = endPointConfiguration.CreateEndPoint(endPoint);
            return endPointConfiguration;
        }

    }
}
