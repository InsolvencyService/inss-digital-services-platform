using System.Security.Claims;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Inss.Auth.Broker.Domain;

public sealed class AuthCode
{
    public const string AuthCodeType = "authcode";
    
    public string Id { get; init; }

    public string CodeType { get; init; } = AuthCodeType;
    
    public byte[] Principal { get; set; }
    
    public string CodeChallenge { get; init; }
    
    public string CodeChallengeMethod { get; init; }

    public void AddClaimsPrincipal(ClaimsPrincipal principal)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        principal.WriteTo(writer);
        Principal = stream.ToArray();
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        using MemoryStream stream = new(Principal);
        using BinaryReader reader = new(stream);
        return new ClaimsPrincipal(reader);
    }
}