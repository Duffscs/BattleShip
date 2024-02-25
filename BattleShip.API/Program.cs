using BattleShip.API;
using static BattleShip.API.GameController;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfiguresServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.RegisterGameController();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors((c) => {
	c.AllowAnyMethod();
	c.AllowAnyOrigin();
	c.AllowAnyHeader();
});

app.Run();