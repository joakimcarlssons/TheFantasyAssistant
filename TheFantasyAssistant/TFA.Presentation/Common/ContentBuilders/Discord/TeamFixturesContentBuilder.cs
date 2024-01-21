using System.Text;
using TFA.Application.Features.Bots.Discord;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Common.ContentBuilders.Discord;

public class TeamFixturesContentBuilder : AbstractContentBuilder<TeamFixturesCommandResponse, string>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<string> Build(TeamFixturesCommandResponse data)
        =>
        [
            new ContentBuilder().AppendCustomText(() =>
            {
                StringBuilder sb = new();

                sb.AppendLine($"{Emoji.Star}{data.Team.Name}");
                foreach (DiscordCommandBestFixturesTeamOpponent opponent in data.Team.Opponents)
                {
                    sb.AppendLine($"{GetFixtureDifficultyEmoji(opponent.FixtureDifficulty)} " +
                        $"GW{opponent.Gameweek} - {opponent.OpponentShortName} ({(opponent.IsHome ? "H" : "A")})");
                }

                return sb.ToString();
            })
        ];
}
