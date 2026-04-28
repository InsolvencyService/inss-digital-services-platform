// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorInfo : ErrorInfoHeader
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public int Count => Identifiers.Length;

    public ErrorIdentifier[] Identifiers { get; set; } = [];
    
    public void AddIdentifier(string firstName, string surname, DateOnly dob, string nino, string value)
    {
        List<ErrorIdentifier> identifiers =
        [
            ..Identifiers,
            new() { FirstName = firstName, Surname = surname, Dob = dob, Nino = nino, Value = value }
        ];
        Identifiers = identifiers.ToArray();
    }
}