using TFA.Application.Common.Commands;

namespace TFA.Application.Features.LeagueData.Commands;

public class LeagueTableCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<LeagueTableData>>(fantasyType);
