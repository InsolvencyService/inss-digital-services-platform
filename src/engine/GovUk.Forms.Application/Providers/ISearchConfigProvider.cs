using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.Providers;

public interface ISearchConfigProvider
{
    SearchModel LoadConfig();
}
