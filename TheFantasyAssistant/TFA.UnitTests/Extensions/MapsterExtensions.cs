using Mapster;
using MapsterMapper;

namespace TFA.UnitTests.Extensions
{
    internal static class MapsterHelpers
    {
        public static Mapper GetMapper()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Infrastructure.AssemblyReference.Assembly);
            return new Mapper(config);
        }
    }
}
