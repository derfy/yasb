using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging;
using Yasb.Common.Serialization;
using Yasb.Common.Messaging;
using Yasb.Common.Tests;
using Yasb.Redis.Messaging.Client.Interfaces;
using System.Net;

namespace Yasb.Tests.Messaging
{
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
        private RedisQueue _sut;
        private Mock<ISerializer> _serializerMock= new Mock<ISerializer>();
        private Mock<IRedisClient> _redisClientMock = new Mock<IRedisClient>();
        public RedisQueueTest()
        {
            _sut = new RedisQueue("test", _serializerMock.Object, _redisClientMock.Object);
           
        }
        [TestMethod]
        public void ShouldBeAbleToPush()
        {

            
            var bytes = Encoding.Default.GetBytes("foo");
            _serializerMock.Setup(c => c.Serialize(It.IsAny<MessageEnvelope>())).Returns(bytes);
            _redisClientMock.Setup(c => c.LPush("test", bytes));
            _sut.Push(new TestMessage("This is a test"), "localConnection:foo");
            _redisClientMock.VerifyAll();
            _serializerMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldBeAbleToDequeue()
        {
            MessageEnvelope newEnvelope = null;
            var envelope = new MessageEnvelope(new TestMessage("This is a test"), "localConnection:foo", "localConnection:bar", DateTimeOffset.UtcNow.Ticks);
            byte[] script1 = Encoding.Default.GetBytes("foo");
            var now=DateTime.Now;
            var timoutWindow = new TimeSpan();
            _redisClientMock.Setup(c => c.EvalSha("TryGetEnvelope.lua", 1, "test", now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString())).Returns(script1);
            _serializerMock.Setup(c => c.Deserialize<MessageEnvelope>(script1)).Returns(envelope);
            var res = _sut.TryDequeue(now, timoutWindow, out newEnvelope);
            _redisClientMock.VerifyAll();
            _serializerMock.VerifyAll();
            Assert.IsTrue(res);
            Assert.AreEqual(envelope, newEnvelope);
        }
        [TestMethod]
        public void ShouldNotBeAbleToDequeue()
        {
            MessageEnvelope envelope = null;
            var now = DateTime.Now;
            var timoutWindow = new TimeSpan();
            _redisClientMock.Setup(c => c.EvalSha("TryGetEnvelope.lua", 1, "test", now.Subtract(timoutWindow).Ticks.ToString(), now.Ticks.ToString())).Returns(() => null);
            var res = _sut.TryDequeue(now, timoutWindow, out envelope);
            _serializerMock.Verify(c => c.Deserialize<MessageEnvelope>(It.IsAny<byte[]>()), Times.Never());
            _redisClientMock.VerifyAll();
            Assert.IsFalse(res);
            Assert.AreEqual(null, envelope);
        }

        [TestMethod]
        public void ShouldNotBeAbleToInvokeSetMessageCompleted()
        {
            var envelopeId=Guid.NewGuid();
            var now = DateTime.Now;
            _sut.SetMessageCompleted(envelopeId.ToString());
            _redisClientMock.Verify(c => c.EvalSha("SetMessageCompleted.lua", 1, It.IsAny<string[]>()), Times.Once());
        }
    }
}
