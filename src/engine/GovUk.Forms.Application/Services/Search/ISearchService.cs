
namespace GovUk.Forms.Application.Services.Search;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(string searchText, int pageSize, int currentPageNumber);
}