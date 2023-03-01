using authorization_server.Models;
using Microsoft.EntityFrameworkCore;
using authorization_server.Tools;

namespace authorization_server.DB;

public class Interaction
{
    static DBContext db = DBContext.db;
    
    public static Dictionary<string, object> Register(string login, string password)
    {       
        var users = db.Users.ToList();

        users.Add(db.Users.Find(  (User? user) => user.Login == login));
        if (users.Count > 0)
        {
            return new Dictionary<string, object>()
            {
                {"status", HTTPstatus.BAD_REQUEST},
                {"data", new Dictionary<string, string>()
                {
                    {"message", "already exists"}
                }}
            };
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User() { Login = login, Password = hashedPassword };
            db.Add(user);
            db.SaveChanges();
            
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
        var sessions = db.Sessions.ToList();
        sessions.Add(db.Sessions.Find((AuthSession? auth) => auth.RefreshToken == refreshToken));
        if (sessions.Count > 0)
        {
            var session = sessions[0];
            var accessToken = Utils.generateToken();
            var newRefreshToken = Utils.generateToken();
            session.RefreshToken = newRefreshToken;
            session.AccessToken = accessToken;
            session.SessionStart = DateTime.Now;
            db.Entry(session).State = EntityState.Modified;
            db.SaveChanges();
            return new Dictionary<string, object>()
            {
                { "status", HTTPstatus.OK },
                {
                    "data", new Dictionary<string, object>()
                    {
                        { "access_token", accessToken },
                        { "refresh_token", newRefreshToken },
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
        var users = db.Users.ToList();
        
        users.Add(db.Users.Find(  (User? user) => user.Login == login));

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
            db.Sessions.Add(new AuthSession()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userId,
                SessionStart = DateTime.Now
            });
            db.SaveChanges();
            return new Dictionary<string, object>()
            {
                { "status", HTTPstatus.OK },
                {
                    "data", new Dictionary<string, object>()
                    {
                        { "access_token", accessToken },
                        { "refresh_token", refreshToken },
                        { "user_id", userId }
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