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
            var sut = new Serializer(new JsonConverter[]{});
            var graph = new TestMessage("foo");
            Byte[] array = sut.Serialize<TestMessage>(graph);
            var result=System.Text.Encoding.Default.GetString(array);
            Assert.AreEqual("{\"Value\":\"foo\"}", result);
        }


        [TestMethod]
        public void CanDeserialize()
        {
            var sut = new Serializer(new JsonConverter[] { });
            var array = System.Text.Encoding.Default.GetBytes("{}");
            var result = sut.Deserialize<TestMessage>(array);
            Assert.AreEqual(typeof(TestMessage), result.GetType());
        }

        [TestMethod]
        public void DeserializeShouldReturnNull()
        {
            var sut = new Serializer(new JsonConverter[] { });
            var array = new Byte[]{};
            var result = sut.Deserialize<TestMessage>(array);
            Assert.AreEqual(null, result);
        }
    }
}
