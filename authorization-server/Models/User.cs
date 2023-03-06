using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace authorization_server.Models;

public class User
{
    
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}
