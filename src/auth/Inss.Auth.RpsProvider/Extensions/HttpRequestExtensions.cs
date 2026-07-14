namespace Inss.Auth.RpsProvider.Extensions;

public static class HttpRequestExtensions
{
    extension(HttpRequest request)
    {
        public string GetForwardedHost()
        {
            string? scheme = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            string? host = request.Headers["X-Forwarded-Host"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(scheme) && !string.IsNullOrWhiteSpace(host))
            {
                return $"{scheme}://{host}";
            }

            // Fallback to the request hosting domain e.g. localhost or azurewebsites.net
            return $"{request.Scheme}://{request.Host}";
        }
    }
}