using Microsoft.Extensions.Logging;

namespace Inss.Platform.Application.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Warning, Message = "Azure Search request sent with an empty search text.")]
    public static partial void MissingSearchText(this ILogger logger);
    
    [LoggerMessage(EventId = 101, Level = LogLevel.Error, Message = "Azure Search request made with invalid page size : {PageSize}.")]
    public static partial void InvalidSearchPageSize(this ILogger logger, int pageSize);

    [LoggerMessage(EventId = 102, Level = LogLevel.Error, Message = "Azure Search request sent with invalid current page number: {CurrentPageNumber}")]
    public static partial void InvalidCurrentPageNumber(this ILogger logger, int currentPageNumber);

    [LoggerMessage(EventId = 103, Level = LogLevel.Warning, Message = "Azure Search request sent with an empty search key and/or value.")]
    public static partial void MissingSearchKeyAndOrValue(this ILogger logger);

    [LoggerMessage(EventId = 104, Level = LogLevel.Warning, Message = "Unable to find column Azure search field '{FieldName}'.")]
    public static partial void SearchConfigAndResultMismatch(this ILogger logger, string fieldName);
}