using Microsoft.Extensions.Options;
using System.Text;
using TFA.Twitter.Config;
using TFA.Twitter.Models;
using Tweetinvi;
using Tweetinvi.Models;
using System.Text.Json;


namespace TFA.Twitter;

public interface ITwitterService
{
    /// <summary>
    /// Send a tweet with a given content.
    /// </summary>
    /// <param name="content">The content of the tweet.</param>
    /// <returns>The sent out tweet.</returns>
    Task<ITweet?> TweetAsync(string content);

    /// <summary>
    /// Send a tweet as a reply to another tweet with a given content.
    /// </summary>
    /// <param name="content">The content of the tweet.</param>
    /// <param name="replyTweet">The tweet to reply to.</param>
    /// <returns>The sent out tweet.</returns>
    Task<ITweet?> TweetAsync(string content, ITweet replyTweet);

    /// <summary>
    /// Tweets a thread of a given collection of tweets.
    /// </summary>
    /// <param name="tweets">The tweets to send. Will be sent in the order they are placed in the list.</param>
    /// <param name="replyTweet">Not null if the thread should start from an already existing tweet.</param>
    /// <returns>The last sent tweet in the thread.</returns>
    Task<ITweet?> TweetThreadAsync(IReadOnlyList<string> tweets, ITweet? replyTweet = null);

}

public abstract class AbstractTwitterService(
    IOptions<TwitterOptions> options,
    bool isDevelopment) : ITwitterService
{
    private readonly TwitterOptions twitterOptions = options.Value;

    /// <inheritdoc />
    public Task<ITweet?> TweetAsync(string content)
    {
        if (content.IsValidLength())
        {
            return SendTweetAsync(new(content));
        }

        // In case the tweet is too long it will be sent as a thread.
        return TweetThreadAsync(SplitTooLongTweets([content]));
    }

    /// <inheritdoc />
    public Task<ITweet?> TweetAsync(string content, ITweet replyTweet)
    {
        if (content.IsValidLength())
        {
            return SendTweetAsync(new(content, new ReplyTweet(replyTweet.IdStr)));
        }

        return TweetThreadAsync(SplitTooLongTweets([content]), replyTweet);
    }

    /// <inheritdoc />
    public async Task<ITweet?> TweetThreadAsync(IReadOnlyList<string> tweets, ITweet? replyTweet = null)
    {
        if (tweets.Count == 0)
        {
            return null;
        }

        ITweet? previousTweet = replyTweet ?? await TweetAsync(tweets[0]);
        foreach (string tweet in tweets.Skip(1))
        {
            if (previousTweet is not null)
            {
                previousTweet = await TweetAsync(tweet, previousTweet);
            }
        }

        return previousTweet;
    }

    /// <summary>
    /// Sends a tweet using the V2 of Twitter Api.
    /// Source https://github.com/linvi/tweetinvi/issues/1147#issuecomment-1173174302 also includes solution for handling media.
    /// </summary>
    protected async Task<ITweet?> SendTweetAsync(TweetRequestParams tweetParams)
    {
        if (!twitterOptions.SendInDev && isDevelopment)
        {
            Console.WriteLine(tweetParams.Text);
            return null;
        }

        TwitterClient client = new(twitterOptions.ConsumerKey, twitterOptions.ConsumerSecret, twitterOptions.AccessToken, twitterOptions.TokenSecret);

        return new Tweet(await client.Execute.AdvanceRequestAsync(request =>
        {
            string? body = JsonSerializer.Serialize(tweetParams);
            StringContent content = new(body, Encoding.UTF8, "application/json");

            request.Query.Url = twitterOptions.Url;
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
            request.Query.HttpContent = content;
        }));
    }

    protected static List<string> SplitTooLongTweets(List<string> tweets, int currentHandledTweetIndex = 0)
    {
        // Verify that there are tweets to handle
        if (currentHandledTweetIndex >= tweets.Count)
            return tweets;

        string tweetToSplit = tweets[currentHandledTweetIndex];
        if (tweetToSplit.IsValidLength())
        {
            // If the tweet is valid, move on to the next
            return SplitTooLongTweets(tweets, ++currentHandledTweetIndex);
        }

        string[] splitTweet = tweetToSplit.Split('\n');
        if (splitTweet.Length <= 1)
        {
            // If the tweet is not splittable,
            // Return an empty list to avoid tweeting.
            return [];
        }

        // Move the last line of the tweet...
        if (currentHandledTweetIndex + 1 < tweets.Count)
        {
            // ...to the next tweet if there is one
            tweets[currentHandledTweetIndex + 1] += $"\n{splitTweet.Last()}";
        }
        else
        {
            // ...to a new tweet
            tweets.Add(splitTweet.Last());
        }

        // Rebuild the current tweet without its last line
        tweets[currentHandledTweetIndex] = string.Join("\n", splitTweet.Take(splitTweet.Length - 1));

        // Verify that the split tweet is now valid
        return SplitTooLongTweets(tweets, currentHandledTweetIndex);
    }
}