using TFA.Application.Common.Commands;
using TFA.Domain.Models;

namespace TFA.Application.Features.BaseData.Commands;

public sealed class BaseDataCommand(FantasyType fantasyType) : AbstractDataCommand<ErrorOr<FantasyBaseData>>(fantasyType);
