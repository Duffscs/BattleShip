using BattleShip.Models;

namespace BattleShip.API.Domain;
public static class Mapper {
	public static GameDto MapToGameDto(Game game) {
		List<BoatDto> boats = [];
		Grid grid = game.PlayerOneGrid;

		foreach(Boat boat in grid.Boats) {
			boats.Add(new(boat.Name, boat.Size, boat.IsHorizontal, boat.Col, boat.Row));
		}
		return new GameDto(game.Id, game.PlayerOneGrid.Size, boats, game.PlayerTwoGrid.Hits, game.PlayerOneGrid.Hits);
	}

	public static CreateGameDto MapToCreateGameDto(Game game) {
		return new CreateGameDto(game.Id);
	}

	public static GameStateDto MapToGameStateDto(GameState gameState) {
		return new GameStateDto(gameState.PlayerHit, gameState.OpponentHit, gameState.HasWinner, gameState.Winner);
	}
}