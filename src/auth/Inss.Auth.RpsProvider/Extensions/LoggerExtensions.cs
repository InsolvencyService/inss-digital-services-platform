namespace Inss.Auth.RpsProvider.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 400, Level = LogLevel.Information, Message = "Logging out of RPS with post logout redirect {Redirect}.")]
    public static partial void RpsLogout(this ILogger logger, string redirect);
    
    [LoggerMessage(EventId = 401, Level = LogLevel.Error, Message = "Caller provided an invalid post logout redirect {Redirect}.")]
    public static partial void InvalidPostRedirectLogoutUrl(this ILogger logger, string redirect);
    
    [LoggerMessage(EventId = 402, Level = LogLevel.Information, Message = "Received a {StatusCode} response and redirect {RedirectUrl} from RPS.")]
    public static partial void LoginResponse(this ILogger logger, string statusCode, string redirectUrl);
    
    [LoggerMessage(EventId = 403, Level = LogLevel.Information, Message = "The login was successful.")]
    public static partial void LoginSuccessResponse(this ILogger logger);
    
    [LoggerMessage(EventId = 404, Level = LogLevel.Warning, Message = "The login failed due to invalid or unknown credentials.")]
    public static partial void LoginFailedResponse(this ILogger logger);
    
    [LoggerMessage(EventId = 405, Level = LogLevel.Warning, Message = "The login failed the account has been locked.")]
    public static partial void LoginAccountLockedResponse(this ILogger logger);
    
    [LoggerMessage(EventId = 406, Level = LogLevel.Error, Message = "The login failed with an unexpected response.")]
    public static partial void LoginUnknownFailureResponse(this ILogger logger);
    
    [LoggerMessage(EventId = 407, Level = LogLevel.Error, Message = "The login returned an unexpected response: {Body}")]
    public static partial void LoginUnexpectedResponseResponse(this ILogger logger, string body);
}