using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Yasb.Common.Messaging;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.MongoDb.Messaging
{
    public static class MongoDbFactory
    {


        public static MongoDbSubscriptionService CreateSubscriptionService(MongoDbEndPoint connection)
        {
             return new MongoDbSubscriptionService(MongoDbFactory.CreateDatabase(connection));
        }


        internal static MongoDatabase CreateDatabase(MongoDbEndPoint connection)
        {
            var connectionString = string.Format("mongodb://{0}", connection.Host);
            var server = MongoServer.Create(connectionString);
            return server.GetDatabase(connection.Database);

        }
    }
}
