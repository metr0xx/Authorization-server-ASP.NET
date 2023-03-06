using authorization_server;
using authorization_server.DB;
using Microsoft.EntityFrameworkCore;

var builder = Builder.Build;

var connection = Builder.Connection;

builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connection));
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

app.MapPost("/login", async (context) =>
{
    var values = await context.Request.ReadFromJsonAsync<Dictionary<string, string>>();
    await context.Response.WriteAsJsonAsync(Interaction.Login(values["login"], values["password"], values["refreshToken"]));
});

app.MapPost("/register", async (context) =>
{
    var values =  await context.Request.ReadFromJsonAsync<Dictionary<string, string>>();
    await context.Response.WriteAsJsonAsync(Interaction.Register(values["login"], values["password"]));
});

app.MapGet("/", () =>
{
    return Interaction.Db.Users.ToList();
});

app.Run();