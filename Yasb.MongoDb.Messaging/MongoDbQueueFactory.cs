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
        public override IQueue CreateFromEndPointValue(string endPointValue)
        {
            var array = endPointValue.Split(':');
            if (array.Length != 3)
                throw new ApplicationException("endPoint is not valid");
            var connection = new MongoDbConnection(array[0], array[1]);
            return CreateQueue(connection, array[2]);
        }

        protected override IQueue CreateQueue(MongoDbConnection connection, string queueName)
        {
            return new MongoDbQueue(connection, queueName);
        }
    }
}
