namespace Inss.Auth.Broker.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 300, Level = LogLevel.Error, Message = "User info endpoint failed validation: {Message}.")]
    public static partial void UserInfoError(this ILogger logger, string message);
}