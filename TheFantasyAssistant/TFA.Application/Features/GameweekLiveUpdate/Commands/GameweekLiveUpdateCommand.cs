using TFA.Application.Common.Commands;

namespace TFA.Application.Features.FixtureLiveUpdate.Commands;

public sealed class GameweekLiveUpdateCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<GameweekLiveUpdateData>>(fantasyType);
