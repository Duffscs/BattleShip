using BattleShip.Models;
using System.Net.Http.Json;

namespace BattleShip.App.Service;
public class GameService(HttpClient Http) {

	public async Task<CreateGameDto?> CreateGame(CreateGameCommand gameCommand) {
		HttpResponseMessage res = await Http.PostAsJsonAsync("/CreateGame", gameCommand);
		if (res.IsSuccessStatusCode) {
			return await res.Content.ReadFromJsonAsync<CreateGameDto>();
		}
		return null;
	}

	public async Task<GameDto?> GetGame(string gameId) {
		HttpResponseMessage res = await Http.GetAsync($"/GetGame/{gameId}");
		if (res.IsSuccessStatusCode) {
			return await res.Content.ReadFromJsonAsync<GameDto>();
		}
		return null;
	}

	public async Task<GameStateDto?> Hit(HitCommand hitCommand) {
		HttpResponseMessage response = await Http.PostAsJsonAsync("/Hit", hitCommand);
		if (!response.IsSuccessStatusCode) {
			return null;
		}
		return await response.Content.ReadFromJsonAsync<GameStateDto>();
	}


}
