using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Interfaces.Services;
using TFA.Twitter;

namespace TFA.Presentation.Presenters;

public class TwitterPresenter(
    IServiceScopeFactory scopeFactory,
    ITwitterService twitterService) : AbstractPresenter(scopeFactory)
{
    public override string Key => PresenterKeys.Twitter;

    public override async Task Present(IPresentable data, CancellationToken cancellationToken)
    {
        foreach (string tweet in BuildContent<string>(data, Presenter.Twitter, data => !string.IsNullOrWhiteSpace(data)))
        {
            await twitterService.TweetAsync(tweet);
        }

        foreach (IReadOnlyList<string> tweets in BuildContent<IReadOnlyList<string>>(data, Presenter.Twitter))
        {
            await twitterService.TweetThreadAsync(tweets);
        }
    }
}
