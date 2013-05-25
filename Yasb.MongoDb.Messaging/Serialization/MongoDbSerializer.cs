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
			var contentType = Type.GetType(doc["ContentType"].AsString);
			var message=BsonSerializer.Deserialize(doc["Message"].AsBsonDocument, contentType) as IMessage;
			var env = new MessageEnvelope(message, doc["From"].AsString, doc["To"].AsString, doc["LastCreateOrUpdateTimestamp"].AsInt64)
			{
				Id = doc["_id"].AsString,
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
			var message = envelope.Message.ToBsonDocument(envelope.ContentType);
			var bdoc = new { _id = envelope.Id, 
							 StartTimestamp = envelope.StartTimestamp, 
							 From = envelope.From, 
							 To = envelope.To, 
							 Message=message,
							 RetriesNumber = envelope.RetriesNumber,
							 LastErrorMessage = envelope.LastErrorMessage,
							 LastCreateOrUpdateTimestamp=envelope.LastCreateOrUpdateTimestamp,
							 ContentType=envelope.ContentType.AssemblyQualifiedName
			};
			BsonSerializer.Serialize(bsonWriter, bdoc, options);
		}
	}
}
