using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using Yasb.Common.Messaging.Configuration;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueueFactory :  IQueueFactory<MongoDbEndPointConfiguration>
    {

        public IQueue<MongoDbEndPointConfiguration> CreateQueue(MongoDbEndPointConfiguration endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
