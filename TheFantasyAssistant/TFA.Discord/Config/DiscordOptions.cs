using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TFA.Discord.Config;

public sealed class DiscordOptions
{
    public const string Key = "Discord";

    [Required]
    public string Token { get; init; } = string.Empty;

    [Required]
    public IReadOnlyCollection<DiscordChannelOption> Channels { get; init; } = [];

    public ulong GetChannelId([ConstantExpected]string channelName)
    {
        if (Channels.FirstOrDefault(channel => channel.Name == channelName) is DiscordChannelOption channel)
        {
            if (ulong.TryParse(channel.Id, out ulong result))
            {
                return result;
            }
            else
            {
                throw new DiscordChannelIdParsingException(channelName);
            }
        }
        else
        {
            throw new DiscordChannelNotFoundException(channelName);
        }
    }
}

public sealed class DiscordChannelOption
{
    [Required]
    public string Id { get; init; } = string.Empty;

    [Required]
    public string Name { get; init; } = string.Empty;
}

public sealed class DiscordChannels
{
    public const string Dev = "dev";
    public const string FPLPriceChanges = "fpl-price-changes";
    public const string FPLUpdates = "fpl-updates";
    public const string AllsvenskanPriceChanges = "fantasy-allsvenskan-price-changes";
    public const string AllsvenskanUpdates = "fantasy-allsvenskan-updates";
}
