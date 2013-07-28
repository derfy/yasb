using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging
{
    public abstract class QueueEndPoint<TConnection>
    {
        public QueueEndPoint(TConnection connection, string name)
        {
            Connection = connection;
            Name = name;
        }

        public QueueEndPoint(string value)
        {
           
        }
        public QueueEndPoint()
        {

        }
        public virtual string Name { get; protected set; }

        public virtual TConnection Connection { get; protected set; }

        public Type Type { get { return GetType(); } }
        
        public abstract string Value { get; }
        
        internal static QueueEndPoint<TConnection> CreateFrom(string value, Type type)
        {
            return Activator.CreateInstance(type,value) as QueueEndPoint<TConnection>;
        }
    }
}
