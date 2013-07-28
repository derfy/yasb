using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Yasb.Common.Serialization.MessageDeserializers
{
    public class SubscriptionMessageDeserializer<TConnection> : IMessageDeserializer
    {
       
        public IMessage DeserializeFrom(JToken messageToken, JsonSerializer serializer)
        {
            var subscriptions = messageToken["Subscriptions"].Select((JToken subscriptionsToken) =>
            {
                var endPointToken=subscriptionsToken["EndPoint"];
                var name = endPointToken["Name"].ToObject<string>(serializer);
                var type = endPointToken["Type"].ToObject<Type>(serializer);
                var connection = endPointToken["Connection"].ToObject<TConnection>(serializer);
                var endPoint = Activator.CreateInstance(type,connection, name) as QueueEndPoint<TConnection>;
                var handler = subscriptionsToken["Handler"].ToObject<string>(serializer);
                return new SubscriptionInfo<TConnection>(endPoint, handler); 
            }).ToArray();
            var typeName = messageToken["TypeName"].ToObject<string>();
            return new SubscriptionMessage<TConnection>(typeName, subscriptions);
        }
    }
}
