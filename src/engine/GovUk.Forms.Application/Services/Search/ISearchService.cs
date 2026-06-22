using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services.Search;

public interface ISearchService
{
    Task<SearchResult[]> SearchAsync(string searchText);
}