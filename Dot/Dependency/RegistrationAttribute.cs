using System;

namespace Dot.Dependency
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegistrationAttribute : Attribute
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public LifeCycle LifeCycle { get; set; }
        public RegisterMode RegisterMode { get; set; }
    }
}