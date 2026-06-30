using System.Collections.Generic;
using System.Reflection;

namespace BIL_Interpreter.Helpers;

public static class AssemblyVault
{
    internal static readonly Dictionary<string, Assembly> Assemblies = new()
    {
        {
            "System",
            typeof(object).Assembly
        }
    };

    /// <summary>
    /// Register a new assembly inside the <see cref="AssemblyVault"/> so it can be used for method invokes
    /// </summary>
    /// <param name="name"></param>
    /// <param name="assembly"></param>
    public static void Add(string name, Assembly assembly)
    {
        Assemblies[name] = assembly;
    }
    
    /// <summary>
    /// Register a new assembly inside the <see cref="AssemblyVault"/> so it can be used for method invokes
    /// </summary>
    /// <param name="assembly"></param>
    public static void Add(Assembly assembly) => Add(assembly.GetName().Name, assembly);
}