using BattleShip.API.Domain;
using BattleShip.API.Service;
using BattleShip.Models;
using BattleShip.Models.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ControllerAttribute = MinimalApiSourceGenerator.ControllerAttribute;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace BattleShip.API;

using HitResult = Results<JsonHttpResult<GameStateDto>, NotFound, ForbidHttpResult, BadRequest>;

[Controller]
public partial class GameController {

	[Authorize]
	[HttpGet("/GetGame/{gameId}")]
	public static Results<JsonHttpResult<GameDto>, NotFound> GetGame([FromRoute] Guid gameId, GameService gameService) {
		return gameService.GetGame(gameId).Match<Results<JsonHttpResult<GameDto>, NotFound>>(
			(game) => TypedResults.Json(Mapper.MapToGameDto(game)),
			(notFound) => TypedResults.NotFound()
		);
	}

	[Authorize]
	[HttpGet("/ExistGame/{gameId}")]
	public static Results<Ok, NotFound> ExistGame([FromRoute] Guid gameId, GameService gameService) {
		return gameService.GetGame(gameId).Match<Results<Ok, NotFound>>(
			(game) => TypedResults.Ok(),
			(notFound) => TypedResults.NotFound()
		);
	}

	[Authorize]
	[HttpPost("/CreateGame")]
	public static Results<JsonHttpResult<CreateGameDto>, BadRequest> CreateGame([FromBody] CreateGameCommand command, GameService gameService) {
		if (!new CreateGameCommandValidator().Validate(command).IsValid) {
			return TypedResults.BadRequest();
		}
		return TypedResults.Json(Mapper.MapToCreateGameDto(gameService.CreateGame(command.Ai, command.AiDifficulty, command.GridSize)));
	}


	[Authorize]
	[HttpPost("/Hit")]
	public static HitResult Hit([FromBody] HitCommand hitCommand, GameService gameService) {

		GameOrNotFound res = gameService.GetGame(hitCommand.GameId);
		if (res.IsT1) {
			return TypedResults.NotFound();
		}

		if (!new HitCommandValidator(res.AsT0.PlayerOneGrid.Size).Validate(hitCommand).IsValid) {
			return TypedResults.BadRequest();
		}

		return gameService.Hit(res.AsT0, new Position(hitCommand.Row, hitCommand.Col))
			.Match<HitResult>(
				(gameState) => TypedResults.Json(Mapper.MapToGameStateDto(gameState)),
				(gameAlreadyFinished) => TypedResults.Forbid()
			);
	}
}