﻿using System;
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
            JObject fromEndPoint = new JObject(
               new JProperty("Host", "from"),new JProperty("Port", 80),new JProperty("QueueName", "fromQueue")
            );
            JObject toEndPoint = new JObject(
               new JProperty("Host", "to"),new JProperty("Port", 80),new JProperty("QueueName", "toQueue")
            );
            var contentType = "Yasb.Tests.Common.Serialization.FooMessage, Yasb.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var message = new JObject();
            var id = Guid.NewGuid();
            JObject jsonObject = new JObject(
               new JProperty("Id",id.ToString()),
               new JProperty("ContentType", contentType),
               new JProperty("Message", message),
               new JProperty("From", fromEndPoint),
               new JProperty("To", toEndPoint)
            );
           
            var reader = new JTokenReader(jsonObject);
            var serializer = CreateSerializerMock();
            var sut = new MessageEnvelopeConverter();

            var result = sut.ReadJson(reader, typeof(MessageEnvelope), null, serializer.Object) as MessageEnvelope;
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(typeof(FooMessage), result.ContentType);
            Assert.AreEqual("from",result.From.Host);
            Assert.AreEqual(80,result.From.Port);
            Assert.AreEqual("fromQueue",result.From.QueueName);
            Assert.AreEqual("to", result.To.Host);
            Assert.AreEqual(80, result.To.Port);
            Assert.AreEqual("toQueue", result.To.QueueName);
        }

        private static Mock<JsonSerializer> CreateSerializerMock()
        {
            var serializer = new Mock<JsonSerializer>();
            serializer.Setup(s => s.Binder).Returns(new DefaultSerializationBinder());
            serializer.Setup(s => s.ContractResolver).Returns(new DefaultContractResolver());
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