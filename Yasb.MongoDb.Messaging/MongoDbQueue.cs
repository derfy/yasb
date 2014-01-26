using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.MongoDb.Messaging
{
    public class MongoDbQueue : IQueue<MongoDbEndPointConfiguration>
    {
       
        private MongoCollection<BsonDocument> _collection;
        private const string TimeOutError = "Operation Timed Out";

        public MongoDbQueue(MongoDbEndPointConfiguration queueEndPoint)
        {
            LocalEndPointConfiguration = queueEndPoint;
            InitializeCollection();
            
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

        public void SetMessageCompleted(string envelopeId,DateTime now)
        {
            _collection.Remove(Query.EQ("_id", envelopeId));
        }
        public void SetMessageInError(string envelopeId, string errorMessage)
        {
            throw new NotImplementedException();
        }
        public void Push(IMessage message)
        {
            var envelope = new MessageEnvelope(message) { Id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() };
            _collection.Insert<MessageEnvelope>(envelope);
        }

        
        public void Clear()
        {
            _collection.RemoveAll();
        }
        public MongoDbEndPointConfiguration LocalEndPointConfiguration { get; private set; }

        private void InitializeCollection()
        {
            var database = MongoDbFactory.CreateDatabase(LocalEndPointConfiguration);
            if (!database.CollectionExists(LocalEndPointConfiguration.QueueName))
            {
                database.CreateCollection(LocalEndPointConfiguration.QueueName);
            }
            _collection = database.GetCollection(LocalEndPointConfiguration.QueueName);
        }






        //private MessageEnvelope CreateMessageEnvelope(IMessage message)
        //{
        //    var objectId = new BsonObjectId(ObjectId.GenerateNewId());
        //    return new MessageEnvelope(objectId.ToString(), message) { LastCreateOrUpdateTimestamp = DateTime.UtcNow.Ticks };

        //}
    }
}
