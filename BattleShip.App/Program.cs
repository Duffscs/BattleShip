using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.App;
using MudBlazor.Services;
using MudBlazor;
using BattleShip.App.Service;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7202") });
builder.Services.AddHttpClient("ServerAPI",
	  client => client.BaseAddress = new Uri(builder.Configuration["ApiHost"]!))
	.AddHttpMessageHandler(sp => {
		var httpMessageHandler = sp.GetService<AuthorizationMessageHandler>()?
			.ConfigureHandler(authorizedUrls: new[] { builder.Configuration["ApiHost"]! });
		return httpMessageHandler ?? throw new NullReferenceException(nameof(AuthorizationMessageHandler));
	});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
  .CreateClient("ServerAPI"));
builder.Services.AddScoped<GameService>();

builder.Services.AddMudServices(config => {
	config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

	config.SnackbarConfiguration.PreventDuplicates = false;
	config.SnackbarConfiguration.NewestOnTop = false;
	config.SnackbarConfiguration.ShowCloseIcon = true;
	config.SnackbarConfiguration.VisibleStateDuration = 10000;
	config.SnackbarConfiguration.HideTransitionDuration = 500;
	config.SnackbarConfiguration.ShowTransitionDuration = 500;
	config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddOidcAuthentication(options => {
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
});
await builder.Build().RunAsync();
