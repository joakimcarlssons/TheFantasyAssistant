namespace TFA.Utils;

public static class EnumerableUtils
{
    public static IEnumerable<IEnumerable<T>> SplitEnumerable<T>(this IEnumerable<T> enumerable, int size)
        => enumerable
            .Select((v, i) => new
            {
                Value = v,
                Index = i
            })
            .GroupBy(x => x.Index / size)
            .Select(g => g.Select(x => x.Value));
}
