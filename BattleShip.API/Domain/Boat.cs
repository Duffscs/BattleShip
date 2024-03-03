namespace BattleShip.API.Domain; 
public class Boat {
	public string Name { get; set; }
	public int Size { get; set; }
	public int Row { get; set; }
	public int Col { get; set; }

	public int Life { get; set; }

	public bool IsHorizontal { get; set; }

	public Boat(string name, int taille) {
		Name = name;
		Size = taille;
		Row = -taille;
		Col = -taille;
		Life = taille;
	}
}
