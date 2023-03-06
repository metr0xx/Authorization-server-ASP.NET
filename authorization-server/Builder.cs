using System.Collections.Immutable;
using System.Text.Json;
using authorization_server.DB;
using authorization_server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace authorization_server;

public class Builder
{
    public static WebApplicationBuilder Build = WebApplication.CreateBuilder();
    public static readonly string Connection = Build.Configuration.GetConnectionString("DefaultConnection");
    private static DbContextOptionsBuilder<DBContext> _optionsBuilder = new DbContextOptionsBuilder<DBContext>();
    public static DbContextOptions<DBContext> Options = _optionsBuilder.UseSqlServer(Connection).Options;
}