namespace Inss.Auth.RpsProvider.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 400, Level = LogLevel.Information, Message = "Logging out of RPS with post logout redirect {Redirect}.")]
    public static partial void RpsLogout(this ILogger logger, string redirect);
    
    [LoggerMessage(EventId = 401, Level = LogLevel.Error, Message = "Caller provided an invalid post logout redirect {Redirect}.")]
    public static partial void InvalidPostRedirectLogoutUrl(this ILogger logger, string redirect);
}