using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Yasb.Common.Tests.Configuration;
using Yasb.Wireup;
using Autofac;
using Yasb.Common.Messaging;
using Moq;
using Yasb.Redis.Messaging.Configuration;
using Autofac.Core;

namespace Yasb.Tests.Wireup
{


    public static class TestConfiguratorExtensions
    {

        public static Configurator<TestEndPoint> ConfigureEndPoints(this Configurator<TestEndPoint> configurator, Action<EndPointsConfigurer<TestEndPointConfiguration>> action)
        {
            var configuration = new EndPointsConfigurer<TestEndPointConfiguration>();
            action(configuration);
            
            return configurator;
        }
        

    }
   
}
