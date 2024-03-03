namespace BattleShip.Models;
public record class GameDto(Guid GameId, int GridSize, List<BoatDto> BoatsPosition, List<Hit> PlayerHits, List<Hit> OpponentHits) {}
