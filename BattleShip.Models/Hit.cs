namespace BattleShip.Models;
public class Hit {
	public int X { get; set; }
	public int Y { get; set; }
	public bool HasHit { get; set; }
	public bool HasSunk { get; set; }
	public string SunkenBoat { get; set; }

}