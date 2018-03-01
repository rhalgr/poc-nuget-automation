using System;
using System.Reflection;

namespace TestConsole
{
    public class TypeProxy : MarshalByRefObject
    {
        public Type LoadFromAssembly(string assemblyPath, string typeName)
        {
            try
            {
                var asm = Assembly.Load(assemblyPath);
                return asm.GetType(typeName);
            }
            catch (Exception) { return null; }
        }
    }
}
