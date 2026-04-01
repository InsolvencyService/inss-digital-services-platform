using System.Security.Claims;

namespace Inss.Auth.Broker.Domain;

public sealed class AuthCode
{
    public ClaimsPrincipal Principal { get; init; }
    
    public string CodeChallenge { get; init; }
    
    public string CodeChallengeMethod { get; init; }
}