using System.Reflection;

namespace TFA.Utils;

public static class PropertyUtils
{
    /// <summary>
    /// Determines if a property is marked as init-only.
    /// </summary>
    /// <returns>True if the property is init only. Else false.</returns>
    public static bool IsInitOnly(this PropertyInfo property)
    {
        if (!property.CanWrite)
            return false;

        return property.SetMethod?.ReturnParameter.GetRequiredCustomModifiers()
            .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)) ?? false;
    }
}
