using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class EndPointConfiguration
    {
        private List<QueueEndPointInfo> _endPointsInfoList = new List<QueueEndPointInfo>();

        public QueueEndPointInfo GetEndPointInfoByConnectionName(string connectionName)
        {
            return _endPointsInfoList.SingleOrDefault(e => e.ConnectionName == connectionName);
        }
        public QueueEndPointInfo GetEndPointInfoByName(string name)
        {
            return _endPointsInfoList.SingleOrDefault(e => e.Name == name);
        }
     
        internal void AddNamedEndPoint(string connectionName,string queueName,string endPointName)

        {
            var queueEndPointInfo=new  QueueEndPointInfo(connectionName,queueName, endPointName);
            _endPointsInfoList.Add(queueEndPointInfo);
        }
        
    }

   
}
