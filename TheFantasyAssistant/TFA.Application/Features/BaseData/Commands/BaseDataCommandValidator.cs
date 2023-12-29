using FluentValidation;

namespace TFA.Application.Features.BaseData.Commands;

public sealed class BaseDataCommandValidator : AbstractValidator<BaseDataCommand>
{
    public BaseDataCommandValidator()
    {
        RuleFor(x => x.Data.Value.Players).NotEmpty().WithMessage("No players loaded.");
        RuleFor(x => x.Data.Value.Teams).NotEmpty().WithMessage("No teams loaded.");
        RuleFor(x => x.Data.Value.Gameweeks).NotEmpty().WithMessage("No gameweeks loaded.");
        RuleFor(x => x.Data.Value.Fixtures).NotEmpty().WithMessage("No fixtures loaded.");
    }
}
