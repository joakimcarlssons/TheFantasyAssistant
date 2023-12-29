using FluentAssertions;
using TFA.Utils;

namespace TFA.UnitTests.Features.Utils;

public class EnumUtilsTests
{
    [Fact]
    public void GetConstantValue_OnMatchingPropertyAndEnum_ReturnsExpectedValue()
    {
        string value = TestEnum.Value.GetConstantValue<string, TestEnumClass>();
        value.Should().Be("value");
    }

    [Fact]
    public void GetConstantValue_WhenPropertyIsNotConstant_ThrowsException()
        => Assert.Throws<InvalidOperationException>(() => TestEnum.NonConstantValue.GetConstantValue<string, TestEnumClass>());

    [Fact]
    public void GetConstantValue_WhenNoPropertyMatchesEnum_ThrowsException()
        => Assert.Throws<InvalidOperationException>(() => TestEnum.NonMatchingPropertyName.GetConstantValue<string, TestEnumClass>());

    [Fact]
    public void GetConstantValue_WhenProvidedValueTypeDoesNotMatchPropertyValueType_ThrowsException()
        => Assert.Throws<InvalidOperationException>(() => TestEnum.NonMatchingValueType.GetConstantValue<string, TestEnumClass>());
}

public enum TestEnum
{
    Value,
    NonConstantValue,
    NonMatchingPropertyName,
    NonMatchingValueType,
}

public sealed class TestEnumClass
{
    public const string Value = "value";
    public string NonConstantValue { get; set; } = string.Empty;
    public const int NonMatchingValueType = 1;
}
