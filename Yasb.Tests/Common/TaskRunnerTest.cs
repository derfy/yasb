using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common;
using System.Threading;
using Moq;

namespace Yasb.Tests.Common
{
    /// <summary>
    /// Summary description for TaskRunnerTest
    /// </summary>
    [TestClass]
    public class TaskRunnerTest
    {
        private TaskRunner _sut;
        [TestInitialize]
        public void Setup()
        {
            _sut = new TaskRunner();
        }

        [TestMethod]
        public void WorkerShouldBeExecutedThreeTimes()
        {
            var worker = new Mock<IWorker>();
            _sut.Run(worker.Object);
            Thread.Sleep(100);
            worker.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [TestMethod]
        public void WorkerShouldHandleException()
        {
            int calls = 0;
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                calls++;
                throw new Exception();
               
            });
            _sut.Run(worker.Object);
            Thread.Sleep(200);
            _sut.Stop();
            worker.Verify(w => w.OnException(It.IsAny<Exception>()), Times.Exactly(calls));
        }
        [TestMethod]
        public void ShouldBeToStop()
        {
            int calls = 0;
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                calls++;

            });
            _sut.Run(worker.Object);
            Thread.Sleep(100);
            worker.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(calls));
            _sut.Stop();
            Thread.Sleep(500);
            worker.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(calls));
        }
    }
}
