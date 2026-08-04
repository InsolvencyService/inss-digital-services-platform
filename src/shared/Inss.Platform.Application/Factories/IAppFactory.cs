using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Factories;

public interface IAppFactory
{
    ValueTask<AppModel> CreateAsync(SessionId session);
}