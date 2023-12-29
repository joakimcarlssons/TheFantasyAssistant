using System.Text.Json;
using System.Text.Json.Serialization;
using TFA.Twitter.Exceptions;
using Tweetinvi.Core.Web;

namespace TFA.Twitter.Models;

public sealed class Tweet : AbstractTweet
{
    public Tweet(string id)
    {
        IdStr = id;
    }

    public Tweet(ITwitterResult twitterResult)
    {
        if (twitterResult.Response.IsSuccessStatusCode
            && JsonSerializer.Deserialize<TwitterResponse>(twitterResult.Content) is { Content: not null } twitterResponse)
        {
            IdStr = twitterResponse.Content.Id;
        }
        else
        {
            throw new TweetException($"Failed to send tweet. Response was: {twitterResult.Response.Content}");
        }

    }
}

/// <summary>
/// The response model from a tweet.
/// </summary>
public sealed record TwitterResponse(
    [property: JsonPropertyName("data")] TweetContent Content);

/// <summary>
/// The content of a <see cref="TwitterResponse" />.
/// </summary>
/// <param name="Id">The id of the tweet.</param>
/// <param name="Text">The content of the tweet.</param>
public sealed record TweetContent(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("text")] string Text);


public sealed record TweetRequestParams(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("reply")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] ReplyTweet? ReplyTweet = null);

public sealed record ReplyTweet(
    [property: JsonPropertyName("in_reply_to_tweet_id")] string Id);