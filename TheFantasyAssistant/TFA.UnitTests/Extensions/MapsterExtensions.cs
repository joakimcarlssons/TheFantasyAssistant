using Mapster;
using MapsterMapper;

namespace TFA.UnitTests.Extensions
{
    internal static class MapsterHelpers
    {
        public static Mapper GetMapper()
        {
            TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Infrastructure.AssemblyReference.Assembly);
            return new Mapper(config);
        }
    }
}
