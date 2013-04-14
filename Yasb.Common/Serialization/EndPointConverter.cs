using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Yasb.Common.Messaging;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Common.Serialization
{
    public class EndPointConverter: JsonConverter 
    {
        public EndPointConverter()
        {
        }
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var endPointValue = JObject.Load(reader)["Value"].ToObject<string>();
            return new BusEndPoint(endPointValue);
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
