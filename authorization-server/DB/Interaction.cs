using authorization_server.Models;
using Microsoft.EntityFrameworkCore;
using authorization_server.Tools;


namespace authorization_server.DB;

public class Interaction
{
    public static DBContext Db = new DBContext(Builder.Options);

    public static Dictionary<string, object> Register(string login, string password)
    {
        if (Db.Users.Any(u => u.Login == login))
        {
            return new Dictionary<string, object>()
            {
                { "status", HTTPstatus.BAD_REQUEST },
                {
                    "data", new Dictionary<string, string>()
                    {
                        { "message", "already exists" }
                    }
                }
            };
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User() { Login = login, Password = hashedPassword };
        Db.Users.Add(user);
        Db.SaveChanges();

        // error handling
        
        return sessionStart(login, password);
    }

    public static Dictionary<string, object> Login(string login, string password, string refreshToken)
    {
        if (password != "" && login != "")
        {
            return sessionStart(login, password);
        }

        if (refreshToken != "")
        {
            return sessionRefresh(refreshToken);
        }

        return new Dictionary<string, object>()
        {
            { "status", HTTPstatus.UNATHORIZED },
            { "data", new Dictionary<string, string>()
                {
                    {"message", "invalid credentials"}
                }
            }
        };
    }

    public static Dictionary<string, object> sessionRefresh(string refreshToken)
    {
        /*var sessions = new List<AuthSession>();
        sessions.Add(Db.Sessions.Find((AuthSession? auth) => auth.RefreshToken == refreshToken));*/
        if (Db.Sessions.Any(session => session.RefreshToken == refreshToken))
        {
            var session = Db.Sessions.ToList()[0];
            var accessToken = Utils.generateToken();
            var newRefreshToken = Utils.generateToken();
            session.RefreshToken = newRefreshToken;
            session.AccessToken = accessToken;
            session.SessionStart = DateTime.Now;
            Db.Entry(session).State = EntityState.Modified;
            Db.SaveChanges();
            return new Dictionary<string, object>()
            {
                { "status", HTTPstatus.OK },
                {
                    "data", new Dictionary<string, object>()
                    {
                        { "accessToken", accessToken },
                        { "refreshToken", newRefreshToken },
                        {
                            "user", new Dictionary<string, int>()
                            {
                                { "id", session.UserId }
                            }
                        }
                    }
                }
            };
            
        }

        return new Dictionary<string, object>()
        {
            { "status", HTTPstatus.NOT_FOUND },
            {
                "data", new Dictionary<string, string>()
                {
                    { "message", "session not found" }
                }
            }
         
        };
    }

    public static Dictionary<string, object> sessionStart(string login, string password)
    {
        var users = new List<User>();

        users.Add(Db.Users.ToList().Find(user => user.Login == login));
        
        foreach (var user in users)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new Dictionary<string, object>()
                {
                    { "status", HTTPstatus.UNATHORIZED },
                    {
                        "data", new Dictionary<string, string>()
                        {
                            { "message", "incorrect password" }
                        }
                    }
                };
            }

            var userId = user.Id;
            var accessToken = Utils.generateToken();
            var refreshToken = Utils.generateToken();
            
            Utils.clearSessions(userId);
            
            Db.Sessions.Add(new AuthSession()
            {
                UserId = userId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                SessionStart = DateTime.Now
            });
            
            Db.SaveChanges();
            
            return new Dictionary<string, object>()
            {
                { "status", HTTPstatus.OK },
                {
                    "data", new Dictionary<string, object>()
                    {
                        { "accessToken", accessToken },
                        { "refreshToken", refreshToken },
                        { "userId", userId }
                    }
                }
            };

        }

        return new Dictionary<string, object>()
        {
            { "status", HTTPstatus.NOT_FOUND },
            {
                "data", new Dictionary<string, string>()
                {
                    { "message", "user not found" }
                }
            }
        };

    }
}