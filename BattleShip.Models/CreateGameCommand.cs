namespace BattleShip.Models;
public record struct CreateGameCommand(bool Ai, int AiDifficulty) {}
