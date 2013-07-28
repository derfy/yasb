using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using Yasb.Common.Messaging;

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
            var handler = doc["Handler"].AsString;
			var env = new MessageEnvelope(id,message, doc["From"].AsString, doc["To"].AsString,handler)
			{
				LastCreateOrUpdateTimestamp = doc["LastCreateOrUpdateTimestamp"].AsInt64,
				RetriesNumber = doc["RetriesNumber"].AsInt32
			};
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
							 From = envelope.From, 
							 To = envelope.To, 
							 Message=message,
							 RetriesNumber = envelope.RetriesNumber,
							 LastErrorMessage = envelope.LastErrorMessage,
							 LastCreateOrUpdateTimestamp=envelope.LastCreateOrUpdateTimestamp
			};
			BsonSerializer.Serialize(bsonWriter, bdoc, options);
		}
	}
}
