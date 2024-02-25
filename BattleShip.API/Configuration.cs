using BattleShip.API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BattleShip.API;
public static class Configuration {
	public static void ConfiguresServices(this WebApplicationBuilder applicationBuilder) {
		IServiceCollection services = applicationBuilder.Services;
		ConfigurationManager configuration = applicationBuilder.Configuration;
		
		services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddCors()
			.AddSingleton<GameService>()
			.AddSingleton<GridService>()
			.AddSingleton<AiService>();

		services.AddAuthentication(options => {
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options => {
			options.Authority = $"https://{configuration["Auth0:Domain"]}";
			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
				ValidAudience = configuration["Auth0:Audience"],
				ValidIssuer = $"https://{configuration["Auth0:Domain"]}"
			};
		});

		services.AddAuthorization(o => {
			//o.AddPolicy("battleship:read-write", p => p.
			//	RequireAuthenticatedUser().
			//	RequireClaim("scope", "battleship:read-write"));
		});
	}
}
