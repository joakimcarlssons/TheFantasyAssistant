using TFA.Utils;

namespace TFA.UnitTests.Features.Utils;

public class EnumerableUtilsTests
{
    [Fact]
    public void SplitEnumerable_CanSplitEnumerableWithEvenNumbers()
    {
        IReadOnlyList<int> list = [1, 2, 3, 4];
        list.SplitEnumerable(2).Should().NotBeEmpty().And.HaveCount(2);
    }

    [Fact]
    public void SplitEnumerable_CanSplitEnumerableWithUnevenNumbers()
    {
        IReadOnlyList<int> list = [1, 2, 3];
        list.SplitEnumerable(2).Should().NotBeEmpty().And.HaveCount(2);
    }
}
