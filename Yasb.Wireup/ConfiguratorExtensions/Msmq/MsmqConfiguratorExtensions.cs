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

namespace Yasb.Wireup.ConfiguratorExtensions.Msmq
{


    public static  class MsmqConfiguratorExtensions 
    {

        public static Configurator<MsmqEndPoint> ConfigureEndPoints(this Configurator<MsmqEndPoint> configurator, Action<EndPointsConfigurer<MsmqEndPointConfiguration>> action)
        {
            var configuration = new EndPointsConfigurer<MsmqEndPointConfiguration>();
            action(configuration);
            IModule endPointModule = new MsmqServiceBusModule(configuration);
            configurator.RegisterModule(endPointModule);
            return configurator;
        }
        public static Configurator<MsmqEndPoint> ConfigureSubscriptionService(this Configurator<MsmqEndPoint> configurator, Action<SubscriptionServiceConfiguration> action)
        {
            var configuration = new SubscriptionServiceConfiguration();
            action(configuration);
            IModule module = new RedisSubscriptionServiceModule<MsmqEndPoint>(configuration);
            configurator.RegisterModule(module);
            return configurator;
        }

    }

    
}
