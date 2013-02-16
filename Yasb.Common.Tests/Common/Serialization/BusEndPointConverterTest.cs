using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;
using Newtonsoft.Json.Linq;
namespace Yasb.Tests.Common.Serialization
{
    /// <summary>
    /// Summary description for BusEndPointConverterTest
    /// </summary>
    [TestClass]
    public class BusEndPointConverterTest
    {
       
        [TestMethod]
        public void ShouldBeDeserialized()
        {
            JObject o = new JObject(
               new JProperty("Host", "Foo"),
               new JProperty("Port", 80),
               new JProperty("QueueName", "Bar")
            );
            var reader = new JTokenReader(o);
            var serializer = new Mock<JsonSerializer>();
            var sut = new BusEndPointConverter();

            var result = sut.ReadJson(reader, typeof(BusEndPoint), null, serializer.Object) as BusEndPoint;
            Assert.AreEqual(result.Host, "Foo");
            Assert.AreEqual(result.Port, 80);
            Assert.AreEqual(result.QueueName, "Bar");
        }
        [TestMethod]
        public void ReadJsonShouldReturnNull()
        {
            var sut = new BusEndPointConverter();
            var reader = new Mock<JsonReader>();
            reader.Setup(r => r.TokenType).Returns(JsonToken.Null);
            var serializer = new Mock<JsonSerializer>();
            sut.ReadJson(reader.Object, typeof(BusEndPoint), null, serializer.Object);
        }
        [TestMethod]
        public void CanConvertShouldReturnTrue()
        {
            var sut = new BusEndPointConverter();
            var result = sut.CanConvert(typeof(BusEndPoint));
            Assert.AreEqual(true,result);
        }
        [TestMethod]
        public void CanWriteShouldReturnFalse()
        {
            var sut = new BusEndPointConverter();
            Assert.AreEqual(false, sut.CanWrite);
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void WriteJsonShouldRaiseException()
        {
            var sut = new BusEndPointConverter();
            var writer=new Mock<JsonWriter>();
            var serializer = new Mock<JsonSerializer>();
            sut.WriteJson(writer.Object, null, serializer.Object);
        }
    }
}
