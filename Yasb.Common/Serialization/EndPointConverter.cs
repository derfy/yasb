using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Yasb.Common.Messaging;

namespace Yasb.Common.Serialization
{
    public abstract class EndPointConverter<TEndPoint> : JsonConverter where TEndPoint : IEndPoint
    {
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var endPoint = JObject.Load(reader)["Value"].ToObject<string>();
            return CreateEndPoint(endPoint);
        }

        protected abstract TEndPoint CreateEndPoint(string endPoint);

        public override bool CanConvert(Type objectType)
        {
            return typeof(TEndPoint).IsAssignableFrom(objectType);
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
