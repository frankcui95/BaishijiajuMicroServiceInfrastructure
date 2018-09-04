﻿using Abp.AspNetCore;
using Abp.Dependency;
using Abp.Threading;
using Castle.MicroKernel.Registration;
using Consul;
using Core.Abstractions;
using Core.ServiceDiscovery;
using Core.Wrappers;
using System;

namespace Abp.Modules
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class ConsulModule : AbpModule
    {
        private IServiceDiscoveryHelper consulHelper;

        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Component
                // .For<IServiceDiscoveryHelper, ServiceDiscoveryHelper>()
                // .ImplementedBy<ServiceDiscoveryHelper>()
                  .For<IServiceDiscoveryHelper>()
                  .ImplementedBy<NullServiceDiscoveryHelper>()
                .LifestyleSingleton(),
                Component
                .For<Action<ConsulClientConfiguration>>()
                .UsingFactoryMethod(krnl =>
                {
                    var consulConfiguration = krnl.Resolve<ServiceDiscoveryConfiguration>();
                    Action<ConsulClientConfiguration> action = config => config.Address = new Uri(consulConfiguration.Address);
                    return action;
                }),
                Component
                .For<IConsulClient, ConsulClient>()
                .ImplementedBy<ConsulClient>()
                .LifestyleSingleton(),
                Component
                .For<IKVEndpoint,KV>()
                .ImplementedBy<KV>()
                .LifestyleSingleton()
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ConsulModule).Assembly);
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IHealthCheckHelper, AspNetHealthCheckHelper>(DependencyLifeStyle.Singleton);
            consulHelper = IocManager.Resolve<IServiceDiscoveryHelper>();
            AsyncHelper.RunSync(async () => await consulHelper.RegisterAsync());
        }

        public override void Shutdown()
        {
            if (consulHelper != null)
            {
                AsyncHelper.RunSync(async () => await consulHelper.DeregisterAsync());
            }

        }
    }
}
