using BattleShip.App.Component.Dialog;
using BattleShip.App.Service;
using BattleShip.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace BattleShip.App.Pages {
	public partial class Game {
		[Parameter]
		public string GameId { get; set; }

		string[,] PlayerGrid { get; set; }
		int GridSize { get; set; }
		Hit[,] PlayerHitGrid { get; set; }
		Hit[,] OpponentHitGrid { get; set; }
		List<Hit> OpponentHitList { get; set; }
		List<Hit> PlayerHitList { get; set; }

		bool gameFinished;
		bool gameExist;

		Stack<(string, Hit)> GameHits { get; set; } = new();

		protected override async Task OnInitializedAsync() {
			if (GameId == null) {
				NavigationManager.NavigateTo("/");
				return;
			}

			var gameDto = await GameService.GetGame(GameId);
			if (gameDto is null) {
				Snackbar.Add("Cette Game n'existe pas.", Severity.Error);
				return;
			}

			PrepareData(gameDto);
		}

		private void PrepareData(GameDto g) {
			GridSize = g.GridSize;
			gameExist = true;
			PlayerGrid = new string[g.GridSize, g.GridSize];
			PlayerHitGrid = new Hit[g.GridSize, g.GridSize];
			OpponentHitGrid = new Hit[g.GridSize, g.GridSize];
			OpponentHitList = g.OpponentHits;
			PlayerHitList = g.PlayerHits;
			for (int i = 0; i < g.GridSize; i++) {
				for (int j = 0; j < g.GridSize; j++) {
					PlayerGrid[i, j] = "";
					PlayerHitGrid[i, j] = null;
					OpponentHitGrid[i, j] = null;
				}
			}
			foreach (BoatDto b in g.BoatsPosition) {
				for (int i = 0; i < b.Size; i++) {
					int row = b.IsHorizontal ? b.Row : b.Row + i;
					int col = b.IsHorizontal ? b.Col + i : b.Col;
					if (i == 0) {
						PlayerGrid[row, col] = $"{b.Name} bow {(b.IsHorizontal ? "horizontal" : "vertical")}";
					} else if (i == b.Size - 1) {
						PlayerGrid[row, col] = $"{b.Name} stern {(b.IsHorizontal ? "horizontal" : "vertical")}";
					} else {
						PlayerGrid[row, col] = $"{b.Name} hull {(b.IsHorizontal ? "horizontal" : "vertical")}";
					}
                }
			}
			foreach (var h in g.PlayerHits) {
				PlayerHitGrid[h.Row, h.Col] = h;
			}
			foreach (var h in g.OpponentHits) {
				OpponentHitGrid[h.Row, h.Col] = h;
			}

			for (int i = 0; i < g.PlayerHits.Count; i++) {
				GameHits.Push(("Player", g.PlayerHits[i]));
				if (g.OpponentHits.ElementAtOrDefault(i) != null) {
					GameHits.Push(("Opponent", g.OpponentHits[i]));
				}
			}
		}

		private async Task ClickCell(int row, int col) {
			if (gameFinished) {
				Snackbar.Add("La partie est termin�e", Severity.Error);
				return;
			}
			if (PlayerHitGrid[row, col] != null) {
				Snackbar.Add("Cette cellule a d�j� �t� utilis�", Severity.Error);
				return;
			}
			PlayerHitGrid[row, col] = new();

			GameStateDto? gameState = await GameService.Hit(new HitCommand(Guid.Parse(GameId), row, col));
			if (gameState == null) {
				Snackbar.Add("Erreur lors de l'envoi de la requ�te", Severity.Error);
				return;
			}

			PlayerHitGrid[row, col] = gameState.PlayerHit;
			GameHits.Push(("Player", gameState.PlayerHit));
			if (gameState.OpponentHit != null) {
				OpponentHitGrid[gameState.OpponentHit.Row, gameState.OpponentHit.Col] = gameState.OpponentHit;
				GameHits.Push(("Opponent", gameState.OpponentHit));
			}
			StateHasChanged();

			if (gameState.HasWinner) {
				gameFinished = true;
				var parameters = new DialogParameters<ValidationDialog> {
					{ x => x.Content, $"Vous avez {(gameState.Winner == 0 ? "Gagn�" : "Perdu")}. Rejouer ?" },
					{ x => x.CancelText, "Non" },
					{ x => x.SubmitText, "Oui" }
				};
				var res = await DialogService.Show<ValidationDialog>("Partie termin�e", parameters, new DialogOptions() { CloseButton = true }).Result;
				if (res.Canceled) {
					return;
				}
				var dialog = await DialogService.Show<CreateGameDialog>("Cr�er une parti", new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge }).Result;
				if (dialog.Canceled) {
					return;
				}
				var a = (CreateGameDialog.CreateGameDialogResults)dialog.Data;

				CreateGameDto? game = await GameService.CreateGame(new CreateGameCommand(a.Ai, a.Difficulty, a.Size));
				if (game == null) {
					Snackbar.Add("Erreur lors de la cr�ation de la partie", Severity.Error);
					return;
				}
				NavigationManager.NavigateTo($"/{game.Id}");
			}
		}

		private static string HitStateString(Hit hit) {
			if (hit == null) {
				return "";
			}
			if (hit.HasSunk) {
				return "sunken";
			}
			if (hit.HasHit) {
				return "hitted";
			}
			return "missed";
		}
	}
}