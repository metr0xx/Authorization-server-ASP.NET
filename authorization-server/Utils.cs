namespace authorization_server;

public class Utils
{
    public static string generateToken()
    {
        var token = "";
        var random = new Random();

        for (var i = 0; i < 40; i++)
            token += random.Next(0, 2) == 1 ? 
                random.Next(0, 10) : 
                random.Next(0, 2) == 1 ? 
                    ((char)random.Next(65, 91)).ToString() : 
                    ((char)random.Next(97, 123)).ToString();
        
        return token;
    }
}