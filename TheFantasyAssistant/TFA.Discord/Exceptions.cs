namespace TFA.Discord;

public class DiscordChannelNotFoundException(string channelName) : Exception(message: $"Could not find configuration for discord channel with name {channelName}.");
public class DiscordChannelIdParsingException(string channelName): Exception(message: $"Failed to parse id for discord channel with name {channelName}");
