using BattleShip.Models;

namespace BattleShip.API.Domain;
public static class Mapper {
	public static GameDto MapToGameDto(Game game) {
		List<BoatDto> boats = [];
		Grid grid = game.PlayerOneGrid;
		foreach (Boat boat in grid.Boats) {
			for (int i = 0; i < boat.Size; i++) {
				int x = boat.PosX;
				int y = boat.PosY;
				if (boat.IsHorizontal) {
					x += i;
				} else {
					y += i;
				}
				boats.Add(new(boat.Name, x, y));
			}
		}
		return new GameDto(game.Id, game.PlayerOneGrid.Size, boats, game.PlayerTwoGrid.Hits, game.PlayerOneGrid.Hits);
	}

	public static CreateGameDto MapToCreateGameDto(Game game) {
		return new CreateGameDto(game.Id);
	}
}