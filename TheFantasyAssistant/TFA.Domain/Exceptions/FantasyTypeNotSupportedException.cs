namespace TFA.Domain.Exceptions;

public class FantasyTypeNotSupportedException : Exception
{
    private const string _message = "Provided fantasy type is not supported.";

    public FantasyTypeNotSupportedException() : base(_message) { }
    public FantasyTypeNotSupportedException(string message) : base(message) { }
    public FantasyTypeNotSupportedException(string message, Exception innerException) : base(message, innerException) { }
}
