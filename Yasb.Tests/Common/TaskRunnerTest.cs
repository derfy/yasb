using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common;
using System.Threading;
using Moq;
using System.Threading.Tasks;

namespace Yasb.Tests.Common
{
    /// <summary>
    /// Summary description for TaskRunnerTest
    /// </summary>
    [TestClass]
    public class TaskRunnerTest
    {
        private TaskRunner _sut;
       

        [TestMethod]
        public void OnlyThreeWorkersShouldBeExecutedAtSameTime()
        {
        
            _sut = new TaskRunner();
            var worker1 = new Mock<IWorker>();
            worker1.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {

                while (true) { }

            });
            _sut.StartWorker(worker1.Object);
            _sut.StartWorker(worker1.Object);
            _sut.StartWorker(worker1.Object);

            _sut.StartWorker(worker1.Object);
            Thread.Sleep(100);
            worker1.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(3));
           
        }

        [TestMethod]
        public void WorkerShouldHandleException()
        {
            var taskScheduler = new CurrentThreadTaskScheduler();
            _sut = new TaskRunner(taskScheduler);
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                throw new Exception();
            });
            _sut.StartWorker(worker.Object);
            worker.Verify(w=>w.Execute(It.IsAny<CancellationToken>()), Times.Once());
            worker.Verify(w => w.OnException(It.IsAny<Exception>()), Times.Once());
        }
        [TestMethod]
        public void ShouldBeToStop()
        {
            _sut = new TaskRunner();
            int calls = 0;
            var worker = new Mock<IWorker>();
            worker.Setup(w => w.Execute(It.IsAny<CancellationToken>())).Callback((CancellationToken t) =>
            {
                calls++;

            });
            _sut.Run(worker.Object);
           _sut.Stop();
            Thread.Sleep(500);
            worker.Verify(w => w.Execute(It.IsAny<CancellationToken>()), Times.Exactly(calls));
        }
    }
}
