namespace Inss.Auth.Broker.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 300, Level = LogLevel.Error, Message = "User info endpoint failed validation: {Message}.")]
    public static partial void UserInfoError(this ILogger logger, string message);
    
    [LoggerMessage(EventId = 301, Level = LogLevel.Information, Message = "Logging out of broker for {Scheme} with post logout redirect {Redirect}.")]
    public static partial void SchemeLogout(this ILogger logger, string scheme, string redirect);
    
    [LoggerMessage(EventId = 302, Level = LogLevel.Error, Message = "Caller provided an invalid post logout redirect {Redirect}.")]
    public static partial void InvalidPostRedirectLogoutUrl(this ILogger logger, string redirect);
    
    [LoggerMessage(EventId = 303, Level = LogLevel.Information, Message = "Performing a connect/authorize with {Issuer}, {ClientId}, " +
                                                                          "{RedirectUrl}, {LoginHint}, {CodeChallenge}, {CodeChallengeMethod}.")]
    public static partial void AuthorizeInfo(
        this ILogger logger, string issuer, string clientId, string redirectUrl, 
        string loginHint, string codeChallenge, string codeChallengeMethod);
    
    [LoggerMessage(EventId = 304, Level = LogLevel.Information, Message = "Callback to the connect/callback succeeded.")]
    public static partial void ConnectCallbackSucceeded(this ILogger logger);
    
    [LoggerMessage(EventId = 305, Level = LogLevel.Error, Message = "Unable to handle the callback. Cookies are {Cookies}")]
    public static partial void ConnectCallbackFailed(this ILogger logger, string cookies);
    
    [LoggerMessage(EventId = 306, Level = LogLevel.Information, Message = "Callback client redirect exists")]
    public static partial void ConnectCallbackRedirectExists(this ILogger logger);
    
    [LoggerMessage(EventId = 307, Level = LogLevel.Information, Message = "Storing auth code resolved with {Id}.")]
    public static partial void ConnectStoreAuthCodeInfo(this ILogger logger, string id);
    
    [LoggerMessage(EventId = 308, Level = LogLevel.Information, Message = "Attempting to retrieve the auth code with {Code}.")]
    public static partial void RetrieveAuthCodeInfo(this ILogger logger, string code);
}