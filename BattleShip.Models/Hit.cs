namespace BattleShip.Models;
public class Hit {
	public int Col { get; set; }
	public int Row { get; set; }
	public bool HasHit { get; set; }
	public bool HasSunk { get; set; }
	public string SunkenBoat { get; set; }

}