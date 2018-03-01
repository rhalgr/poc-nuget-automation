using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NuGet;
using TestContentConsole;

namespace TestConsole
{
    /// <summary>
    /// Manage package updates and dynamic library loading
    /// </summary>
    /// <remarks>
    /// Based on help found here:
    /// https://stackoverflow.com/questions/31859267/load-nuget-dependencies-at-runtime
    /// and here:
    /// https://stackoverflow.com/questions/15232228/are-there-reference-implementations-of-hot-swapping-in-net
    /// </remarks>
    public class PackageManagement : IPackageManagement
    {
        public string PackagePath { get; }
        public ICollection<string> PackageList { get; }

        public PackageManagement()
        {
            PackagePath = Path.Combine(Environment.CurrentDirectory, "Packages");
            PackageList = new List<string> { "TestAddLibrary" };
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        public void UpdatePackages()
        {
            var repo = PackageRepositoryFactory.Default
                .CreateRepository("http://nuget.mike.at.local/nuget");
            
            if (!Directory.Exists(PackagePath))
            {
                Directory.CreateDirectory(PackagePath);
            }
            var packageManager = new PackageManager(repo, PackagePath);
            packageManager.PackageInstalled += PackageManager_PackageInstalled;

            foreach (var packageName in PackageList)
            {
                /*
                 * If we wanted a specific version, you can pass in the the parsed SemanticVersion:
                 *
                 * repo.FindPackage(packageName, SemanticVersion.Parse("1.0.0"));
                 */
                var package = repo.FindPackage(packageName);
                if (package != null)
                {
                    Console.WriteLine($"Found nuget package for {package.Id} version {package.Version.Version}");
                    packageManager.InstallPackage(package, false, true);
                }
            }

            //TODO: Remove hardcoded names and do the loading from a collection
            LoadTypeFromAppDomain("TestAddLibrary.AddMethods");
        }

        public void PackageManager_PackageInstalled(object sender, PackageOperationEventArgs e)
        {
            var files = e.FileSystem.GetFiles(e.InstallPath, "*.dll", true);
            foreach (var file in files)
            {
                Console.WriteLine($"File {file} installed at {e.InstallPath}");
                
                var fileParts = file.Split('\\');
                var fileName = fileParts.Last();
                var rootPath = e.FileSystem.Root;
                var sourceFilePath = Path.Combine(rootPath, file);
                var destinationFilePath = Path.Combine(rootPath, fileName);

                if (File.Exists(destinationFilePath))
                    File.Delete(destinationFilePath);

                File.Move(sourceFilePath, destinationFilePath);
                
                Console.WriteLine($"File {sourceFilePath} moved to {destinationFilePath}");
            }

            // Cleanup/remove the package. This ensures that our PackageInstalled handler
            // will always get called and that we're always pulling
            // (this may not be desired all the time...just for proof of concept)
            Directory.Delete(e.InstallPath, true);
        }

        public Type LoadTypeFromAppDomain(string className)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .Select(assembly => assembly.GetType(className))
                .Where(t => t != null)
                .ToList();
            return types.Count == 1 ? types.First() : null;
        }

        public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            Console.WriteLine($"Attempting to resolve assembly for {args.Name}");
            var assemblyPath = Path.Combine(PackagePath, new AssemblyName(args.Name).Name + ".dll");
            return !File.Exists(assemblyPath) ? null : Assembly.Load(assemblyPath);
        }
    }
}
