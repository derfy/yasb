using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Yasb.Common.Serialization;
using Moq;
using Yasb.Common.Messaging;
using Newtonsoft.Json.Serialization;
using Yasb.Redis.Messaging.Configuration;
using Yasb.Tests.Common.Messaging;
using Yasb.Redis.Messaging;
using Yasb.Common.Tests;

namespace Yasb.Tests.Common.Serialization
{
    
    /// <summary>
    /// Summary description for MessageEnvelopeConverterTest
    /// </summary>
    [TestClass]
    public class MessageEnvelopeConverterTest
    {

        
        [TestMethod]
        public void ShouldBeDeserialized()
        {
            var fromEndPoint = new BusEndPoint("from:fromQueue");
            var toEndPoint = new BusEndPoint("Value", "to:toQueue");
            var contentType = "Yasb.Common.Tests.TestMessage, Yasb.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var message = new JObject();
            var id = Guid.NewGuid().ToString();
            JObject jsonObject = new JObject(
               new JProperty("Id",id),
               new JProperty("ContentType", contentType),
               new JProperty("Message", message),
               new JProperty("From", new JObject(
                   new JProperty("Value", fromEndPoint.Value)
                )),
               new JProperty("To", new JObject(
                   new JProperty("Value", toEndPoint.Value)
                ))
            );
           
            var reader = new JTokenReader(jsonObject);
            var serializer = CreateSerializerMock();
            var sut = new MessageEnvelopeConverter();
            var result = sut.ReadJson(reader, typeof(MessageEnvelope), null, serializer.Object) as MessageEnvelope;
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(typeof(TestMessage), result.ContentType);
            Assert.AreEqual(fromEndPoint.GetType(), result.From.GetType());
            Assert.AreEqual(toEndPoint.GetType(), result.To.GetType());
        }

        private static Mock<JsonSerializer> CreateSerializerMock()
        {
            var serializer = new Mock<JsonSerializer>();
            serializer.Setup(s => s.Binder).Returns(new DefaultSerializationBinder());
            serializer.Setup(s => s.ContractResolver).Returns(new DefaultContractResolver());
            var converters=new JsonConverterCollection();
            converters.Add(new EndPointConverter());
            serializer.Setup(s => s.Converters).Returns(converters);
            return serializer;
        }
        [TestMethod]
        public void ReadJsonShouldReturnNull()
        {
            var sut = new MessageEnvelopeConverter();
            var reader = new Mock<JsonReader>();
            reader.Setup(r => r.TokenType).Returns(JsonToken.Null);
            var serializer = new Mock<JsonSerializer>();
            sut.ReadJson(reader.Object, typeof(MessageEnvelope), null, serializer.Object);
        }
        [TestMethod]
        public void CanConvertShouldReturnTrue()
        {
            var sut = new MessageEnvelopeConverter();
            var result = sut.CanConvert(typeof(MessageEnvelope));
            Assert.AreEqual(true, result);
        }
        [TestMethod]
        public void CanWriteShouldReturnFalse()
        {
            var sut = new MessageEnvelopeConverter();
            Assert.AreEqual(false, sut.CanWrite);
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void WriteJsonShouldRaiseException()
        {
            var sut = new MessageEnvelopeConverter();
            var writer = new Mock<JsonWriter>();
            var serializer = new Mock<JsonSerializer>();
            sut.WriteJson(writer.Object, null, serializer.Object);
        }
    }
}
