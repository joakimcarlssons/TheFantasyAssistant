namespace TFA.Twitter.Exceptions;

public class TweetException : Exception
{
    public TweetException() : base($"Failed to send tweet.") { }
    public TweetException(string message) : base(message) { }
    public TweetException(string message, Exception innerException) : base(message, innerException) { }
}