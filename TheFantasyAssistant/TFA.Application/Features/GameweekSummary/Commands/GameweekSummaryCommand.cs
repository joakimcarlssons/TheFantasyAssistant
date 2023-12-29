using TFA.Application.Common.Commands;

namespace TFA.Application.Features.GameweekFinished.Commands;

public sealed class GameweekSummaryCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<GameweekSummaryData>>(fantasyType);
