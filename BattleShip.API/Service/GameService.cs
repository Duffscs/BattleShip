using BattleShip.API.Domain;
using BattleShip.API.Exception;
using BattleShip.Models;

namespace BattleShip.API.Service;
public class GameService(GridService gridService, AiService aiService) {

	readonly Dictionary<Guid, Game> games = [];

	public Game CreateGame(bool ai, int aiDifficulty, int gridSize) {
		Grid playerGrid = gridService.GenerateGrid(0, gridSize);
		Grid opponentGrid = gridService.GenerateGrid(1, gridSize);
		Game game = new(playerGrid, opponentGrid, ai, aiDifficulty);
		games.Add(game.Id, game);
		return game;
	}

	public GameOrNotFound GetGame(Guid gameId) {
		if (!games.TryGetValue(gameId, out Game? value))
			return NotFound.Instance;
		return value;
	}

	public GameStateOrGameAlreadyFinished Hit(Game game, Position pos) {

		if (game.Winner != -1)
			return GameAlreadyFinished.Instance;

		GameState result = new();
		result.PlayerHit = PlayerHit(result, game, game.PlayerOneGrid, game.PlayerTwoGrid, pos);
		if (!result.HasWinner) {
			result.OpponentHit = PlayerHit(result, game, game.PlayerTwoGrid, game.PlayerOneGrid, aiService.GetNextMove(game));
		}
		return result;
	}

	private Hit PlayerHit(GameState result, Game game, Grid offenderGrid, Grid victimGrid, Position pos) {
		Hit hit = new() { Col = pos.Col, Row = pos.Row };
		victimGrid.Hits.Add(hit);
		Boat? hitted = gridService.HasHitAny(victimGrid, pos);
		if (hit.HasHit = hitted != null) {
			hitted.Life--;
			hit.HasSunk = hitted.Life == 0;
			hit.SunkenBoat = hit.HasSunk ? hitted.Name : null;
		}
		if (hit.HasSunk && victimGrid.Boats.All((b) => b.Life == 0)) {
			result.HasWinner = true;
			game.Winner = result.Winner = offenderGrid.PlayerId;
		}
		return hit;
	}
}
