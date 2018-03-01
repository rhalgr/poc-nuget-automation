using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NuGet;

namespace TestContentConsole
{
    public interface IPackageManagement
    {
        string PackagePath { get; }
        ICollection<string> PackageList { get; }

        void UpdatePackages();
        void PackageManager_PackageInstalled(object sender, PackageOperationEventArgs e);

        Type LoadTypeFromAppDomain(string className);

        Assembly ResolveAssembly(object sender, ResolveEventArgs args);
    }
}
