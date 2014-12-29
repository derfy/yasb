using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Messaging;
using Yasb.Redis.Messaging;
using Newtonsoft.Json;
using System.Net;
using Yasb.Redis.Messaging.Client.Interfaces;
using Yasb.Redis.Messaging.Client;
using Yasb.Redis.Messaging.Configuration;
using Autofac.Core;
using Yasb.Msmq.Messaging;
using Yasb.Msmq.Messaging.Configuration;
using Yasb.MongoDb.Messaging;
using Yasb.MongoDb.Messaging.Configuration;

namespace Yasb.Wireup.ConfiguratorExtensions.MongoDb
{


    public static class MongoDbConfiguratorExtensions 
    {

        public static Configurator<MongoDbEndPoint> ConfigureEndPoints(this Configurator<MongoDbEndPoint> configurator, Action<EndPointsConfigurer<MongoDbEndPointConfiguration>> action)
        {
            var configuration = new EndPointsConfigurer<MongoDbEndPointConfiguration>();
            action(configuration);
            IModule endPointModule = new MongoDbServiceBusModule(configuration);
            configurator.RegisterModule(endPointModule);
            return configurator;
        }
        public static Configurator<MongoDbEndPoint> ConfigureSubscriptionService(this Configurator<MongoDbEndPoint> configurator, Action<MsmqSubscriptionServiceConfiguration> action)
        {
            return configurator;
        }

    }

    
}
