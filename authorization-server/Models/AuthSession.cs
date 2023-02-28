namespace authorization_server.Models;

public class AuthSession
{
    private int userId { get; set; }
    private string accessToken { get; set; }
    private string refreshToken { get; set; }
}