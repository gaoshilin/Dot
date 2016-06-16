using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace Dot.Util
{
    public static class AssemblyUtil
    {
        public static IEnumerable<Assembly> GetAssemblies(bool isWebApplication)
        {
            return isWebApplication
                 ? BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                 : AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> GetBinFolderAssemblies(bool isWebApplication)
        {
            var binFolder = isWebApplication
                          ? HttpRuntime.AppDomainAppPath + "bin\\"
                          : AppDomain.CurrentDomain.BaseDirectory;

            var dllFiles = Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList();
            var assemblies = dllFiles.Select(dllFile => Assembly.LoadFile(dllFile)).ToList();
            return assemblies;
        }
    }
}