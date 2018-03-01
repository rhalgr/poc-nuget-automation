using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestContentConsole;

namespace TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PackageManagement packageManagement = new PackageManagement();
            
            Console.WriteLine("Preparing to update packages");
            packageManagement.UpdatePackages();
            Console.WriteLine("Package update complete");
            Console.ReadKey();
        }
    }
}
