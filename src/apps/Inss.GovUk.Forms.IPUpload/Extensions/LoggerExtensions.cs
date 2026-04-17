using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.IPUpload.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Loading the {File} as XML failed.")]
    public static partial void XmlLoadError(this ILogger logger, string file);
}