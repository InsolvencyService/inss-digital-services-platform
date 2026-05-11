using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Extensions;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.IPUpload.Application.Services;

public sealed class SubmitUploadedXmlService : ISubmitUploadedXmlService
{
    private readonly ISubmitIPUploadSectionClient _submitIPUploadSectionClient;
    private readonly ILogger<SubmitUploadedXmlService> _logger;
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ReferenceNumberLength = 8;

    public SubmitUploadedXmlService(ISubmitIPUploadSectionClient submitIPUploadSectionClient, ILogger<SubmitUploadedXmlService> logger)
    {
        _submitIPUploadSectionClient = submitIPUploadSectionClient;
        _logger = logger;
    }

    public Task<string> SubmitAsync(SectionModel section, string userId)
    {
        // Process
        // 1. Generate a reference number (c# RandomNumberGenerator probability of clashes when it generates an 10 character alphanumeric string with only uppercase)
        string reference = GenerateReferenceNumber();
        
        // 2. XML split into multiple JSON objects. This will give us a correlation Id for each JSON object
        // 3. Table row entry added for each JSON using the RK (correlation Id) and PK (reference)
        // 4. Delete form from CosmosDb
        
        _ = ProcessAsync(reference); // Do not await this. Let the process go back to the caller

        return Task.FromResult(reference);
    }
    
    private async Task ProcessAsync(string reference)
    {
        // TODO: Enumerate the JSON passed into this method 
        for (int i = 0; i < 10; i++)
        {
            // 5. Enumerate each and send to Dynamics async and return to final page - do not wait for responses. Will need wrapper to help
            // 6. As each call to Dynamics completes, update the table row with the status/error info
            await _submitIPUploadSectionClient.SubmitAsync(new SectionModel(), ""); // TODO Send the correct parameters
            await Task.Delay(100);
            _logger.SubmittingUploadToDynamics(reference, i.ToString(CultureInfo.CurrentCulture));
        }
        
        // 7. Review results in table storage and notify the user
        
        // 8. Archive the results into an archive table store
    }
    
    private static string GenerateReferenceNumber()
    {
        // Generates an 8 char alphanumeric reference number
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