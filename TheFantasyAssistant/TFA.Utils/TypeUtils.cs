namespace TFA.Utils;

public static class TypeUtils
{
    public static bool IsType<TValue, TExpected>() => typeof(TValue) == typeof(TExpected);

    /// <summary>
    /// Check if any of the constructors defined on <paramref name="type"/> takes in a parameter of type <paramref name="constructorType"/>
    /// </summary>
    public static bool TakesInTypeInConstructor(this Type type, Type constructorType)
        => type.GetConstructors().Any(ctr => 
            ctr.GetParameters().Any(param => constructorType.IsAssignableFrom(param.ParameterType)));
}
