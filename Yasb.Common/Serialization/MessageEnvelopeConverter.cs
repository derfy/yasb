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
    public class MessageEnvelopeConverter : JsonConverter 
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
            var id = jsonObject.Property("Id").Value.ToObject<Guid>();
            var contentType = jsonObject.Property("ContentType").Value.ToObject<Type>();
            var from = jsonObject.Property("From").Value.ToObject<BusEndPoint>(serializer);
            var to = jsonObject.Property("To").Value.ToObject<BusEndPoint>(serializer);
            var message = jsonObject.Property("Message").Value.ToObject(contentType,serializer) as IMessage;
            
            
            return new MessageEnvelope(message,id,from,to);
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
