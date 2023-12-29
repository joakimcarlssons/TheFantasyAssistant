using TFA.Application.Features.BaseData.Transforms;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.BaseData.Events;

public sealed record BaseDataPresentModel(
    FantasyType FantasyType,
    TransformedBaseData Data) : INotification, IPresentable;
