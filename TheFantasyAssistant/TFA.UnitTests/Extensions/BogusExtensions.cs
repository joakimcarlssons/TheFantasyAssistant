using Bogus;
using System.Runtime.Serialization;

namespace TFA.UnitTests.Extensions;

public static class BogusExtensions
{
    public static Faker<T> IsRecord<T>(this Faker<T> faker) where T : class
    {
        faker.CustomInstantiator(_ => (FormatterServices.GetSafeUninitializedObject(typeof(T)) as T)!);
        return faker;
    }
}
