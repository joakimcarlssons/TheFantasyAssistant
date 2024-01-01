using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using TFA.Discord.Config;

namespace TFA.Discord;

public interface IDiscordService
{
    Task SendMessageAsync(string message, string channelName);
    Task SendMessageAsync(DiscordEmbedBuilder embed, string channelName);
    Task SendMessageAsync(DiscordMessageBuilder content, string channelName);
}

public class DiscordService : IDiscordService
{
    private readonly DiscordClient _client;
    private readonly DiscordOptions _options;
    private readonly IHostEnvironment _env;

    public DiscordService(
        DiscordClient client, 
        IOptions<DiscordOptions> options,
        IHostEnvironment env)
    {
        _client = client;
        _options = options.Value;
        _env = env;

        // Connect the client as soon as possible
        Task.Run(() =>
        {
            if (_options.ConnectClient.Equals("True", StringComparison.InvariantCultureIgnoreCase))
            {
                _client.ConnectAsync();
            }
        });
    }

    public async Task SendMessageAsync(string message, [ConstantExpected] string channelName)
    {
        DiscordChannel channel = await _client.GetChannelAsync(GetChannelId(channelName));
        await channel.SendMessageAsync(message);
    }

    public async Task SendMessageAsync(DiscordEmbedBuilder embed, [ConstantExpected] string channelName)
    {
        DiscordChannel channel = await _client.GetChannelAsync(GetChannelId(channelName));
        await channel.SendMessageAsync(embed);
    }

    public async Task SendMessageAsync(DiscordMessageBuilder content, [ConstantExpected] string channelName)
    {
        DiscordChannel channel = await _client.GetChannelAsync(GetChannelId(channelName));
        await channel.SendMessageAsync(content);
    }

    /// <summary>
    /// Wrapper of the original <see cref="DiscordOptions.GetChannelId(string)"/> to always get the Dev channel when in development.
    /// </summary>
    /// <param name="channelName">The name of the channel to get the id for. Should be part of <see cref="DiscordChannels"/></param>
    private ulong GetChannelId([ConstantExpected] string channelName)
        => _env.IsDevelopment()
            ? _options.GetChannelId(DiscordChannels.Dev)
            : _options.GetChannelId(channelName);
}
