using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Dependency;

namespace Dot.Test.Support
{
    public interface ILanguage : IDisposable
    {
    }

    [Registration(Name = "csharp", RegisterMode = RegisterMode.Self | RegisterMode.DefaultInterface, LifeCycle = LifeCycle.Singelton)]
    public class Csharp : ILanguage 
    {
        public void Dispose()
        {
            Console.WriteLine("Csharp entity disposed.");
        }
    }

    [Registration(Name = "java", RegisterMode = RegisterMode.Self | RegisterMode.Interface, LifeCycle = LifeCycle.Transient)]
    public class Java : ILanguage 
    {
        public void Dispose()
        {
            Console.WriteLine("Java entity disposed.");
        }
    }

    [Registration(Name = "php", RegisterMode = RegisterMode.Self | RegisterMode.Interface, LifeCycle = LifeCycle.Scope)]
    public class Php : ILanguage 
    {
        public void Dispose()
        {
            Console.WriteLine("Php entity disposed.");
        }
    }

    [Registration(RegisterMode = RegisterMode.Self)]
    public class Ruby : ILanguage 
    {
        public void Dispose()
        {
            Console.WriteLine("Ruby entity disposed.");
        }
    }
}