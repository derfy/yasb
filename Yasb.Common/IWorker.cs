using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Yasb.Common
{
    
    public interface IWorker
    {
        void Execute();
        void OnException(Exception ex);
        void OnCanceled();
    }
}
