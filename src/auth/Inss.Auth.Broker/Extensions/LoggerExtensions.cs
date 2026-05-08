namespace Inss.Auth.Broker.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 300, Level = LogLevel.Error, Message = "User info endpoint failed validation: {Message}.")]
    public static partial void UserInfoError(this ILogger logger, string message);
    
    [LoggerMessage(EventId = 301, Level = LogLevel.Information, Message = "Logging out of broker for {Scheme} with post logout redirect {Redirect}.")]
    public static partial void SchemeLogout(this ILogger logger, string scheme, string redirect);
    
    [LoggerMessage(EventId = 302, Level = LogLevel.Error, Message = "Caller provided an invalid post logout redirect {Redirect}.")]
    public static partial void InvalidPostRedirectLogoutUrl(this ILogger logger, string redirect);
}