using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 700, Level = LogLevel.Error, Message = "Azure Search request has failed. Status Code:  {StatusCode}, SearchText: {SearchText}")]
    public static partial void AzureSearchFailed(this ILogger logger, int statusCode, string searchText);
    
    [LoggerMessage(EventId = 701, Level = LogLevel.Error, Message = "\"Unable to load configuration file '{FileName}'.\"")]
    public static partial void SearchConfigMissing(this ILogger logger, string filename);
}