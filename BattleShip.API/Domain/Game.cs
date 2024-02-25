using BattleShip.Models;

namespace BattleShip.API.Domain;
public class Game {
	public Guid Id { get; set; } = Guid.NewGuid();
	public bool Ai { get; set; } = false;
	public int AiDifficulty { get; set; } = 0;

	public Grid PlayerOneGrid { get; set; }
	public Grid PlayerTwoGrid { get; set; }

	public Queue<Position> OpponentAvailablePosition { get; set; }

	public int Winner { get; set; } = -1;

	public Game(Grid playerGrid, Grid opponentGrid, bool ai, int aiDifficulty) {
		PlayerOneGrid = playerGrid;
		PlayerTwoGrid = opponentGrid;
		Ai = ai;
		AiDifficulty = aiDifficulty;
	}

}
