using BattleShip.API.Domain;
using BattleShip.Models;

namespace BattleShip.API.Service;
public class AiService {

	public Position GetNextMove(Game game) {
		return game.AiDifficulty switch {
			1 => RandomStrategy(game, game.PlayerOneGrid),
			2 => HuntStrategy(game, game.PlayerOneGrid),
			3 => HardStrategy(game, game.PlayerOneGrid),
			_ => RandomStrategy(game, game.PlayerOneGrid)
		};
	}

	private Position RandomStrategy(Game game, Grid grid) {
		if (game.OpponentAvailablePosition == null) {
			Position[] availableHits = new Position[grid.Size * grid.Size];
			for (int row = 0; row < grid.Size; row++) {
				for (int col = 0; col < grid.Size; col++) {
					if (game.PlayerOneGrid.Hits.Any(hit => hit.Col == col && hit.Row == row))
						continue;
					availableHits[row * grid.Size + col] = new(row, col);
				}
			}
			Random.Shared.Shuffle(availableHits);
			game.OpponentAvailablePosition = new(availableHits);
		}
		return game.OpponentAvailablePosition.Dequeue();
	}

	private Position HuntStrategy(Game game, Grid grid) {
		(int dx, int dy)[] directions = [
			(1, 0), // right
			(-1, 0), // left
			(0, 1), // down
			(0, -1) // up
		];
		return Hunt(game, grid, directions);
	}

	private Position HardStrategy(Game game, Grid grid) {
		(int dx, int dy)[] directions = [
			(1, 0),
			(-1, 0),
			(0, 1),
			(0, -1),
			(1, 1),
			(-1, -1),
			(1, -1),
			(-1, 1)
		];
		return Hunt(game, grid, directions);
	}

	private Position Hunt(Game game, Grid grid, (int dx, int dy)[] directions) {
		var successfulHits = grid.Hits.Where(hit => hit.HasHit && !hit.HasSunk).ToList();

		if (successfulHits.Count == 0) {
			return RandomStrategy(game, grid);
		}

		List<Position> possiblePositions = [];

		foreach (var (row, col) in directions) {
			foreach (var hit in successfulHits) {
				if (hit.Col + col < 0 || hit.Col + col >= grid.Size || hit.Row + row < 0 || hit.Row + row >= grid.Size)
					continue;
				possiblePositions.Add(new(hit.Row + row, hit.Col + col));
			}
		}

		possiblePositions = possiblePositions.Where(pos => !grid.Hits.Any(hit => hit.Col == pos.Col && hit.Row == pos.Row)).Distinct().ToList();

		if (possiblePositions.Count != 0) {
			return possiblePositions[Random.Shared.Next(possiblePositions.Count)];
		} else {
			return RandomStrategy(game, grid);
		}
	}

}
