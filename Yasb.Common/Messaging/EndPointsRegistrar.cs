using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections;

namespace Yasb.Common.Messaging
{
    public class EndPointsRegistrar 
    {
        private Dictionary<string, BusEndPoint> _namedEndPointsMap = new Dictionary<string, BusEndPoint>();
        
        public BusEndPoint GetEndPointByName(string name)
        {
            if (!_namedEndPointsMap.ContainsKey(name))
                throw new ApplicationException(string.Format("No endpoint has been registered with name {0}",name));
            return _namedEndPointsMap[name];
        }
        public void AssignNameToEndPoint(string endPointName,BusEndPoint endPoint)
        {
            _namedEndPointsMap[endPointName] = endPoint;
        }


        public IEnumerable<BusEndPoint> EndPoints { get { return _namedEndPointsMap.Values; } }
    }
}
