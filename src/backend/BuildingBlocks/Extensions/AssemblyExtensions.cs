using System.Reflection;

namespace Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetTypes(this Assembly[] assemblies)
    {
        return assemblies.SelectMany(assembly => assembly.GetTypes());
    }
}