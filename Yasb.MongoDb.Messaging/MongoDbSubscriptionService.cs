using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.MongoDb.Messaging
{

  

    public class MongoDbSubscription {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        //[BsonSerializer(typeof(MyClassSerializer))]
        public string[] Endpoints { get; set; }
    }
    public class MongoDbSubscriptionService : ISubscriptionService<MongoDbConnection>
    {
        private MongoCollection _collection;
        public MongoDbSubscriptionService(MongoDatabase database)
        {
            InitializeCollection(database);
        }
       

        public void Handle(SubscriptionMessage<MongoDbConnection> message)
        {

          //  _collection.Update(Query.EQ("TypeName", message.TypeName), Update.AddToSet("Endpoints", message.SubscriberEndPoint), UpdateFlags.Upsert);
        }
        public SubscriptionInfo<MongoDbConnection>[] GetSubscriptions(string typeName)
        {
            return null;
            //  return _collection.FindOneAs<MongoDbSubscription>(Query.EQ("TypeName", typeName)).Endpoints;
        }
        public void UnSubscribe(string typeName, string subscriberEndPoint)
        {
            throw new NotImplementedException();
        }
        private void InitializeCollection(MongoDatabase database)
        {
            if (!database.CollectionExists("subscriptions"))
            {
                database.CreateCollection("subscriptions");
            }
            _collection = database.GetCollection("subscriptions");
        }

    
    }
}
