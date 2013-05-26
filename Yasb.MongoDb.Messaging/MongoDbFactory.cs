using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Yasb.Common.Messaging.Configuration.MongoDb;
using Yasb.Common.Messaging;

namespace Yasb.MongoDb.Messaging
{
    public static class MongoDbFactory
    {
        

        public static MongoDbSubscriptionService CreateSubscriptionService(MongoDbConnection connection)
        {
             return new MongoDbSubscriptionService(MongoDbFactory.CreateDatabase(connection));
        }


        internal static MongoDatabase CreateDatabase(MongoDbConnection connection)
        {
            var connectionString = string.Format("mongodb://{0}", connection.Host);
            var server = MongoServer.Create(connectionString);
            return server.GetDatabase(connection.Database);

        }
    }
}
