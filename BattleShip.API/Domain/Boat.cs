namespace BattleShip.API.Domain; 
public class Boat {
	public string Name { get; set; }
	public int Size { get; set; }
	public int PosX { get; set; }
	public int PosY { get; set; }

	public int Life { get; set; }

	public bool IsHorizontal { get; set; }

	public Boat(string name, int taille) {
		Name = name;
		Size = taille;
		PosX = -taille;
		PosY = -taille;
		Life = taille;
	}
}
