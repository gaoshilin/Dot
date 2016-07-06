using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using Dot.Configuration;
using Dot.Dependency;
using Dot.Extension;
using Dot.Util;

namespace Dot.Dependency.Engine
{
    public abstract class EngineBase : IEngine
    {
        protected IContainer _container;

        public EngineBase() : this(ConfigurationManager.GetSection("dotConfig") as DotConfig)
        {
        }

        public EngineBase(DotConfig config)
        {
            Ensure.NotNull<DotConfig>(config, "config");

            var builder = new ContainerBuilder();
            this.CustomRegister(builder, config);       // 用户自定义注册，扩展点
            this.RegistrationRegister(builder, config); // 自动注册 RegistrationAttribute
            _container = builder.Build();
        }

        protected abstract void CustomRegister(ContainerBuilder builder, DotConfig config);

        protected void RegistrationRegister(ContainerBuilder builder, DotConfig config)
        {            
            var assemblies = this.GetAssemblies(config);
            var registrations = Registration.Scan(assemblies).ToArray();
            this.DoRegistrationRegister(builder, registrations);
        }

        protected virtual IEnumerable<Assembly> GetAssemblies(DotConfig config)
        {
            try
            {
                var skipPattern = config.AssemblySkipPattern;
                var restrictPattern = config.AssemblyRestrictPattern;
                var isWebApplication = config.IsWebApplication;
                var assemblies = AssemblyUtil.GetAssemblies(isWebApplication);

                if (!string.IsNullOrEmpty(skipPattern) && !string.IsNullOrEmpty(restrictPattern))
                    return assemblies.Where(t => t.FullName.IsNotMatch(skipPattern) && t.FullName.IsMatch(restrictPattern));

                if (!string.IsNullOrEmpty(skipPattern))
                    return assemblies.Where(t => t.FullName.IsNotMatch(skipPattern));

                if (!string.IsNullOrEmpty(restrictPattern))
                    return assemblies.Where(t => t.FullName.IsMatch(restrictPattern));

                return assemblies;
            }
            catch
            {
                return Enumerable.Empty<Assembly>();
            }
        }

        protected virtual void DoRegistrationRegister(ContainerBuilder builder, Registration[] registrations)
        {
            builder.Register(registrations);
        }

        #region IEngine members

        public void RegisterType(Type type, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "")
        {
            var builder = new ContainerBuilder();
            builder.RegisterType(type, mode, name).UseLifeCycle(lifeCycle);
            builder.Update(_container);
        }

        public void RegisterType<T>(LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "")
        {
            this.RegisterType(typeof(T), lifeCycle, mode, name);
        }

        public void RegisterInstance<T>(T instance, RegisterMode mode = RegisterMode.Self, string name = "") where T : class
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance<T>(instance, mode, name);
            builder.Update(_container);
        }

        public void Register<T>(Func<T> creator, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "") where T : class
        {
            var builder = new ContainerBuilder();
            builder.Register<T>(creator, mode, name).UseLifeCycle(lifeCycle);
            builder.Update(_container);
        }

        public void Register<T>(Func<IComponentContext, IEnumerable<Parameter>, T> creator) where T : class
        {
            var builder = new ContainerBuilder();
            builder.Register<T>(creator);
            builder.Update(_container);
        }

        #endregion

        #region IContainer members

        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return _container.BeginLifetimeScope(tag, configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return _container.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return _container.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }

        public IDisposer Disposer
        {
            get { return _container.Disposer; }
        }

        public object Tag
        {
            get { return _container.Tag; }
        }

        public IComponentRegistry ComponentRegistry
        {
            get { return _container.ComponentRegistry; }
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return _container.ResolveComponent(registration, parameters);
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        #endregion
    }
}