using TFA.Domain.Exceptions;

namespace TFA.Application.Features.Transforms;

public interface ITransformer<TFrom, TTo>
{
    TTo Transform(TFrom newData, TFrom prevData);
}

public static class TransformerExtensions
{
    public static TTo Transform<TFrom, TTo>(this TFrom newData, TFrom prevData) where TTo : class
        => GetTransformer<TFrom, TTo>().Transform(newData, prevData);

    private static ITransformer<TFrom, TTo> GetTransformer<TFrom, TTo>()
    {
        ITransformer<TFrom, TTo>? transformer = AssemblyReference.Assembly.ExportedTypes
            .Where(x => typeof(ITransformer<TFrom, TTo>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ITransformer<TFrom, TTo>>()
            .FirstOrDefault();

        return transformer is null
            ? throw new TransformerNotFoundException<TFrom, TTo>()
            : transformer;
    }
}