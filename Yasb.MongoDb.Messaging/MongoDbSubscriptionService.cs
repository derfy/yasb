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
using Yasb.Common.Messaging.EndPoints.MongoDb;

namespace Yasb.MongoDb.Messaging
{

  

    public class MongoDbSubscription {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        //[BsonSerializer(typeof(MyClassSerializer))]
        public string[] Endpoints { get; set; }
    }
    public class MongoDbSubscriptionService : ISubscriptionService<MongoDbEndPoint>
    {
        private MongoCollection _collection;
        public MongoDbSubscriptionService(MongoDatabase database)
        {
            InitializeCollection(database);
        }
       

        public MongoDbEndPoint[] GetSubscriptionEndPoints()
        {
            throw new NotImplementedException();
        }





        public void SubscribeTo(MongoDbEndPoint topicEndPoint)
        {
            throw new NotImplementedException();
        }
        public void UnSubscribe(string topicName, MongoDbEndPoint subscriberEndPoint)
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
