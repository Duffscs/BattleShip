using BattleShip.API.Domain;
using BattleShip.Models;

namespace BattleShip.API.Service;

public class GridService {

	public GridService() { }

	public Grid GenerateGrid(int playerId, int gridSize) {
		Grid grid = new(playerId, gridSize);

		List<Boat> boats = [
			new("A", 2),
			new("B", 2),
			new("C", 3),
			new("D", 3),
			new("E", 4),
			new("F", 4),
		];
		foreach (Boat boat in boats) {
			AssignRandomPosition(boat, grid);
			grid.Boats.Add(boat);
		}
		return grid;
	}

	private void AssignRandomPosition(Boat boat, Grid grid) {
		boat.IsHorizontal = Random.Shared.Next(2) == 0;
		bool isPositionValid = false;
		while (!isPositionValid) {
			boat.PosX = Random.Shared.Next(boat.IsHorizontal ? grid.Size - boat.Size : grid.Size);
			boat.PosY = Random.Shared.Next(boat.IsHorizontal ? grid.Size : grid.Size - boat.Size);
			isPositionValid = true;
			foreach (Boat otherBoat in grid.Boats) {
				if (IsOverlapping(boat, otherBoat)) {
					isPositionValid = false;
					break;
				}
			}
		}
	}

	public Boat? HasHitAny(Grid grid, Position pos) {
		foreach (Boat boat in grid.Boats) {
			if (IsHit(boat, pos.X, pos.Y)) {
				return boat;
			}
		}
		return null;
	}

	private bool IsOverlapping(Boat boat, Boat otherBoat) {
		for (int i = 0; i < boat.Size; i++) {
			int x = boat.PosX;
			int y = boat.PosY;
			if (boat.IsHorizontal) {
				x += i;
			} else {
				y += i;
			}
			if (IsHit(otherBoat, x, y)) {
				return true;
			}
		}
		return false;
	}

	private bool IsHit(Boat boat, int x, int y) {
		for (int i = 0; i < boat.Size; i++) {
			int boatX = boat.PosX;
			int boatY = boat.PosY;
			if (boat.IsHorizontal) {
				boatX += i;
			} else {
				boatY += i;
			}
			if (boatX == x && boatY == y) {
				return true;
			}
		}
		return false;
	}
}


