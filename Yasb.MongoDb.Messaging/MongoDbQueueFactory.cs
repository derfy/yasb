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
            throw new NotImplementedException();
        }

        protected override IQueue CreateQueue(MongoDbConnection connection, string queueName)
        {
            var connectionString = string.Format("mongodb://{0}",connection.Host);
            var server = MongoServer.Create(connectionString);
            var database = server.GetDatabase(connection.Database);
            //if (!database.CollectionExists(queueName))
            //{
            //    database.CreateCollection(queueName);
            //}
            //var collection=database.GetCollection(queueName);

            return new MongoDbQueue(database,queueName);
        }
    }
}
