using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class EndPointConfiguration
    {
        private List<BusEndPoint> _endPoints = new List<BusEndPoint>();
        public EndPointConfiguration()
        {

        }
        public BusEndPoint LocalEndPoint { get; internal set; }

        public BusEndPoint[] NamedEndPoints { get { return _endPoints.ToArray(); } }

        internal void AddNamedEndPoint(BusEndPoint endPoint)
        {
            _endPoints.Add(endPoint);
        }
    }
}
