using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yasb.Tests.Common
{
    public class CurrentThreadTaskScheduler : TaskScheduler
    {
        protected override void QueueTask(Task task)
        {
            
            TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }

       
    }
}
