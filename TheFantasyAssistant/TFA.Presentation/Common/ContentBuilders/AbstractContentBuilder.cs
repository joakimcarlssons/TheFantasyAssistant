using System.Diagnostics.CodeAnalysis;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Common.ContentBuilders;

public abstract class AbstractContentBuilder<TIn, TOut> : IContentBuilder<TIn, TOut>
    where TIn : IPresentable
{
    public abstract Presenter Presenter { get; }

    public abstract IReadOnlyList<TOut> Build(TIn data);

    protected static string NowDate => DateTime.Now.ToString("yyyy-MM-dd");

    protected static string BuildStringContent<T>(Func<IReadOnlyList<T>, string, string, string> builder, IReadOnlyList<T> items, string header, [ConstantExpected] string emoji)
        => items.Count > 0 ? builder.Invoke(items, header, emoji) : string.Empty;

    protected static string GetFixtureDifficultyEmoji(int fixtureDiffculty)
        => fixtureDiffculty switch
        {
            < 3 => Emoji.GreenCircle,
            3 => Emoji.WhiteCircle,
            _ => Emoji.RedCircle
        };

    protected static string GetOpponentHomeAwayText(bool isHome)
        => isHome ? "(A)" : "(H)";
}