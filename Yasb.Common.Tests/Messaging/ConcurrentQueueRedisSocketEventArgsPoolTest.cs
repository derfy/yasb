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
    public class ConcurrentQueueRedisSocketEventArgsPoolTest
    {

        [TestMethod]
        public void PoolShouldHaveNotItemsAfterCreation()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(size);
            Assert.AreEqual(0, sut.Size);
        }
        [TestMethod]
        public void ShouldPreallocateSizeItems()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(size);
            sut.PreallocateItems(endPointMock.Object);
            Assert.AreEqual(size, sut.Size);
        }

        [TestMethod]
        public void PreallocateInvokedTwiceForSameAddressShouldNotIncreaseItems()
        {
            var size = 10;
            var endPointMock = new Mock<EndPoint>();
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(size);
            sut.PreallocateItems(endPointMock.Object);
            sut.PreallocateItems(endPointMock.Object);
            Assert.AreEqual(size, sut.Size);
        }
        
       

        [TestMethod]
        public void PreallocateInvokedOn2EndPonintsShouldAllocateNewItems()
        {
            var size = 10;
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(size);
            var endPointMock1 = new Mock<EndPoint>();
            var endPointMock2 = new Mock<EndPoint>();
            var result1 = sut.PreallocateItems(endPointMock1.Object);
            var result2 = sut.PreallocateItems(endPointMock2.Object);
            Assert.AreEqual(2*size, sut.Size);
        }

        [TestMethod]
        public void DequeueInvokedSizeTimesShouldAllocateNewItem()
        {

            var size = 10;
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(size);
            var endPointMock = new Mock<EndPoint>();
            for (int i = 0; i < size; i++)
            {
                sut.Dequeue(endPointMock.Object);
            }
            Assert.AreEqual(0, sut.Size);
            var result=sut.Dequeue(endPointMock.Object);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "No item was preallocated")]
        public void EnqueueAfterNoDequeueShouldThrow()
        {
            var endPointMock = new Mock<EndPoint>();
            var eventArg = new RedisSocketAsyncEventArgs() { RemoteEndPoint=endPointMock.Object};
            var sut = new ConcurrentQueueRedisSocketEventArgsPool(10);
            sut.Enqueue(eventArg);
            
        }

       
    }
}
