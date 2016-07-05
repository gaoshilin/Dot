using System;
using Autofac;
using Dot.Dependency;

namespace Dot.Engine
{
    public interface IEngine : IContainer
    {
        void RegisterType(Type type, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "");

        void RegisterType<T>(LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "");

        void RegisterInstance<T>(T instance, RegisterMode mode = RegisterMode.Self, string name = "") where T : class;

        void Register<T>(Func<T> creator, LifeCycle lifeCycle = LifeCycle.Transient, RegisterMode mode = RegisterMode.Self, string name = "") where T : class;
    }
}