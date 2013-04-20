using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasb.Redis.Messaging.Client.Interfaces
{
    public interface ICommandResultProcessor : IDisposable
    {
        byte[] ExpectInt();
        byte[] ExpectSingleLine();
        byte[][] ExpectMultiLine();
        byte[][] ExpectMultiBulkData();
        byte[] ExpectBulkData();
    }
}
