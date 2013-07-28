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

    public class WorkerPool<TResult> : IWorkerPool<TResult>
    {

        private const int MaxRunningTasksNumber=3;
        private int _runningTasksNumber = 0;
        private ConcurrentQueue<Task> _waitingTasksQueue = new ConcurrentQueue<Task>();
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        private ManualResetEventSlim _mres = new ManualResetEventSlim(false);


        public WorkerPool(IWorker<TResult> worker)
        {
            Parallel.For(0, MaxRunningTasksNumber, i => _waitingTasksQueue.Enqueue(CreateTask(worker)));
        }
        public Task Run()
        {
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _mres.Reset();
                    Task task = null;
                    while (_waitingTasksQueue.TryDequeue(out task))
                    {
                        Join(task);
                    }
                    _mres.Wait();
                }
            },TaskCreationOptions.LongRunning);
           
        }

        public void Join(Task workerTask)
        {
          
            if (_runningTasksNumber == MaxRunningTasksNumber)
            {
                _mres.Wait();
                _mres.Reset();
            }
           
            workerTask.Start();
        }

        private Task CreateTask(IWorker<TResult> worker)
        {
            var token = _tokenSource.Token;
            token.ThrowIfCancellationRequested();
            var task = new Task<TResult>((state) =>
            {
               
                Interlocked.Increment(ref _runningTasksNumber);
                return worker.Execute();
            }, token);
            task.ContinueWith(t =>
            {
                if (t.Result != null)
                    worker.OnCompleted(t.Result);
                _waitingTasksQueue.Enqueue(CreateTask(worker));
                Interlocked.Decrement(ref _runningTasksNumber);
                _mres.Set();
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(faultedTask =>
            {
                faultedTask.Exception.Handle(ex =>
                {
                    worker.OnException(ex);
                    return true;
                });
            }, TaskContinuationOptions.ExecuteSynchronously|TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(canceledTask =>
            {
                canceledTask.Exception.Handle(ex =>
                {
                    worker.OnException(ex);
                    return true;
                });
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled);
            return task;
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
       

    }
   
}
