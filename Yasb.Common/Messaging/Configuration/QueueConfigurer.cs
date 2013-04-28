using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Configuration
{
    public class QueueConfigurer<TConnection>
    {
        public QueueConfigurer()
        {
            Built = new QueueConfiguration<TConnection>();
        }

        public QueueConfigurer<TConnection> WithEndPoint(string connectionName, string queueName, string endPointName)
        {
            Built.EndPointConfiguration = new EndPointConfiguration();
            Built.EndPointConfiguration.AddNamedEndPoint(connectionName, queueName, endPointName);
            return this;
        }
        

        public QueueConfigurer<TConnection> ConfigureConnections<TConnectionConfigurer>(Action<TConnectionConfigurer> action)
            where TConnectionConfigurer : IConnectionConfigurer<TConnection>
        {
            var connectionConfigurer = Activator.CreateInstance<TConnectionConfigurer>();
            action(connectionConfigurer);
            Built.ConnectionConfiguration = connectionConfigurer.Built;
            return this;
        }


        public QueueConfiguration<TConnection> Built { get; private set; }
    }
}
