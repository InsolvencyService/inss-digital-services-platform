using Inss.Platform.RpsProvider.Domain.Enums;

namespace Inss.Platform.RpsProvider.Application.Services;

public interface ILoginService
{
    Task<RpsAuthenticationTypes> AuthenticateAsync(string email, string password);
}