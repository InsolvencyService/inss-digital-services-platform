namespace Inss.Auth.RpsProvider.Domain;

public sealed class UserAuth
{
    public const string AuthCodeType = "userauth";
    
    public string Id { get; init; }
    
    public string CodeType { get; init; } = AuthCodeType;
    
    public required string Username { get; init; }
    
    public required string CodeChallenge { get; init; }
    
    public required string CodeChallengeMethod { get; init; }
    
    public required string RedirectUri { get; init; }
    
    public required string ClientId { get; init; }
}