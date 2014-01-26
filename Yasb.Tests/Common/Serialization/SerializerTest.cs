using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Yasb.Common.Messaging;
using Newtonsoft.Json;
using Yasb.Common.Tests;
using Moq;
using Yasb.Redis.Messaging;
using Yasb.Common.Messaging.Serialization.Json;
using Yasb.Redis.Messaging.Serialization.MessageDeserializers;

namespace Yasb.Tests.Common.Serialization
{
    
    /// <summary>
    /// Summary description for SerializerTest
    /// </summary>
    [TestClass]
    public class DefaultJsonMessageDeserializerTest
    {
        
        [TestMethod]
        public void CanSerialize()
        {
            var sut = new DefaultJsonMessageDeserializer(typeof(TestMessage));
            var graph = new TestMessage("foo");
            Byte[] array = sut.Serialize(graph);
            var result=System.Text.Encoding.Default.GetString(array);
            Assert.AreEqual("\"{\\\"Value\\\":\\\"foo\\\"}\"", result);
        }


        [TestMethod]
        public void CanDeserialize()
        {
            var sut = new DefaultJsonMessageDeserializer(typeof(TestMessage));
            var array = System.Text.Encoding.Default.GetBytes("{}");
            var result = sut.Deserialize(array);
            Assert.AreEqual(typeof(TestMessage), result.GetType());
        }

        [TestMethod]
        public void DeserializeShouldReturnNull()
        {
            var sut = new DefaultJsonMessageDeserializer(typeof(TestMessage));
            var array = new Byte[]{};
            var result = sut.Deserialize(array);
            Assert.AreEqual(null, result);
        }
    }
}
