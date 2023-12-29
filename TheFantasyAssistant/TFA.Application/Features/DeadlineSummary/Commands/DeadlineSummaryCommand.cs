using TFA.Application.Common.Commands;

namespace TFA.Application.Features.Deadline.Commands;

public class DeadlineSummaryCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<DeadlineSummaryData>>(fantasyType);
