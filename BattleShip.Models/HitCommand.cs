namespace BattleShip.Models;
public record class HitCommand(Guid GameId, int Row, int Col) {}
