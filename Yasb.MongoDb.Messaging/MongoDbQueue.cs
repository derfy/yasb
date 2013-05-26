using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using Yasb.Common.Messaging.Configuration.MongoDb;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueue : IQueue
    {
       
        private MongoCollection<BsonDocument> _collection;
        private const string TimeOutError = "Operation Timed Out";

        public MongoDbQueue(MongoDbConnection connection, string queueName)
        {
            InitializeCollection(connection,queueName);
            LocalEndPoint = string.Format("{0}:{1}:{2}",connection.Host,connection.Database,queueName);
        }

       
        public bool TryDequeue(DateTime now, TimeSpan timoutWindow, out MessageEnvelope envelope)
        {
            envelope = null;
            var newEnvelope = _collection.FindAs<MessageEnvelope>(Query.Exists("LastCreateOrUpdateTimestamp"))
                                         .SetSortOrder(SortBy.Ascending("LastCreateOrUpdateTimestamp")).SetLimit(1).FirstOrDefault();
            if (newEnvelope == null || newEnvelope.RetriesNumber >= 5)
                return false;
            string lastErrorMessage = string.Empty;
            var query = Query.And(Query.EQ("_id", newEnvelope.Id), Query.EQ("RetriesNumber", newEnvelope.RetriesNumber));
            if (newEnvelope.StartTime.HasValue)
            {
                if (now.Subtract(newEnvelope.StartTime.Value) < timoutWindow)
                {
                    _collection.Update(Query.EQ("_id", newEnvelope.Id), Update.Set("LastCreateOrUpdateTimestamp", now.Ticks));
                    return false;
                }
                lastErrorMessage = TimeOutError;
            }
            
            envelope = _collection.FindAndModify(query, null,
                                                 Update.Set("StartTimestamp", now.Ticks)
                                                       .Set("LastCreateOrUpdateTimestamp", now.Ticks)
                                                       .Set("LastErrorMessage", lastErrorMessage)
                                                       .Inc("RetriesNumber", 1), true).GetModifiedDocumentAs<MessageEnvelope>();
            return envelope!=null;
        }

        public void SetMessageCompleted(string envelopeId)
        {
            _collection.Remove(Query.EQ("_id", envelopeId));
        }
        public void SetMessageInError(string envelopeId, string errorMessage)
        {
            throw new NotImplementedException();
        }
        public void Push(MessageEnvelope envelope)
        {
            var objectId = new BsonObjectId(ObjectId.GenerateNewId());
            envelope.Id = objectId.ToString();
           
            _collection.Insert<MessageEnvelope>(envelope);
        }
        public void Clear()
        {
            _collection.RemoveAll();
        }
        public string LocalEndPoint { get; private set; }

        private void InitializeCollection(MongoDbConnection connection, string queueName)
        {
            var database = MongoDbFactory.CreateDatabase(connection);
            if (!database.CollectionExists(queueName))
            {
               database.CreateCollection(queueName);
            }
            _collection = database.GetCollection(queueName);
        }



       
    }
}
