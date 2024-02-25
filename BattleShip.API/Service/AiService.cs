using BattleShip.API.Domain;
using BattleShip.Models;

namespace BattleShip.API.Service;
public class AiService {

	public Position GetNextMove(Game game) {
		return game.AiDifficulty switch {
			1 => RandomStategy(game, game.PlayerOneGrid),
			2 => HuntStategy(game, game.PlayerOneGrid),
			3 => ProbabilisticStategy(game, game.PlayerOneGrid),
			_ => RandomStategy(game, game.PlayerOneGrid)
		};
	}

	private Position RandomStategy(Game game, Grid grid) {
		if (game.OpponentAvailablePosition == null) {
			Position[] availableHits = new Position[grid.Size * grid.Size];
			for (int i = 0; i < grid.Size; i++) {
				for (int j = 0; j < grid.Size; j++) {
					if(game.PlayerTwoGrid.Hits.Any(hit => hit.X == i && hit.Y == j))
						continue;
					availableHits[i * grid.Size + j] = new(i, j);
				}
			}
			Random.Shared.Shuffle(availableHits);
			game.OpponentAvailablePosition = new(availableHits);
		}
		return game.OpponentAvailablePosition.Dequeue();
	}

	private Position HuntStategy(Game game, Grid grid) {
		Hit lastHit = grid.Hits.FirstOrDefault(hit => hit.HasHit);
		if (lastHit == null || lastHit.HasSunk) {
			return RandomStategy(game, grid);
		}

		(int dx, int dy)[] directions = [
			(lastHit.X + 1, lastHit.Y), // right
			(lastHit.X - 1, lastHit.Y), // left
			(lastHit.X, lastHit.Y + 1), // down
			(lastHit.X, lastHit.Y - 1) // up
		];

		foreach (var (x, y) in directions) {
			if (!(x >= 0 && x < grid.Size && y >= 0 && y < grid.Size))
				continue; //isntInBounds
			if (!grid.Hits.Any(hit => hit.X == x && hit.Y == y)) { // isntPlayed
				return new Position(x, y);
			}
		}
		return RandomStategy(game, grid);
	}

	private List<int> GetRemainingBoatSizes(Grid grid) {
		var sunkBoats = grid.Hits.Where(h => h.HasSunk).Select(h => h.SunkenBoat).Distinct();
		var remainingBoats = grid.Boats.Where(b => !sunkBoats.Contains(b.Name));
		return remainingBoats.Select(b => b.Size).Distinct().ToList();
	}

	private Position ProbabilisticStategy(Game game, Grid grid) {
		int[,] probabilityGrid = new int[grid.Size, grid.Size];
		List<int> remainingBoatSizes = GetRemainingBoatSizes(grid);
		for (int x = 0; x < grid.Size; x++) {
			for (int y = 0; y < grid.Size; y++) {
				if (grid.Hits.Any(hit => hit.X == x && hit.Y == y))
					continue;
				foreach (var size in remainingBoatSizes) {
					probabilityGrid[x, y] += CalculateProbability(x, y, size, true, grid);
					probabilityGrid[x, y] += CalculateProbability(x, y, size, false, grid);
				}
			}
		}

		int maxProbability = 0;
		Position nextMove = new(0, 0);
		for (int x = 0; x < grid.Size; x++) {
			for (int y = 0; y < grid.Size; y++) {
				if (probabilityGrid[x, y] > maxProbability) {
					maxProbability = probabilityGrid[x, y];
					nextMove = new(x, y);
				}
			}
		}
		if (maxProbability == 0) {
			return RandomStategy(game, grid);
		}

		return nextMove;
	}

	private int CalculateProbability(int x, int y, int size, bool isHorizontal, Grid grid) {
		int count = 0;
		int dx = isHorizontal ? 1 : 0;
		int dy = isHorizontal ? 0 : 1;
		for (int step = 0; step < size; step++) {
			int newX = x + step * dx;
			int newY = y + step * dy;
			if (newX < grid.Size && newY < grid.Size && !grid.Hits.Any(hit => hit.X == newX && hit.Y == newY)) {
				count++;
			} else {
				break;
			}
		}
		return count == size ? 1 : 0;
	}


}
