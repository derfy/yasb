using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueueFactory : AbstractQueueFactory<MongoDbConnection>
    {
        public MongoDbQueueFactory(QueueConfiguration<MongoDbConnection> queueConfiguration)
            : base(queueConfiguration)
        {
        }
        //public IQueue<MongoDbConnection> CreateFromEndPointValue(string endPointValue)
        //{
        //    var array = endPointValue.Split(':');
        //    if (array.Length != 3)
        //        throw new ApplicationException("endPoint is not valid");
        //    var connection = new MongoDbConnection(array[0], array[1]);
        //    return CreateQueue(connection, array[2]);
        //}

        public override IQueue<MongoDbConnection> CreateQueue(MongoDbConnection connection, string queueName)
        {
            var endPoint = new MongoDbQueueEndPoint(connection, queueName);
            return new MongoDbQueue(endPoint, queueName);
        }
    }
}
