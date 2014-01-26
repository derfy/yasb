using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using System.Collections.Concurrent;
using MongoDB.Driver;
using Yasb.Msmq.Messaging.Configuration;
using MongoDB.Bson.Serialization;

namespace Yasb.Msmq.Messaging
{
    public class MsmqSubscriptionService : ISubscriptionService<MsmqEndPointConfiguration>
    {
        private MsmqEndPointConfiguration _localEndPointConfiguration;
        MongoDatabase _database;
        public MsmqSubscriptionService(MsmqSubscriptionServiceConfiguration configuration)
        {
            _localEndPointConfiguration = configuration.LocalEndPointConfiguration;
            CreateDatabaseConnection(configuration);
            if (!BsonClassMap.IsClassMapRegistered(typeof(MsmqEndPointConfiguration)))
            {
                BsonClassMap.RegisterClassMap<MsmqEndPointConfiguration>(cm =>
                {
                    cm.MapCreator(c => new MsmqEndPointConfiguration(c.Host, c.QueueName));
                    cm.MapProperty(c => c.Host);
                    cm.MapProperty(c => c.QueueName);

                    cm.MapIdProperty(c => c.Value);
                });
            }
        }





        public void SubscribeTo(MsmqEndPointConfiguration topicEndPoint)
        {
            var subscriptions = EnsureCollection(topicEndPoint.Value);
            var subscription = subscriptions.FindOneByIdAs<MsmqEndPointConfiguration>(_localEndPointConfiguration.Value);
            if (subscription == null)
            {
                subscriptions.Insert<MsmqEndPointConfiguration>(_localEndPointConfiguration);
            }
            
        }


        public MsmqEndPointConfiguration[] GetSubscriptionEndPoints()
        {
            var subscriptions = EnsureCollection(_localEndPointConfiguration.Value);
            return subscriptions.FindAllAs<MsmqEndPointConfiguration>().ToArray();
        }

        public void UnSubscribe(string topicName, MsmqEndPointConfiguration subscriberEndPoint)
        {
            throw new NotImplementedException();
        }

        private MongoCollection EnsureCollection(string name)
        {
            var subscriptions = name.Replace("$","");
            if (!_database.CollectionExists(subscriptions))
            {
                _database.CreateCollection(subscriptions);
            }
            return _database.GetCollection(subscriptions);
        }
        private void CreateDatabaseConnection(MsmqSubscriptionServiceConfiguration configuration)
        {
            var settings = new MongoClientSettings() { Server = new MongoServerAddress(configuration.HostName, configuration.Port) };
            var client = new MongoClient(settings);
            var server = client.GetServer();
            _database = server.GetDatabase(configuration.DatabaseName);
        }
    }
}
