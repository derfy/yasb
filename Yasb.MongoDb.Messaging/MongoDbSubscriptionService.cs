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
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.MongoDb.Messaging
{

  

    public class MongoDbSubscription {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        //[BsonSerializer(typeof(MyClassSerializer))]
        public string[] Endpoints { get; set; }
    }
    public class MongoDbSubscriptionService : ISubscriptionService<MongoDbEndPointConfiguration>
    {
        private MongoCollection _collection;
        public MongoDbSubscriptionService(MongoDatabase database)
        {
            InitializeCollection(database);
        }


        public MongoDbEndPointConfiguration[] GetSubscriptionEndPoints()
        {
            throw new NotImplementedException();
        }





        public void SubscribeTo(MongoDbEndPointConfiguration topicEndPoint)
        {
            throw new NotImplementedException();
        }
        public void UnSubscribe(string topicName, MongoDbEndPointConfiguration subscriberEndPoint)
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
