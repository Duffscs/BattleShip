using BattleShip.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;

namespace BattleShip.App.Service; 
public class GameService(HttpClient Http, NavigationManager NavigationManager, ISnackbar Snackbar) {

	public async Task CreateGame() {
		var res = await Http.PostAsJsonAsync("/CreateGame", new { });
		if (res.IsSuccessStatusCode) {
			CreateGameDto game = await res.Content.ReadFromJsonAsync<CreateGameDto>();
			NavigationManager.NavigateTo($"/{game.Id}");
		} else {
			Snackbar.Add("Erreur lors de la création de la partie", Severity.Error);
		}
	}


}
