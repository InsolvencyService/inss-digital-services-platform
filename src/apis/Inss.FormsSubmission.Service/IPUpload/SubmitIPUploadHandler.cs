using System.Net;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload.Clients;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;
using Inss.FormsSubmission.Service.IPUpload.Processing;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IDynamicsStoreProvider _dynamicsStoreProvider;
    private readonly IBackgroundDynamicsQueue _backgroundDynamicsQueue;
    private readonly IDynamicsClient _dynamicsClient;

    public SubmitIPUploadHandler(
        IMapperFactory mapperFactory, 
        IDynamicsStoreProvider dynamicsStoreProvider, 
        IBackgroundDynamicsQueue backgroundDynamicsQueue,
        IDynamicsClient dynamicsClient)
    {
        _mapperFactory = mapperFactory;
        _dynamicsStoreProvider = dynamicsStoreProvider;
        _backgroundDynamicsQueue = backgroundDynamicsQueue;
        _dynamicsClient = dynamicsClient;
    }
    
    public async Task<SubmitIPUploadResponse> HandleAsync(SubmitIPUploadRequest request)
    {
        // Purpose
        // Transpose xml into json objects
        // Generate reference
        // Save to storage (Cosmos/table)
        // At this point, return reference as we have persisted the json
        // Async enum the json and send to dynamics, updating each record with success/failure in storage
        // Finally send email to gov notify (another task)
        
        JsonMessage[] jsonMessages = CreateJsonMessages(request.FileContents);

        string reference = ReferenceNumbers.Generate();

        await StoreMessageAsync(jsonMessages, reference, request.UserId, request.IsEmployeeUpload);

        await SubmitMessagesToDynamics(jsonMessages, reference);
        
        return new SubmitIPUploadResponse { Reference = reference };
    }

    private JsonMessage[] CreateJsonMessages(string fileContents)
    {
        object model = FileHelper.GetRedundancyPaymentObject(fileContents);
        IMapper mapper = _mapperFactory.Create(model);
        return mapper.Map();
    }

    private async Task StoreMessageAsync(JsonMessage[] jsonMessages, string reference, string userId, bool isEmployeeUpload)
    {
        foreach (JsonMessage jsonMessage in jsonMessages)
        {
            DynamicsSubmission submission = new()
            {
                Id = jsonMessage.CorrelationId,
                Reference = reference,
                Json = jsonMessage.Json,
                PayloadType = isEmployeeUpload ? nameof(PayloadTypes.Employee) : nameof(PayloadTypes.Employer),
                SubmissionTimestamp = DateTimeOffset.UtcNow,
                UserId = userId
            };
            
            await _dynamicsStoreProvider.StoreAsync(submission);
        }
    }

    private async Task SubmitMessagesToDynamics(JsonMessage[] jsonMessages, string reference)
    {
        foreach (JsonMessage jsonMessage in jsonMessages)
        {
            await _backgroundDynamicsQueue.QueueAsync(async cancellationToken =>
            {
                Console.WriteLine("Submitting...");
                await Task.Delay(5000, cancellationToken);
                SubmitResponse submitResponse = await SubmitMessageToDynamicsAsync(jsonMessage, cancellationToken);
                await UpdateStoredMessageAsync(jsonMessage, reference, submitResponse, cancellationToken);
                await SendEmailAsync();
                Console.WriteLine("Submitted.");
            });
        }
    }

    private async Task<SubmitResponse> SubmitMessageToDynamicsAsync(JsonMessage jsonMessage, CancellationToken cancellationToken)
    {
        try
        {
            return await _dynamicsClient.SubmitAsync(jsonMessage, cancellationToken);
        }
        catch (Exception error)
        {
            return new SubmitResponse { StatusCode = HttpStatusCode.InternalServerError, Error = error.ToString() };
        }
    }
    
    private async Task UpdateStoredMessageAsync(JsonMessage jsonMessage, string reference, SubmitResponse submitResponse, CancellationToken cancellationToken)
    {
        DynamicsSubmission? dynamicsSubmission = await _dynamicsStoreProvider.GetAsync(jsonMessage.CorrelationId, reference);

        if (dynamicsSubmission is null)
        {
            // TODO: Log
            return;
        }

        dynamicsSubmission.StatusCode = submitResponse.StatusCode.ToString();
        dynamicsSubmission.ErrorInfo = submitResponse.Error;

        await _dynamicsStoreProvider.StoreAsync(dynamicsSubmission);
    }

    private static Task SendEmailAsync()
    {
        // TODO: Send email once outcome of https://inssdigital.atlassian.net/browse/MEDS-1019 determined and requirements defined
        Console.WriteLine("Sending email via Gov Notify...");
        return Task.CompletedTask;
    }
}