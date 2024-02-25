using BattleShip.API.Domain;
using BattleShip.API.Exception;
using BattleShip.Models;

namespace BattleShip.API.Service;
public class GameService(GridService gridService, AiService aiService) {

	readonly Dictionary<Guid, Game> games = [];

	public Game CreateGame(bool ai, int aiDifficulty) {
		Grid playerGrid = gridService.GenerateGrid(0, 10);
		Grid opponentGrid = gridService.GenerateGrid(1, 10);
		Game game = new(playerGrid, opponentGrid, ai, aiDifficulty);
		games.Add(game.Id, game);
		return game;
	}

	public GameOrNotFound GetGame(Guid gameId) {
		if (!games.TryGetValue(gameId, out Game? value))
			return NotFound.Instance;
		return value;
	}

	public GameStateOrNotFoundOrGameAlreadyFinished Hit(Guid gameId, Position pos) {
		GameOrNotFound res = GetGame(gameId);

		if (res.IsT1)
			return res.AsT1;

		Game game = res.AsT0;
		if (game.Winner != -1)
			return GameAlreadyFinished.Instance;

		GameStateDto result = new();
		result.PlayerHit = PlayerHit(ref result, game, game.PlayerOneGrid, game.PlayerTwoGrid, pos);
		if (!result.HasWinner) {
			result.OpponentHit = PlayerHit(ref result, game, game.PlayerTwoGrid, game.PlayerOneGrid, aiService.GetNextMove(game));
		}
		return result;
	}

	private Hit PlayerHit(ref GameStateDto result, Game game, Grid offenderGrid, Grid victimGrid, Position pos) {
		Hit hit = new() { X = pos.X, Y = pos.Y };
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
