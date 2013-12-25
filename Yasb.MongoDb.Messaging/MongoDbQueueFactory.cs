using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints.MongoDb;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueueFactory :  IQueueFactory<MongoDbEndPoint>
    {

        public  IQueue<MongoDbEndPoint> CreateQueue(MongoDbEndPoint endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
