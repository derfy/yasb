using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization;
using Yasb.Common.Messaging;

namespace Yasb.MongoDb.Messaging.Serialization
{
    public static class SerializerRegisterer
    {
        public static void Register<T>() {
            BsonSerializer.RegisterSerializer(typeof(T), new MongoDbSerializer());
        } 
    }
}
