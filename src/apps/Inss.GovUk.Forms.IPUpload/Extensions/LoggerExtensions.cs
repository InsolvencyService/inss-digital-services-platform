using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.IPUpload.Extensions;

[ExcludeFromCodeCoverage]
public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Loading the {File} as XML failed.")]
    public static partial void XmlLoadError(this ILogger logger, string file);
    
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Checking that the case reference {CaseReference} exists in RPS.")]
    public static partial void CheckCaseReferenceExists(this ILogger logger, string caseReference);
}