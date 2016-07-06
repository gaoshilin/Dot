using System;
using Autofac;
using Autofac.Core.Lifetime;
using Dot.Dependency.Engine;
using Dot.Sample.Support;

namespace Dot.Sample
{
    partial class Program
    {
        static void RunEngineSample()
        {
            var engine = new DotEngine();

            // RegisterMode = RegisterMode.Self, LifeCycle = LifeCycle.Singelton
            var java1 = engine.Resolve<Java>();
            var java2 = engine.BeginLifetimeScope().Resolve<Java>();
            var java3 = engine.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag).Resolve<Java>();
            var java4 = engine.BeginLifetimeScope("foo").Resolve<Java>();
            var java5 = engine.BeginLifetimeScope("bar").Resolve<Java>();
            Console.WriteLine("java1 equal than java2 = {0}", object.ReferenceEquals(java1, java2));
            Console.WriteLine("java1 equal than java3 = {0}", object.ReferenceEquals(java1, java3));
            Console.WriteLine("java1 equal than java4 = {0}", object.ReferenceEquals(java1, java4));
            Console.WriteLine("java1 equal than java4 = {0}", object.ReferenceEquals(java1, java5));
            Console.WriteLine("java4 equal than java5 = {0}", object.ReferenceEquals(java4, java5));
            Console.WriteLine("-----------------------");

            // RegisterMode = RegisterMode.Self, LifeCycle = LifeCycle.Scope
            var php1 = engine.Resolve<Php>();
            var scope1 = engine.BeginLifetimeScope();
            var php2 = scope1.Resolve<Php>();
            var php3 = scope1.Resolve<Php>();
            var scope2 = engine.BeginLifetimeScope();
            var php4 = scope2.Resolve<Php>();
            Console.WriteLine("php1 equal than php2 = {0}", object.ReferenceEquals(php1, php2));
            Console.WriteLine("php2 equal than php3 = {0}", object.ReferenceEquals(php2, php3));
            Console.WriteLine("php3 equal than php4 = {0}", object.ReferenceEquals(php3, php4));
            scope1.Dispose(); // reference of php2 and php3 will dispose.
            scope2.Dispose(); // reference of php4 will dispose.
            Console.WriteLine("-----------------------");

            // RegisterMode.Self | RegisterMode.DefaultInterface, LifeCycle = LifeCycle.Transient
            var csharp = engine.Resolve<ILanguage>();
            Console.WriteLine("csharp is type of Csharp = {0}", csharp.GetType() == typeof(Csharp));
            var csharp1 = engine.Resolve<Csharp>();
            var csharp2 = engine.Resolve<Csharp>();
            Console.WriteLine("csharp1 equal than csharp2 = {0}", object.ReferenceEquals(csharp1, csharp2));
            using (var scope = engine.BeginLifetimeScope())
            {
                var csharp3 = scope.Resolve<Csharp>();
                var csharp4 = scope.Resolve<Csharp>();
                Console.WriteLine("csharp3 equal than csharp4 = {0}", object.ReferenceEquals(csharp3, csharp4));
            }
            using (var scope = engine.BeginLifetimeScope("csharp"))
            {
                var csharp5 = scope.Resolve<Csharp>();
                var csharp6 = scope.Resolve<Csharp>();
                Console.WriteLine("csharp5 equal than csharp6 = {0}", object.ReferenceEquals(csharp5, csharp6));
            }
            Console.WriteLine("-----------------------");

            // RegisterMode = RegisterMode.Self | RegisterMode.Interface, LifeCycle = LifeCycle.Transient, Name = "rudy"
            var ruby = engine.ResolveNamed("ruby", typeof(Ruby));
            Console.WriteLine("ruby is typeof {0}", ruby.GetType().FullName);
        }
    }
}