namespace BattleShip.Models;
public record class GameStateDto(Hit PlayerHit, Hit OpponentHit, bool HasWinner, int Winner) { }
