using TFA.Application.Common.Commands;

namespace TFA.Application.Features.PredictedPriceChanges.Commands;

public sealed class PredictedPriceChangeCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<PredictedPriceChangeData>>(fantasyType);
