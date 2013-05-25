using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yasb.Common.Messaging;

namespace Yasb.Common.Serialization
{
    public  class MessageEnvelopeConverter : JsonConverter 
    {
        
       
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var jobject = JObject.Load(reader);
            return PopuplaleFrom(jobject,serializer);
        }

        private object PopuplaleFrom(JObject jsonObject,JsonSerializer serializer)
        {
            var contentType = jsonObject.Property("ContentType").Value.ToObject<Type>();
            var message = jsonObject.Property("Message").Value.ToObject(contentType, serializer) as IMessage;

            var from = jsonObject.Property("From").Value.ToObject<string>(serializer);
            var to = jsonObject.Property("To").Value.ToObject<string>(serializer);
            var lastCreateOrUpdateTimestamp = jsonObject.Property("LastCreateOrUpdateTimestamp").Value.ToObject<long>(serializer);
          
            var envelope = new MessageEnvelope(message, from, to, lastCreateOrUpdateTimestamp);
            
            if (jsonObject.Property("Id") != null)
                envelope.Id = jsonObject.Property("Id").Value.ToObject<string>();
            if (jsonObject.Property("StartTimestamp") != null)
                envelope.StartTimestamp = jsonObject.Property("StartTimestamp").Value.ToObject<long?>();
            if (jsonObject.Property("RetriesNumber") != null)
                envelope.RetriesNumber = jsonObject.Property("RetriesNumber").Value.ToObject<int>();
            if (jsonObject.Property("LastErrorMessage") != null)
                envelope.LastErrorMessage = jsonObject.Property("LastErrorMessage").Value.ToObject<string>();
            return envelope;
        }

       
       
        public override bool CanConvert(Type objectType)
        {
            return typeof(MessageEnvelope).IsAssignableFrom(objectType);
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
