using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class EndPointConfiguration
    {
        private List<EndPointInfo> _endPointsInfoList = new List<EndPointInfo>();

        
        public EndPointInfo GetEndPointInfoByName(string name)
        {
            return _endPointsInfoList.SingleOrDefault(e => e.Name == name);
        }
     
        internal void AddNamedEndPoint(string connectionName,string queueName,string endPointName)

        {
            var queueEndPointInfo=new  EndPointInfo(connectionName,queueName, endPointName);
            _endPointsInfoList.Add(queueEndPointInfo);
        }
        
    }

   
}
