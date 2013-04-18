using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Lifetime;
using Autofac.Builder;
using Yasb.Common.Messaging;
using Yasb.Common;

namespace Yasb.Wireup
{
    public class ServiceBusModule<TConfiguration, TConnectionConfiguration> : CommonModule<TConfiguration>
        where TConfiguration : ServiceBusConfiguration<TConnectionConfiguration>
    {
        public ServiceBusModule(TConfiguration configuration):base(configuration,"bus")
        {
        }

        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterGeneratedFactory<MessageHandlerFactory>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<IHandleMessages>((componentScope, p) =>
            {
                var type = p.Named<Type>("type");
                var genericType = typeof(IHandleMessages<>).MakeGenericType(type);
                return componentScope.Resolve(genericType) as IHandleMessages;
            }).InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<ITaskRunner>(componentScope => new TaskRunner()).As<ITaskRunner>().InstancePerMatchingLifetimeScope(Scope);


            builder.RegisterWithScope<IHandleMessages<SubscriptionMessage>>(componentScope => new SubscriptionMessageHandler(componentScope.Resolve<ISubscriptionService>()))
                .As<IHandleMessages<SubscriptionMessage>>().InstancePerMatchingLifetimeScope(Scope);

            builder.RegisterWithScope<IWorker>((componentScope, parameters) =>
            {
                var queueFactory = componentScope.Resolve<QueueFactory>();
                var localQueue = queueFactory(Configuration.EndPointConfiguration.LocalEndPoint);
                return new MessagesReceiver(localQueue, componentScope.Resolve<MessageHandlerFactory>());
            }).InstancePerMatchingLifetimeScope(Scope);

            if (Configuration.MessageHandlersAssembly != null)
            {
                builder.RegisterAssemblyTypes(Configuration.MessageHandlersAssembly)
                      .AsClosedTypesOf(typeof(IHandleMessages<>))
                      .AsImplementedInterfaces();
            }
            builder.RegisterWithScope<IServiceBus>((componentScope, parameters) =>
            {
                return new ServiceBus(Configuration.EndPointConfiguration.NamedEndPoints, componentScope.Resolve<IWorker>(), componentScope.Resolve<QueueFactory>(), componentScope.Resolve<ISubscriptionService>(), componentScope.Resolve<ITaskRunner>());
            }).InstancePerMatchingLifetimeScope(Scope);
        }
    }
}
