using System.Reflection;

namespace TFA.Api;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly; 
}
