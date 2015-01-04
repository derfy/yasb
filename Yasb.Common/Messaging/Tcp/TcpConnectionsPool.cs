using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Yasb.Common.Messaging;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Yasb.Common.Messaging.Tcp
{
    public class TcpConnectionsPool<TConnection> : ITcpConnectionsPool<TConnection> where TConnection : TcpConnectionState 
    {
        private ConcurrentQueue<TConnection> _internalQueue = new ConcurrentQueue<TConnection>();
        private ManualResetEventSlim _mres = new ManualResetEventSlim(false);

        public event EventHandler Empty;

        public TcpConnectionsPool(int size, Func<TConnection> factory)
        {
        
            Initialise(size, factory);
        }

      

        internal void Initialise(int size,  Func<TConnection> factory)
        {
            for (int ii = 0; ii < size; ii++)
            {
                _internalQueue.Enqueue(factory());
            }
        }

        public TConnection Dequeue()
        {

            TConnection connection = default(TConnection);
            do
            {
                while (!_internalQueue.TryDequeue(out connection))
                {
                    if (Empty != null)
                    {
                        Empty(this,new EventArgs());
                    }
                    _mres.Wait();
                    _mres.Reset();
                }
            } while (!connection.Reserve());
            return connection;
        }

        public int Size { get { return _internalQueue.Count; } }




        public void Enqueue(TConnection connection)
        {
           
            _internalQueue.Enqueue(connection);
            connection.UnReserve();
            _mres.Set();
        }

       

     
       

    }
}
