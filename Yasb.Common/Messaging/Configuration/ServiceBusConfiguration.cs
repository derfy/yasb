using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Yasb.Common.Messaging.Configuration
{
    public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        private List<BusEndPoint> _namedEndPointsList = new List<BusEndPoint>();
        private BusEndPoint _currentEndPoint;
        public ServiceBusConfiguration()
        {
        }
        public IServiceBusConfiguration WithLocalEndPoint(Action<EndPointConfiguration> configurer)
        {
            return WithEndPoint("local",configurer);
        }

        public IServiceBusConfiguration WithEndPoint(string endPointName, Action<EndPointConfiguration> configurer)
        {
            var configuration = new EndPointConfiguration(endPointName);
            configurer(configuration);
            _namedEndPointsList.Add(configuration.Built);
            return this;
        }
        public IServiceBusConfiguration WithMessageHandlersAssembly(Assembly assembly)
        {
            MessageHandlersAssembly = assembly;
            return this;
        }

        public BusEndPoint[] EndPoints { get { return _namedEndPointsList.ToArray(); } }

        public AddressInfo[] AddressInfoList { get { return EndPoints.Select(e => e.AddressInfo).Distinct().ToArray(); } }

        public BusEndPoint LocalEndPoint { get { return _namedEndPointsList.First(e=>e.Name=="local"); } }

        public Assembly MessageHandlersAssembly { get; private set; }

    }
}
