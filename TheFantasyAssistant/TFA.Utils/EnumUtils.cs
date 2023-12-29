namespace TFA.Utils;

public static class EnumUtils
{
    public static TValue GetConstantValue<TValue, TClass>(this Enum e)
        where TClass : class
    {
        string expectedConstName = e.ToString();
        Type classType = typeof(TClass);

        if (classType.GetField(expectedConstName) is { IsLiteral: true, IsInitOnly: false } constant)
        {
            if (constant.GetRawConstantValue() is TValue value)
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException($"Constant {expectedConstName} on class {classType.Name} is not of expected type {typeof(TValue).Name}.");
            }
        }
        else
        {
            throw new InvalidOperationException($"No constant with name {expectedConstName} found in class {classType.Name}.");
        }
    }
}
