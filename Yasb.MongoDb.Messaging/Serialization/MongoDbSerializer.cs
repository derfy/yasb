using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.EndPoints.MongoDb;

namespace Yasb.MongoDb.Messaging.Serialization
{
	public class MongoDbSerializer : BsonBaseSerializer
	{
		public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
		{
		   
			var doc = BsonDocument.ReadFrom(bsonReader);
			var id = doc["_id"].AsString;
			var contentType = Type.GetType(doc["ContentType"].AsString);
			var message = BsonSerializer.Deserialize(doc["Message"].AsBsonDocument, contentType) as IMessage;
            var from = BsonSerializer.Deserialize(doc["From"].AsBsonDocument, contentType) as MongoDbEndPoint;
			var env = new MessageEnvelope(message)
			{
                Id=id,
				RetriesNumber = doc["RetriesNumber"].AsInt32
			};
            env.SetHeader<Int64>("LastCreateOrUpdateTimestamp", doc["LastCreateOrUpdateTimestamp"].AsInt64);
			if (!doc["StartTimestamp"].IsBsonNull)
			{
				env.StartTimestamp = doc["StartTimestamp"].AsInt64;
			}
			if (!doc["LastErrorMessage"].IsBsonNull)
			{
				env.LastErrorMessage = doc["LastErrorMessage"].AsString;
			}
			return env;
		}

		public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
		{
            var envelope = value as MessageEnvelope;
			var message = envelope.Message.ToBsonDocument(envelope.Message.GetType());
			var bdoc = new { _id = envelope.Id, 
							 StartTimestamp = envelope.StartTimestamp, 
							 //From = envelope.From, 
							 Message=message,
							 RetriesNumber = envelope.RetriesNumber,
							 LastErrorMessage = envelope.LastErrorMessage,
                             LastCreateOrUpdateTimestamp = envelope.GetHeader<long>("LastCreateOrUpdateTimestamp")
			};
			BsonSerializer.Serialize(bsonWriter, bdoc, options);
		}
	}
}
