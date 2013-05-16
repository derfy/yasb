using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Yasb.Common
{
    public interface ITaskRunner
    {
        void Run(Action<CancellationToken> workerAction, Action<Exception> faultedAction = null);
        void Stop();
    }
}
