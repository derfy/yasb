using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Redis.Messaging.Client;
using System.Net;
using Moq;

namespace Yasb.Tests.Messaging
{
    /// <summary>
    /// Summary description for ConcurrentQueueRedisSocketEventArgsPoolTest
    /// </summary>
    [TestClass]
    public class RedisSocketAsyncEventArgsPoolTest
    {

        [TestMethod]
        public void PoolShouldHaveSizeItemsAfterCreation()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock.Object);
            Assert.AreEqual(size, sut.Size);
        }
        [TestMethod]
        public void ShouldDequeueItem()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock.Object);
            var res=sut.Dequeue();
            Assert.AreEqual(size-1, sut.Size);
        }

        [TestMethod]
        public void ShouldEnqueueItem()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new RedisSocketAsyncEventArgsPool(size, endPointMock.Object);
            var item = new RedisSocketAsyncEventArgs();
            sut.Enqueue(item);
            Assert.AreEqual(size + 1, sut.Size);
        }
       

      

       
    }
}
