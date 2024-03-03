using BattleShip.Models;

namespace BattleShip.API.Domain; 
public class GameState {
	public Hit PlayerHit { get; set; }
	public Hit OpponentHit { get; set; }
	public bool HasWinner { get; set; }
	public int Winner { get; set; }
}
