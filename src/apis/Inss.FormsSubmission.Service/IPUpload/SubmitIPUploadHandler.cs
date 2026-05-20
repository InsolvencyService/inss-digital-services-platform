using System.Security.Cryptography;
using System.Text;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IDynamicsStoreProvider _dynamicsStoreProvider;
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ReferenceNumberLength = 8;

    public SubmitIPUploadHandler(IMapperFactory mapperFactory, IDynamicsStoreProvider dynamicsStoreProvider)
    {
        _mapperFactory = mapperFactory;
        _dynamicsStoreProvider = dynamicsStoreProvider;
    }
    
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
        
        JsonMessage[] jsonMessages = CreateJsonMessages(request.FileContents);

        string reference = GenerateReferenceNumber();

        foreach (JsonMessage message in jsonMessages)
        {
            DynamicsSubmission submission = new()
            {
                Id = message.CorrelationId,
                Reference = reference,
                Json = message.Json,
                PayloadType = request.IsEmployeeUpload ? PayloadTypes.Employee : PayloadTypes.Employer,
                SubmissionTimestamp = DateTimeOffset.UtcNow,
                UserId = request.UserId
            };
            
            await _dynamicsStoreProvider.StoreAsync(submission);
        }
        
        return new SubmitIPUploadResponse { Reference = reference };
    }

    private JsonMessage[] CreateJsonMessages(string xml)
    {
        object model = FileHelper.GetRedundancyPaymentObject(xml);
        IMapper mapper = _mapperFactory.Create(model);
        return mapper.Map();
    }
    
    private static string GenerateReferenceNumber()
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