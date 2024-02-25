using BattleShip.API.Domain;
using BattleShip.API.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ControllerAttribute = MinimalApiSourceGenerator.ControllerAttribute;

namespace BattleShip.API;

[Controller]
public partial class GameController {

	[Authorize]
	[HttpGet("/GetGame/{gameId}")]
	public static Results<JsonHttpResult<GameDto>, NotFound> GetGame([FromRoute] Guid gameId, GameService gameService) {
		return gameService.GetGame(gameId).Match<Results<JsonHttpResult<GameDto>, NotFound>>(
			(game) => TypedResults.Json(Mapper.MapToGameDto(game)),
			(notfound) => TypedResults.NotFound()
		);
	}

	[Authorize]
	[HttpGet("/ExistGame/{gameId}")]
	public static Results<Ok, NotFound> ExistGame([FromRoute] Guid gameId, GameService gameService) {
		return gameService.GetGame(gameId).Match<Results<Ok, NotFound>>(
			(game) => TypedResults.Ok(),
			(notfound) => TypedResults.NotFound()
		);
	}

	[Authorize]
	[HttpPost("/CreateGame")]
	public static CreateGameDto CreateGame([FromBody] CreateGameCommand command, GameService gameService) {
		return Mapper.MapToCreateGameDto(gameService.CreateGame(command.Ai, command.AiDifficulty));
	}

	[Authorize]
	[HttpPost("/Hit")]
	public static Results<JsonHttpResult<GameStateDto>, NotFound, ForbidHttpResult> Hit([FromBody] HitCommand hitCommand, GameService gameService) {
		return gameService.Hit(hitCommand.gameId, new Position(hitCommand.x, hitCommand.y))
			.Match<Results<JsonHttpResult<GameStateDto>, NotFound, ForbidHttpResult>>(
				(gameState) => TypedResults.Json(gameState),
				(notfound) => TypedResults.NotFound(),
				(gamealreadyfinished) => TypedResults.Forbid()
			);
	}
}