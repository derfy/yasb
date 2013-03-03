using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections;

namespace Yasb.Common.Messaging
{
    public class EndPointsRegistrar : IEndPointsRegistrar
    {
        private Dictionary<string, BusEndPoint> _namedEndPointsMap = new Dictionary<string, BusEndPoint>();
        private List<BusEndPoint> _registeredEndPoints = new List<BusEndPoint>();
        
        public BusEndPoint GetEndPointByName(string name)
        {
            if (!_namedEndPointsMap.ContainsKey(name))
                throw new ApplicationException(string.Format("No endpoint has been registered with name {0}",name));
            return _namedEndPointsMap[name];
        }
        public void AssignNameToEndPoint(BusEndPoint endPoint, string endPointName)
        {
            if (!_registeredEndPoints.Contains(endPoint))
            {
                throw new ApplicationException("Endpoint has not been registered");
            }
            _namedEndPointsMap[endPointName] = endPoint;
        }

        public bool IsEndPointRegisterd(BusEndPoint endPoint)
        {
            return _registeredEndPoints.Contains(endPoint);
        }

        public IEnumerable<BusEndPoint> GetRegisteredEndPoints()
        {
            return _registeredEndPoints.AsReadOnly();
        }

        internal void Add(BusEndPoint targetEndPoint)
        {
            _registeredEndPoints.Add(targetEndPoint);
        }
    }
}
