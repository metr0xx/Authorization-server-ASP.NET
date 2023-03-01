using System.Runtime.InteropServices.JavaScript;

namespace authorization_server.Models;

public class AuthSession
{
    public int UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime SessionStart { get; set; }
}