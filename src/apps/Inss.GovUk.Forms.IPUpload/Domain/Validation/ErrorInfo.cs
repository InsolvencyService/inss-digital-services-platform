namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorInfo : ErrorInfoHeader
{
    private readonly List<ErrorIdentifier> _identifiers = [];

    public string Id { get; } = Guid.NewGuid().ToString();

    public int Count => _identifiers.Count;

    public ErrorIdentifier[] Identifiers => _identifiers.ToArray();
    
    public void AddIdentifier(string firstName, string surname, DateOnly dob, string nino, string value)
    {
        _identifiers.Add(new ErrorIdentifier { FirstName = firstName, Surname = surname, Dob = dob, Nino = nino, Value = value });
    }
}