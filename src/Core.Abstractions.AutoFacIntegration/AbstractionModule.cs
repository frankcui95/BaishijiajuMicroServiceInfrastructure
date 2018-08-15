﻿using Autofac;
using Autofac.Core;
using Core.Messages;
using Core.Messages.Bus;
using Core.PersistentStore.Repositories;
using Core.ServiceDiscovery;
using Core.Session;
using Core.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;

namespace Core.Abstractions
{
    public sealed class AbstractionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterIfNot<IMessagePublisher, MessagePublisher>(ServiceLifetime.Transient)
                   .RegisterIfNot<IMessageDescriptorResolver, MessageDescriptorResolver>(ServiceLifetime.Transient)
                   .RegisterIfNot<IMessageBus, MessageBus>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IMessageConverter, DefaultMessageConverter>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IServiceHelper, DefaultServiceHelper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<HttpMessageHandler, HttpClientHandler>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IHttpClientWrapper, HttpClientWrapper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<ICoreSession, NullCoreSession>(ServiceLifetime.Singleton);

            builder.Register(c => c.Resolve<IHttpClientWrapper>().HttpClient)
                .As<HttpClient>()
                .IfNotRegistered(typeof(HttpClient))
                .SingleInstance()
                .ExternallyOwned();

            //Consul
            builder.RegisterIfNot<ServiceDiscoveryConfiguration>();

            //Repository
            builder.RegisterGeneric(typeof(NullAsyncRepository<,>))
               .As(typeof(IRepository<,>))
               .IfNotRegistered(typeof(IRepository<,>))
               .SingleInstance();

            builder.RegisterGeneric(typeof(NullAsyncRepository<,>))
               .As(typeof(IAsyncRepository<,>))
               .IfNotRegistered(typeof(IAsyncRepository<,>))
               .SingleInstance();
        }
    }

}
