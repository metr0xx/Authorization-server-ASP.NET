using System.Data;
using authorization_server.DB;
using authorization_server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Utils = authorization_server.Utils;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connection));

var app = builder.Build();

app.Map("/login/", () =>
{

});

app.Map("/register/", () =>
{

});

app.MapGet("/", () =>
{
    return Utils.generateToken();
});
app.Run();