using System.Security.Cryptography;
using System.Text;

namespace Inss.FormsSubmission.Service.IPUpload;

internal static class ReferenceNumbers
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ReferenceNumberLength = 8;
    
    internal static string Generate()
    {
        StringBuilder result = new(ReferenceNumberLength);
        byte[] randomBytes = new byte[ReferenceNumberLength];

        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomBytes);
        
        foreach (byte currentRandomByte in randomBytes)
        {
            result.Append(AllowedChars[currentRandomByte % AllowedChars.Length]);
        }

        return result.ToString();
    }
}