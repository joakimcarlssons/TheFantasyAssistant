namespace TFA.Domain.Exceptions;

public class TransformerNotFoundException<TFrom, TTo> : Exception
{
    public TransformerNotFoundException() : base($"Transformer from {typeof(TFrom)} to {typeof(TTo)} has not been registered.") { }
    public TransformerNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
