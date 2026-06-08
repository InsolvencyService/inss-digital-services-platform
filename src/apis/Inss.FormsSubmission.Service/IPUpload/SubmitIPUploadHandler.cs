using System.Net;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload.Clients;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;
using Inss.FormsSubmission.Service.IPUpload.Processing;
using Notify.Client;
using Notify.Models.Responses;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IDynamicsStoreProvider _dynamicsStoreProvider;
    private readonly IBackgroundDynamicsQueue _backgroundDynamicsQueue;
    private readonly IDynamicsClient _dynamicsClient;
    private readonly ILogger<SubmitIPUploadHandler> _logger;

    public SubmitIPUploadHandler(
        IMapperFactory mapperFactory, 
        IDynamicsStoreProvider dynamicsStoreProvider, 
        IBackgroundDynamicsQueue backgroundDynamicsQueue,
        IDynamicsClient dynamicsClient,
        ILogger<SubmitIPUploadHandler> logger)
    {
        _mapperFactory = mapperFactory;
        _dynamicsStoreProvider = dynamicsStoreProvider;
        _backgroundDynamicsQueue = backgroundDynamicsQueue;
        _dynamicsClient = dynamicsClient;
        _logger = logger;
    }
    
    public async Task<SubmitIPUploadResponse> HandleAsync(SubmitIPUploadRequest request, CancellationToken cancellationToken)
    {
        JsonMessage[] jsonMessages = CreateJsonMessages(request.FileContents);

        string reference = ReferenceNumbers.Generate();

        await StoreMessageAsync(jsonMessages, reference, request.UserId, request.IsEmployeeUpload, cancellationToken);

        await SubmitMessagesToDynamics(jsonMessages, reference, request.IsEmployeeUpload, cancellationToken);
        
        return new SubmitIPUploadResponse { Reference = reference };
    }

    private JsonMessage[] CreateJsonMessages(string fileContents)
    {
        object model = FileHelper.GetRedundancyPaymentObject(fileContents);
        IMapper mapper = _mapperFactory.Create(model);
        return mapper.Map();
    }

    private async Task StoreMessageAsync(
        JsonMessage[] jsonMessages, 
        string reference, 
        string userId, 
        bool isEmployeeUpload, 
        CancellationToken cancellationToken)
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
            
            await _dynamicsStoreProvider.StoreAsync(submission, cancellationToken);
        }
    }

    private async Task SubmitMessagesToDynamics(
        JsonMessage[] jsonMessages, 
        string reference, 
        bool isEmployeeSubmission, 
        CancellationToken cancellationToken)
    {
        foreach (JsonMessage jsonMessage in jsonMessages)
        {
            await _backgroundDynamicsQueue.QueueAsync(async _ =>
            {
                _logger.SubmittingDynamicsMessage(jsonMessage.CorrelationId, reference);
                SubmitResponse submitResponse = await SubmitMessageToDynamicsAsync(jsonMessage, cancellationToken);

                _logger.UpdatingDynamicsResponseInStore(jsonMessage.CorrelationId, reference);
                await UpdateStoredMessageAsync(jsonMessage, reference, submitResponse, cancellationToken);
            });
        }
        
        // TODO: Determine overall success status
        
        _logger.SendingGovNotifyEmail(reference);
        await SendEmailAsync(reference, isEmployeeSubmission);
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
    
    private async Task UpdateStoredMessageAsync(
        JsonMessage jsonMessage, 
        string reference,
        SubmitResponse submitResponse, 
        CancellationToken cancellationToken)
    {
        DynamicsSubmission? dynamicsSubmission = await _dynamicsStoreProvider.GetAsync(
            jsonMessage.CorrelationId, reference, cancellationToken);

        if (dynamicsSubmission is null)
        {
            _logger.StoredMessageNotFound(jsonMessage.CorrelationId, reference);
            return;
        }

        dynamicsSubmission.StatusCode = submitResponse.StatusCode.ToString();
        dynamicsSubmission.ErrorInfo = submitResponse.Error;

        await _dynamicsStoreProvider.StoreAsync(dynamicsSubmission, cancellationToken);
    }

    private static Task SendEmailAsync(string reference, bool isEmployeeSubmission)
    {
        Dictionary<String, dynamic> personalisation = new()
        {
            {"formType", isEmployeeSubmission ? "RP14A" : "RP14"},
            {"referenceNumber", reference},
            {"succeeded/failed", "succeeded"}, // OR failed
            {"uploadDateAndTime", ""},
            {"rejectedState", ""}
        };
        NotificationClient client = new("api-key");
        EmailNotificationResponse response = client.SendEmail("email-address", "template-from-config", personalisation);

        if (string.IsNullOrWhiteSpace(response.id))
        {
            // Handle error
        }
        
        // TODO: Send email once outcome of https://inssdigital.atlassian.net/browse/MEDS-1019 determined and requirements defined
        return Task.CompletedTask;
    }
}