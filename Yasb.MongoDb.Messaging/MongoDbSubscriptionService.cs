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

namespace Yasb.MongoDb.Messaging
{

    public class MyClassSerializer :  IBsonArraySerializer {
    // ...

    // implement GetItemSerializationInfo
        public BsonSerializationInfo GetItemSerializationInfo()
        {
            throw new NotImplementedException();
        }

        public object Deserialize(MongoDB.Bson.IO.BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(MongoDB.Bson.IO.BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            throw new NotImplementedException();
        }

        public IBsonSerializationOptions GetDefaultSerializationOptions()
        {
            throw new NotImplementedException();
        }

        public void Serialize(MongoDB.Bson.IO.BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            throw new NotImplementedException();
        }
    }


    public class MongoDbSubscription {
        public ObjectId Id { get; set; }
        public string TypeName { get; set; }
        //[BsonSerializer(typeof(MyClassSerializer))]
        public string[] Endpoints { get; set; }
    }
    public class MongoDbSubscriptionService : ISubscriptionService
    {
        private MongoCollection _collection;
        public MongoDbSubscriptionService(MongoDatabase database)
        {
            InitializeCollection(database);
        }
        public string[] GetSubscriptionEndPoints(string typeName)
        {
            return _collection.FindOneAs<MongoDbSubscription>(Query.EQ("TypeName", typeName)).Endpoints;
        }

        public void Subscribe(string typeName, string subscriberEndPoint)
        {

            _collection.Update(Query.EQ("TypeName", typeName), Update.AddToSet("Endpoints", subscriberEndPoint), UpdateFlags.Upsert);
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
