using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services.Search;

public interface ISearchConfigProvider
{
    SearchModel LoadSearchConfig();
}
