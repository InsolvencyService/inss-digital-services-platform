using System.Security.Cryptography;
using System.Text;
using Inss.Common.IPUpload;

namespace Inss.FormsSubmission.Service.Handlers;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ReferenceNumberLength = 8;
    
    public async Task<SubmitIPUploadResponse> HandleAsync(SubmitIPUploadRequest request)
    {
        // Purpose
        // Transpose xml into json objects
        // Generate reference
        // Save to storage (Cosmos/table)
        // Delete from cosmos
        // At this point, return reference as we have persisted the json
        // Async enum the json and send to dynamics, updating each record with success/failure in storage
        // Finally send email to gov notify (another task)
        // Archive storage data from working to archive
        
        await Task.Delay(100);
        return new SubmitIPUploadResponse { Reference = GenerateReferenceNumber() };
    }
    
    private static string GenerateReferenceNumber()
    {
        StringBuilder result = new(ReferenceNumberLength);
        byte[] randomBytes = new byte[ReferenceNumberLength];
        
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(randomBytes);
        }
        
        foreach (byte currentRandomByte in randomBytes)
        {
            result.Append(AllowedChars[currentRandomByte % AllowedChars.Length]);
        }

        return result.ToString();
    }
}