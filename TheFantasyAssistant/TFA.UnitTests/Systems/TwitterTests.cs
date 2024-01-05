using FluentAssertions;
using Microsoft.Extensions.Options;
using TFA.Presentation.Utils;
using TFA.Twitter;
using TFA.Twitter.Config;

namespace TFA.UnitTests.Systems;

/// <summary>
/// Mocked service in order to trigger console outputs.
/// </summary>
internal class MockTwitterService(IOptions<TwitterOptions> options) : AbstractTwitterService(options, true);

public class TwitterTests
{
    private readonly IOptions<TwitterOptions> options = Options.Create(new TwitterOptions
    {
        SendInDev = false
    });

    private MockTwitterService TwitterService => new(options);
    private const int MaxTweetLength = 280;

    [Fact]
    public async Task TweetAsync_SingleLine_TweetIsSent()
    {
        StringWriter output = RegisterOutput();
        await TwitterService.TweetAsync("Test");

        output.ToString()
            .Should()
            .Be(string.Format("Test{0}", Environment.NewLine));
    }

    [Fact]
    public async Task TweetAsync_TooLongTweet_ShouldSplitTweetIntoThread()
    {
        StringWriter output = RegisterOutput();

        string content = string.Concat(new('*', MaxTweetLength), "\n*");
        await TwitterService.TweetAsync(content);

        output.ToString()
            .Count(c => c == '\n')
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task TweetAsync_TooLongTweetWithEmojis_ShouldSplitTweetIntoThread()
    {
        StringWriter output = RegisterOutput();

        string content = string.Concat(new('*', MaxTweetLength - 2), $"\n{Emoji.Warning}{Emoji.Warning}");
        await TwitterService.TweetAsync(content);

        output.ToString()
            .Count(c => c == '\n')
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task TweetAsync_TooLongTweetWithoutLineBreaks_ShouldNotTweet()
    {
        StringWriter output = RegisterOutput();
        await TwitterService.TweetAsync(new string('*', MaxTweetLength + 1));
        output.ToString()
            .Should()
            .Be(string.Empty);
    }

    [Fact]
    public async Task TweetAsync_TooLongTweetWithoutLineBreaksInTime_ShouldNotTweet()
    {
        StringWriter output = RegisterOutput();

        string content = string.Concat(new string('*', MaxTweetLength + 1), "\n*");
        await TwitterService.TweetAsync(content);

        output.ToString()
            .Should()
            .Be(string.Empty);
    }

    /// <summary>
    /// Register an output reference on the console to read results.
    /// </summary>
    private static StringWriter RegisterOutput()
    {
        StringWriter output = new();
        Console.SetOut(output);
        return output;
    }
}
