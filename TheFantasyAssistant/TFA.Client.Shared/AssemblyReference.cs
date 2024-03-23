using System.Reflection;

namespace TFA.Client.Shared;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
