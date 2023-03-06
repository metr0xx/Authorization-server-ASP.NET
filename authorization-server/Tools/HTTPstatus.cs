using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace authorization_server.Tools;

public class HTTPstatus
{
    public const int OK = 200;
    public const int BAD_REQUEST = 400;
    public const int UNATHORIZED = 401;
    public const int FORBIDDEN = 403;
    public const int NOT_FOUND = 404;
    public const int NOT_ALLOWED = 405;
    public const int INTERNAL_SERVER_ERROR = 500;
}