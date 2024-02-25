namespace BattleShip.Models;
public struct GameStateDto {
	public Hit PlayerHit { get; set; }
	public Hit OpponentHit { get; set; }
	public bool HasWinner { get; set; }
	public int Winner { get; set; }

}
