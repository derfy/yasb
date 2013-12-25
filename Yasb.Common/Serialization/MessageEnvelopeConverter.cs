using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yasb.Common.Messaging;
using Yasb.Common.Serialization.MessageDeserializers;

namespace Yasb.Common.Serialization
{
    

    
    public  class MessageEnvelopeConverter : JsonConverter 
    {

        private Func<Type, IMessageDeserializer> _messageDeserializerFactory;

        public MessageEnvelopeConverter()
        {

        }
        public MessageEnvelopeConverter(Func<Type,IMessageDeserializer> messageDeserializerFactory)
        {
            _messageDeserializerFactory = messageDeserializerFactory;
        }
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
            var messageDeserializer = _messageDeserializerFactory(contentType) as IMessageDeserializer;
            var message = messageDeserializer.DeserializeFrom(jsonObject.Property("Message").Value, serializer);
            var envelopeId = jsonObject.Property("Id").Value.ToObject<string>();
            //var from = jsonObject.Property("From").Value.ToObject<string>(serializer);
            //var to = jsonObject.Property("To").Value.ToObject<string>(serializer);
            
            var envelope = new MessageEnvelope(envelopeId,message);
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
