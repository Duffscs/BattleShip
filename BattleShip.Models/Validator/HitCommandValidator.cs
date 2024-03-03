
using FluentValidation;

namespace BattleShip.Models.Validator;
public class HitCommandValidator : AbstractValidator<HitCommand> {
	public HitCommandValidator(int gridSize) {
		RuleFor(x => x.GameId).NotEmpty();
		RuleFor(x => x.Col).InclusiveBetween(0, gridSize);
		RuleFor(x => x.Row).InclusiveBetween(0, gridSize);
	}
}
