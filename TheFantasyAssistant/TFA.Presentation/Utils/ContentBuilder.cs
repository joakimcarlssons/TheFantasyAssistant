using System.Text;
using TFA.Domain.Data;

namespace TFA.Presentation.Utils;

internal sealed class ContentBuilder
{
    private string Content = "";

    public ContentBuilder AppendFantasyTypeHashTag(FantasyType fantasyType)
    {
        StringBuilder sb = new(Content);
        Content = sb
            .Append(fantasyType switch
            {
                FantasyType.FPL => "#FPL",
                FantasyType.Allsvenskan => "#AllsvenskanFantasy",
                _ => string.Empty
            })
            .ToString();

        return this;
    }

    public ContentBuilder AppendText(string text)
    {
        StringBuilder sb = new(Content);
        Content = sb
            .Append(text)
            .ToString();

        return this;
    }

    public ContentBuilder AppendTextConditionally(string text, bool condition)
    {
        if (condition)
        {
            StringBuilder sb = new(Content);
            Content = sb
                .Append(text)
                .ToString();
        }

        return this;
    }

    public ContentBuilder AppendTextWithLineBreak(string text)
    {
        StringBuilder sb = new(Content);
        Content = sb
            .Append(text)
            .Append(Environment.NewLine)
            .ToString();

        return this;
    }

    public ContentBuilder AppendStandardHeader(FantasyType fantasyType, string text)
    {
        Content = string.Empty;
        AppendFantasyTypeHashTag(fantasyType);
        AppendText(string.Concat(" ", text));
        AppendLineBreaks(2);

        return this;
    }

    public ContentBuilder AppendLineBreaks(int amount = 1)
    {
        Content += new StringBuilder()
            .Insert(0, Environment.NewLine, amount)
            .ToString();

        return this;
    }

    public ContentBuilder AppendTextLines<T>(Func<T, string> lineConverter, IReadOnlyCollection<T> values)
    {
        Content += Environment.NewLine + string.Join(Environment.NewLine, values.Select(lineConverter));
        return this;
    }

    public ContentBuilder AppendCustomText(Func<string> builder)
    {
        Content += builder.Invoke();
        return this;
    }

    public string Build() => Content;
    public static implicit operator string(ContentBuilder builder) => builder.Build();
}

internal static class ContentBuilderUtils
{
    public static string ConvertPriceToString(this decimal price) => price.ToString().Replace(',', '.');
}
