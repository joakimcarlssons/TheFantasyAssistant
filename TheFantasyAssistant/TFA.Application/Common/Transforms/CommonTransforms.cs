
namespace TFA.Application.Common.Transforms;

public static class CommonTransforms
{
    public static string TransformPlayerPosition(this PlayerPosition position)
        => position switch
        {
            PlayerPosition.Goalkeeper => PlayerPositionDisplayNames.Goalkeeper,
            PlayerPosition.Defender => PlayerPositionDisplayNames.Defender,
            PlayerPosition.Midfielder => PlayerPositionDisplayNames.Midfielder,
            PlayerPosition.Attacker => PlayerPositionDisplayNames.Attacker,
            _ => string.Empty
        };

    public static PlayerPosition TransformPlayerPosition(this string? position)
        => position switch
        {
            PlayerPositionDisplayNames.Goalkeeper => PlayerPosition.Goalkeeper,
            PlayerPositionDisplayNames.Defender => PlayerPosition.Defender,
            PlayerPositionDisplayNames.Midfielder => PlayerPosition.Midfielder,
            PlayerPositionDisplayNames.Attacker => PlayerPosition.Attacker,
            _ => throw new NotSupportedException()
        };
}
