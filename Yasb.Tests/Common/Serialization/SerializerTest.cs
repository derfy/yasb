using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Serialization;
using System.IO;
using Yasb.Common.Messaging;
using Newtonsoft.Json;
using Yasb.Common.Tests;
using Yasb.Common.Messaging.EndPoints.Redis;
using Moq;
using Yasb.Common.Serialization.Json;

namespace Yasb.Tests.Common.Serialization
{
    /// <summary>
    /// Summary description for SerializerTest
    /// </summary>
    [TestClass]
    public class SerializerTest
    {
        
        [TestMethod]
        public void CanSerialize()
        {
            var endPointConverter = new Mock<EndPointConverter<RedisEndPoint>>();
            var sut = new JsonNetSerializer<RedisEndPoint>(endPointConverter.Object);
            var graph = new TestMessage("foo");
            Byte[] array = sut.Serialize<TestMessage>(graph);
            var result=System.Text.Encoding.Default.GetString(array);
            Assert.AreEqual("{\"Value\":\"foo\"}", result);
        }


        [TestMethod]
        public void CanDeserialize()
        {
            var endPointConverter = new Mock<EndPointConverter<RedisEndPoint>>();
            var sut = new JsonNetSerializer<RedisEndPoint>(endPointConverter.Object);
            var array = System.Text.Encoding.Default.GetBytes("{}");
            var result = sut.Deserialize<TestMessage>(array);
            Assert.AreEqual(typeof(TestMessage), result.GetType());
        }

        [TestMethod]
        public void DeserializeShouldReturnNull()
        {
            var endPointConverter = new Mock<EndPointConverter<RedisEndPoint>>();
            var sut = new JsonNetSerializer<RedisEndPoint>(endPointConverter.Object);
            var array = new Byte[]{};
            var result = sut.Deserialize<TestMessage>(array);
            Assert.AreEqual(null, result);
        }
    }
}
