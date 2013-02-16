using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Yasb.Common.Messaging;
using Newtonsoft.Json.Linq;

namespace Yasb.Common.Serialization
{
    public class BusEndPointConverter : JsonConverter 
    {
       

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jobject = JObject.Load(reader);
            return PopuplaleFrom(jobject);
        }

        private object PopuplaleFrom(JObject jsonObject)
        {
            var host = jsonObject.Property("Host").Value.ToObject<string>();
            var port = jsonObject.Property("Port").Value.ToObject<int>();
            var queueName = jsonObject.Property("QueueName").Value.ToObject<string>();
            return new BusEndPoint(host,port,queueName);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(BusEndPoint).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
