using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Collections.Concurrent;

namespace Yasb.Common
{

    public class WorkerPool : IWorkerPool
    {

        private ConcurrentQueue<Task> _waitingTasksQueue = new ConcurrentQueue<Task>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        

        public WorkerPool(IWorker worker,int maxRunningTaskNumber=3)
        {
            MaxRunningTasksNumber = maxRunningTaskNumber;
            Parallel.For(0, MaxRunningTasksNumber, i => _waitingTasksQueue.Enqueue(CreateTask(worker)));
        }
        public Task Run()
        {
            return Task.Factory.StartNew(() => DoWork(() => true), TaskCreationOptions.LongRunning);
        }

        public void DoWork(Func<bool> predicate)
        {
            Task task;
            while (predicate())
            {
                
                if (_waitingTasksQueue.TryDequeue(out task) && task.Status!=TaskStatus.Canceled)
                {
                    task.Start();
                }
            }
           
        }

        public int MaxRunningTasksNumber { get; private set; }
        

        private Task CreateTask(IWorker worker)
        {
            var token = _tokenSource.Token;
            token.ThrowIfCancellationRequested();
            var task = new Task(() => worker.Execute(), token);
            task.ContinueWith(t =>
            {
                _waitingTasksQueue.Enqueue(CreateTask(worker));

            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(faultedTask =>
            {
                faultedTask.Exception.Handle(ex =>
                {
                    worker.OnException(ex);
                    _waitingTasksQueue.Enqueue(CreateTask(worker));
                    return true;
                });
            }, TaskContinuationOptions.ExecuteSynchronously|TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(canceledTask =>
            {
                worker.OnCanceled();
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled);
            return task;
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
       

    }
   
}
