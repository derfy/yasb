﻿using System;
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
using Yasb.Common.Tests.Configuration;
using Yasb.Common.Extensions;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging.EndPoints.Redis;
using Yasb.Common.Messaging.EndPoints;
namespace Yasb.Tests.Messaging
{
    /// <summary>
    /// Summary description for RedisQueueTest
    /// </summary>
    [TestClass]
    public class RedisQueueTest
    {
        private IQueue<RedisEndPoint> _sut;
        private Mock<ISerializer> _serializerMock= new Mock<ISerializer>();
        private Mock<IRedisClient> _redisClientMock = new Mock<IRedisClient>();
        private RedisEndPoint _endPointTest;
        private RedisEndPoint _fromEndPoint = new RedisEndPoint("", "");

        private RedisQueueFactory _queueFactory;
        public RedisQueueTest()
        {
          //  var connection= new RedisConnection("foo", 8080);
            _endPointTest = new RedisEndPoint("localhost", "test") { Port=8080};
           // _serializerMock.Setup(s=>s.
          _queueFactory = new RedisQueueFactory(_serializerMock.Object, c => _redisClientMock.Object);

            _sut = _queueFactory.CreateQueue(_endPointTest);// new RedisQueue(_endPointTest, _serializerMock.Object, _redisClientMock.Object);
           
        }

        [TestInitialize()]
        public void BeforeTest()
        {
            _sut.Clear();

        }
        //  [Ignore]
       

        [TestMethod]
        public void ShouldBeAbleToPush()
        {
            
            var bytes = Encoding.Default.GetBytes("foo");
            _serializerMock.Setup(c => c.Serialize(It.IsAny<MessageEnvelope>())).Returns(bytes);
            _redisClientMock.Setup(c => c.EvalSha("PushMessage.lua", 1, It.IsAny<byte[]>(), bytes));
            var message = new TestMessage("This is a test");
            var envelope = new MessageEnvelope(message);
            _sut.Push(envelope);
            _redisClientMock.VerifyAll();
            _serializerMock.VerifyAll();
        }

        [TestMethod]
        public void ShouldBeAbleToDequeue()
        {
         //   _endPointTest.Setup(e => e.Name).Returns("test");
            MessageEnvelope newEnvelope = null;
            var envelope = new MessageEnvelope(new TestMessage("This is a test"));
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
           // _endPointTest.Setup(e => e.Name).Returns("test");
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
            _sut.SetMessageCompleted(envelopeId.ToString(), DateTime.Now);
            _redisClientMock.Verify(c => c.EvalSha("SetMessageCompleted.lua", 1, It.IsAny<string[]>()), Times.Once());
        }
    }
}
