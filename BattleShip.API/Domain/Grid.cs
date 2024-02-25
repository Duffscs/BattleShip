using BattleShip.Models;

namespace BattleShip.API.Domain;
public class Grid {
	public int PlayerId { get; set; }
	public int Size { get; set; }
	public List<Boat> Boats { get; set; }
	public List<Hit> Hits { get; set; }

	public Grid(int id, int size) {
		PlayerId = id;
		Size = size;
		Boats = [];
		Hits = [];
	}
}
