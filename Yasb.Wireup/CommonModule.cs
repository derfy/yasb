using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Common;
using Yasb.Common.Serialization;
using Newtonsoft.Json;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Wireup
{
    public class CommonModule: Autofac.Module
    {
        private IServiceBusConfiguration _serviceBusConfiguration;
        public CommonModule(IServiceBusConfiguration serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
        }
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            builder.RegisterGeneratedFactory<QueueFactory>().InstancePerMatchingLifetimeScope("bus");
            builder.RegisterGeneratedFactory<MessageHandlerFactory>().InstancePerMatchingLifetimeScope("bus");

            builder.RegisterWithScope<IWorker>((componentScope, parameters) =>
            {
                var queueFactory = componentScope.Resolve<QueueFactory>();
                var localQueue = queueFactory(_serviceBusConfiguration.EndPointConfiguration.LocalEndPoint);
                return new MessagesReceiver(localQueue, componentScope.Resolve<MessageHandlerFactory>());
            }).As(typeof(IWorker)).InstancePerMatchingLifetimeScope("bus");



            builder.RegisterWithScope<ISerializer>((componentScope, parameters) =>
            {
                return new Serializer(new JsonConverter[] { new EndPointConverter(), new MessageEnvelopeConverter() });
            }).InstancePerMatchingLifetimeScope("bus");
            builder.RegisterWithScope<IHandleMessages>((componentScope, p) =>
            {
                var type = p.Named<Type>("type");
                var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                return componentScope.Resolve(genericType) as IHandleMessages;
            }).InstancePerMatchingLifetimeScope("bus");

            builder.RegisterWithScope<ITaskRunner>(componentScope => new TaskRunner()).As<ITaskRunner>().InstancePerMatchingLifetimeScope("bus");
            builder.RegisterWithScope<IServiceBus>((componentScope, parameters) =>
            {
                return new ServiceBus(_serviceBusConfiguration.EndPointConfiguration.NamedEndPoints, componentScope.Resolve<IWorker>(), componentScope.Resolve<QueueFactory>(), componentScope.Resolve<ISubscriptionService>(), componentScope.Resolve<ITaskRunner>());
            }).As<IServiceBus>().InstancePerMatchingLifetimeScope("bus");
            
            builder.RegisterWithScope<IHandleMessages<SubscriptionMessage>>(componentScope => new SubscriptionMessageHandler(componentScope.Resolve<ISubscriptionService>())).As<IHandleMessages<SubscriptionMessage>>().InstancePerMatchingLifetimeScope("bus");

            if (_serviceBusConfiguration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(_serviceBusConfiguration.MessageHandlersAssembly)

                      .AsClosedTypesOf(typeof(IHandleMessages<>))
                      .AsImplementedInterfaces();
            }
        }
    }
}
