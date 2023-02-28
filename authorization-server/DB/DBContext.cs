using authorization_server.Models;
using Microsoft.EntityFrameworkCore;

namespace authorization_server.DB;

public class DBContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<AuthSession> Sessions { get; set; } = null!;
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
        Database.EnsureCreated();   
    }
    
    public static DBContext db;
     
}