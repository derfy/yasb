using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueueEndPoint : QueueEndPoint<MongoDbConnection>
    {
        private string _database;
        public MongoDbQueueEndPoint(MongoDbConnection connection, string name)
            : base(connection, name)
        {
        }

        public override string Value { get { return string.Format("{0}:{1}:{2}", Connection.Host, Connection.Database, Name); } }

    }
}
