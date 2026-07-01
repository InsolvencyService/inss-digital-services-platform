using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = "Loading page {PagePath} in section {SectionTitle}.")]
    public static partial void LoadingPage(this ILogger logger, ContentPath pagePath, string sectionTitle);
    
    [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "Overring page {PagePath} in section {SectionTitle} with {OverridePath}.")]
    public static partial void OverridingPagePath(this ILogger logger, ContentPath pagePath, string sectionTitle, string overridePath);
    
    [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "Validating page {PagePath}.")]
    public static partial void ValidatingPage(this ILogger logger, ContentPath pagePath);
    
    [LoggerMessage(EventId = 103, Level = LogLevel.Information, Message = "Processing page {PagePath} in section {SectionTitle}.")]
    public static partial void ProcessingPage(this ILogger logger, ContentPath pagePath, string sectionTitle);
    
    [LoggerMessage(EventId = 104, Level = LogLevel.Warning, Message = "Azure Search request sent with an empty search text.")]
    public static partial void MissingSearchText(this ILogger logger);
    
    [LoggerMessage(EventId = 105, Level = LogLevel.Error, Message = "Azure Search request made with invalid page size : {PageSize}.")]
    public static partial void InvalidSearchPageSize(this ILogger logger, int pageSize);
    
    [LoggerMessage(EventId = 106, Level = LogLevel.Error, Message = "Azure Search request sent with invalid current page number: {CurrentPageNumber}")]
    public static partial void InvalidCurrentPageNumber(this ILogger logger, int currentPageNumber);
}