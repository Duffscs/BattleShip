namespace BattleShip.Models;
public record class CreateGameCommand(bool Ai, int AiDifficulty, int GridSize) {}
