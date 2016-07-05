using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dot.Dependency
{
    public class Registration
    {
        public Type ServiceType { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public LifeCycle LifeCycle { get; set; }
        public RegisterMode RegisterMode { get; set; }

        public Registration(Type serviceType, RegistrationAttribute attr = null)
        {
            this.ServiceType = serviceType;

            if (attr != null)
            {
                this.Order = attr.Order;
                this.Name = attr.Name;
                this.LifeCycle = attr.LifeCycle;
                this.RegisterMode = attr.RegisterMode;
            }
        }

        public static List<Registration> Scan(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                             .ToDictionary(type => type, type => type.GetCustomAttribute<RegistrationAttribute>())
                             .Where(kv => kv.Value != null)
                             .Select(kv => new Registration(kv.Key, kv.Value))
                             .OrderBy(reg => reg.Order)
                             .ToList();
        }
    }
}