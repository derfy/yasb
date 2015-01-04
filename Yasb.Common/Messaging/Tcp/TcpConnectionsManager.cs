using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yasb.Common.Messaging.Tcp
{
    public class TcpConnectionsManager<TConnection> where TConnection : TcpConnectionState
    {
        private ITcpConnectionsPool<TConnection> _connectionsPool;
        public TcpConnectionsManager(ITcpConnectionsPool<TConnection> connectionsPool)
        {
            _connectionsPool = connectionsPool;
        }



        public Task<byte[]> SendAsync(byte[][] data)
        {
            var connection = _connectionsPool.Dequeue();
            var tcs = new TaskCompletionSource<byte[]>();
            if (!connection.IsBound)
            {
                connection.ConnectAsync().ContinueWith(connectTask =>
                {
                    if (!connectTask.Result)
                    {
                        tcs.SetException(new ApplicationException("Failed to connect to the Server"));
                        return;
                    }
                    connection.SendAsync(data).ContinueWith(sendTask =>
                    {
                        tcs.SetResult(sendTask.Result);
                        _connectionsPool.Enqueue(connection);
                    });
                });
            }
            else
            {

                connection.SendAsync(data).ContinueWith(sendTask =>
                {
                    tcs.SetResult(sendTask.Result);
                    _connectionsPool.Enqueue(connection);
                });
            }
            return tcs.Task;
        }




    }
}
