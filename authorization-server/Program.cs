using System.Data;
using System.Text.Json;
using authorization_server.DB;
using authorization_server.Models;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Utils = authorization_server.Utils;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connection));

var app = builder.Build();

app.MapPost("/login/", (string loginData) =>
{
    var values = JsonSerializer.Deserialize<Dictionary<string, string>>(loginData);
    return JsonSerializer.Serialize(Interaction.Login(values["login"], values["password"], values["refreshToken"]));
});

app.Map("/register/", (string registerData) =>
{
    var values = JsonSerializer.Deserialize<Dictionary<string, string>>(registerData);
    return JsonSerializer.Serialize(Interaction.Register(values["login"], values["password"]));
});

app.MapGet("/", () =>
{
    return Utils.generateToken();
});
app.Run();