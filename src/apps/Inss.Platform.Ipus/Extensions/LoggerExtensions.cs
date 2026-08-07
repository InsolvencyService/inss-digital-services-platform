using System.Diagnostics.CodeAnalysis;

namespace Inss.Platform.Ipus.Extensions;

[ExcludeFromCodeCoverage]
public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Loading the {File} as XML failed.")]
    public static partial void XmlLoadError(this ILogger logger, string file);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Looking up the case reference {CaseReference} in Dynamics.")]
    public static partial void LookupCaseDetailsExists(this ILogger logger, string caseReference);
}