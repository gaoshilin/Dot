using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Dot.Dependency;

namespace Dot.Dependency.Engine
{
    public interface IEngine : IContainer
    {
        void RegisterType(Type type, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "");

        void RegisterType<T>(LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "");

        void RegisterInstance<T>(T instance, RegisterMode mode = RegisterMode.Self, string name = "") where T : class;

        void Register<T>(Func<T> creator, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "") where T : class;

        void Register<T>(Func<IComponentContext, IEnumerable<Parameter>, T> creator) where T : class;
    }
}