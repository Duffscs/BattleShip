using FluentValidation;

namespace BattleShip.Models.Validator;
public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand> {

	public CreateGameCommandValidator() {
		RuleFor(x => x.GridSize).InclusiveBetween(Const.MinGridSize, Const.MaxGridSize);
		RuleFor(x => x.AiDifficulty).InclusiveBetween(Const.MinAiDifficulty, Const.MaxAiDifficulty);
	}

}
