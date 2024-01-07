using System.Text;
using TFA.Application.Features.Bots.Discord;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Common.ContentBuilders.Discord;

public class DiscordBotCommandContentBuilder : AbstractContentBuilder<BestFixturesCommandResponse, string>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<string> Build(BestFixturesCommandResponse data)
            => [
                    new ContentBuilder().AppendCustomText(() =>
                    {
                        StringBuilder sb = new();

                        foreach (DiscordCommandBestFixturesTeam team in data.Teams)
                        {
                            sb.AppendLine($"{Emoji.Star}{team.Name}");
                            foreach (DiscordCommandBestFixturesTeamOpponent opponent in team.Opponents)
                            {
                                sb.AppendLine($"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} " +
                                    $"{opponent.OpponentShortName} ({(opponent.IsHome ? "H" : "A")})");
                            }

                            sb.AppendLine();
                        }

                        return sb.ToString();
                    })
               ];
}
