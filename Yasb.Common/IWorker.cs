using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Yasb.Common
{
    
    public interface IWorker<TResult> 
    {
        TResult Execute();

        void OnException(Exception ex);

        void OnCompleted(TResult result);
    }
}
