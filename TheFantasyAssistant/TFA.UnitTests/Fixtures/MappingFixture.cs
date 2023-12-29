using MapsterMapper;

namespace TFA.UnitTests.Fixtures;

internal class MappingFixture : IDisposable
{
    public MappingFixture()
    {
        MapsterHelpers.GetMapper();
    }

    public void Dispose()
    {
    }
}
