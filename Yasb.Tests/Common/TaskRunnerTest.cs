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
        private WorkerPool _sut;

        [TestMethod]
        public void NoMoreThanMaxCapacityShouldBeExecutedAtSameTime()
        {
            
            
            var worker = new Mock<IWorker>();
            _sut = new WorkerPool(worker.Object);
            var executionTimes = _sut.MaxRunningTasksNumber +1;
            CountdownEvent cde = new CountdownEvent(executionTimes);
            worker.Setup(w => w.Execute()).Callback(() =>
            {
                cde.Signal();

            });
            int i = 0;
            _sut.DoWork(() => i++ < executionTimes);
            cde.Wait(20);
            worker.Verify(w => w.Execute(),Times.Exactly(_sut.MaxRunningTasksNumber));

        }
        [TestMethod]
        public void LessThanMaxRunningTasksNumberWorkersCanBeExecutedAtSameTime()
        {
           
            var worker = new Mock<IWorker>();
            _sut = new WorkerPool(worker.Object);
            var executionTimes = _sut.MaxRunningTasksNumber - 1;
            CountdownEvent cde = new CountdownEvent(executionTimes);
            worker.Setup(w => w.Execute()).Callback(() =>
            {
                cde.Signal();

            });
            int i = 0;
            _sut.DoWork(() => i++ < executionTimes);
            cde.Wait(200);
            worker.Verify(w => w.Execute(), Times.Exactly(executionTimes));
           
        }
        [TestMethod]
        public void MaxRunningTasksNumberWorkersCanBeExecutedAtSameTime()
        {

            var worker = new Mock<IWorker>();
            _sut = new WorkerPool(worker.Object);
            var executionTimes = _sut.MaxRunningTasksNumber;
            CountdownEvent cde = new CountdownEvent(executionTimes);
            worker.Setup(w => w.Execute()).Callback(() =>
            {
                cde.Signal();

            });
            int i = 0;
            _sut.DoWork(() => i++ < executionTimes);
            cde.Wait(200);
            worker.Verify(w => w.Execute(), Times.Exactly(executionTimes));

        }
        [TestMethod]
        public void WorkerShouldHandleException()
        {
           
            var worker = new Mock<IWorker>();
            _sut = new WorkerPool(worker.Object);
            var executionTimes = _sut.MaxRunningTasksNumber;
            CountdownEvent cde = new CountdownEvent(executionTimes);
            worker.Setup(w => w.Execute()).Throws(new Exception());
            worker.Setup(w => w.OnException(It.IsAny<Exception>())).Callback((Exception t) =>
            {
                 cde.Signal();
            });
            int i = 0;
            _sut.DoWork(() => i++ < executionTimes);
            cde.Wait(100);
            worker.Verify(w => w.OnException(It.IsAny<Exception>()), Times.Exactly(executionTimes));
        }
        [TestMethod]
        public void ShouldBeToStop()
        {
            var mre = new ManualResetEventSlim();
            var worker = new Mock<IWorker>();
            _sut = new WorkerPool(worker.Object);
            worker.Setup(w => w.OnCanceled()).Callback(() =>
            {
                 mre.Set();

            });
            _sut.Run();
           _sut.Stop();
           mre.Wait();
           Assert.IsTrue(mre.IsSet);
        }
    }
}
