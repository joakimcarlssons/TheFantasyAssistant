using TFA.Application.Interfaces.Services;

namespace TFA.Application.Common.Data;

public abstract record SummaryData : INotification, IPresentable;

public abstract record SummaryTeamToTarget(int NumberOfOpponents, int TotalDifficulty, int Position);

public abstract record SummaryTeamOpponent;