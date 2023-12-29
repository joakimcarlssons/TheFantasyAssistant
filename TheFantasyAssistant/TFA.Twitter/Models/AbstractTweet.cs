using Tweetinvi.Models.DTO;
using Tweetinvi.Models.Entities;
using Tweetinvi.Models;
using Tweetinvi;

namespace TFA.Twitter.Models;

/// <summary>
/// Abstract representation of the <see cref="ITweet"/> interface.
/// </summary>
/// <remarks>
/// Created to above all abstract away unused properties after the Twitter V2 API Upgrade.
/// Unused properties/methods are now marked with <see cref="ObsoleteAttribute"/>.
/// New response type is scaled down to <see cref="Tweet"/>.
/// </remarks>
public abstract class AbstractTweet : ITweet
{
    public ITwitterClient? Client { get; set; }

    /// <summary>
    /// The tweet content
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The primary identitfier for a tweet
    /// </summary>
    public string? IdStr { get; set; }

    [Obsolete]
    public long Id { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    [Obsolete]
    public DateTimeOffset CreatedAt => throw new NotSupportedException();

    [Obsolete]
    public string Prefix => throw new NotSupportedException();

    [Obsolete]
    public string Suffix => throw new NotSupportedException();

    [Obsolete]
    public string FullText => throw new NotSupportedException();

    [Obsolete]
    public int[] DisplayTextRange => throw new NotSupportedException();

    [Obsolete]
    public int[] SafeDisplayTextRange => throw new NotSupportedException();

    [Obsolete]
    public IExtendedTweet ExtendedTweet => throw new NotSupportedException();

    [Obsolete]
    public ICoordinates Coordinates => throw new NotSupportedException();

    [Obsolete]
    public string Source => throw new NotSupportedException();

    [Obsolete]
    public bool Truncated => throw new NotSupportedException();

    [Obsolete]
    public int? ReplyCount => throw new NotSupportedException();

    [Obsolete]
    public long? InReplyToStatusId => throw new NotSupportedException();

    [Obsolete]
    public string InReplyToStatusIdStr => throw new NotSupportedException();

    [Obsolete]
    public long? InReplyToUserId => throw new NotSupportedException();

    [Obsolete]
    public string InReplyToUserIdStr => throw new NotSupportedException();

    [Obsolete]
    public string InReplyToScreenName => throw new NotSupportedException();

    [Obsolete]
    public IUser CreatedBy => throw new NotSupportedException();

    [Obsolete]
    public ITweetIdentifier CurrentUserRetweetIdentifier => throw new NotSupportedException();

    [Obsolete]
    public int[] ContributorsIds => throw new NotSupportedException();

    [Obsolete]
    public IEnumerable<long> Contributors => throw new NotSupportedException();

    [Obsolete]
    public int RetweetCount => throw new NotSupportedException();

    [Obsolete]
    public ITweetEntities Entities => throw new NotSupportedException();

    [Obsolete]
    public bool Favorited => throw new NotSupportedException();

    [Obsolete]
    public int FavoriteCount => throw new NotSupportedException();

    [Obsolete]
    public bool Retweeted => throw new NotSupportedException();

    [Obsolete]
    public bool PossiblySensitive => throw new NotSupportedException();

    [Obsolete]
    public Language? Language => throw new NotSupportedException();

    [Obsolete]
    public IPlace Place => throw new NotSupportedException();

    [Obsolete]
    public Dictionary<string, object> Scopes => throw new NotSupportedException();

    [Obsolete]
    public string FilterLevel => throw new NotSupportedException();

    [Obsolete]
    public bool WithheldCopyright => throw new NotSupportedException();

    [Obsolete]
    public IEnumerable<string> WithheldInCountries => throw new NotSupportedException();

    [Obsolete]
    public string WithheldScope => throw new NotSupportedException();

    [Obsolete]
    public ITweetDTO TweetDTO => throw new NotSupportedException();

    [Obsolete]
    public List<IHashtagEntity> Hashtags => throw new NotSupportedException();

    [Obsolete]
    public List<IUrlEntity> Urls => throw new NotSupportedException();

    [Obsolete]
    public List<IMediaEntity> Media => throw new NotSupportedException();

    [Obsolete]
    public List<IUserMentionEntity> UserMentions => throw new NotSupportedException();

    [Obsolete]
    public bool IsRetweet => throw new NotSupportedException();

    [Obsolete]
    public ITweet RetweetedTweet => throw new NotSupportedException();

    [Obsolete]
    public int? QuoteCount => throw new NotSupportedException();

    [Obsolete]
    public long? QuotedStatusId => throw new NotSupportedException();

    [Obsolete]
    public string QuotedStatusIdStr => throw new NotSupportedException();

    [Obsolete]
    public ITweet QuotedTweet => throw new NotSupportedException();

    [Obsolete]
    public string Url => throw new NotSupportedException();

    public TweetMode TweetMode => throw new NotSupportedException();

    [Obsolete]
    public Task DestroyAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task DestroyRetweetAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public bool Equals(ITweet? other)
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task FavoriteAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task<IOEmbedTweet> GenerateOEmbedTweetAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task<ITweet[]> GetRetweetsAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task<ITweet> PublishRetweetAsync()
    {
        throw new NotImplementedException();
    }

    [Obsolete]
    public Task UnfavoriteAsync()
    {
        throw new NotImplementedException();
    }
}
