namespace BattleShip.API.Exception; 
public readonly record struct NotFound {
	public static NotFound Instance { get; } = new NotFound();
}

public readonly record struct GameAlreadyFinished {
	public static GameAlreadyFinished Instance { get; } = new GameAlreadyFinished();
}