using System.Net;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload.Clients;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;
using Inss.FormsSubmission.Service.IPUpload.Processing;
using Inss.FormsSubmission.Service.Options;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IDynamicsStoreProvider _dynamicsStoreProvider;
    private readonly IBackgroundDynamicsQueue _backgroundDynamicsQueue;
    private readonly IDynamicsClient _dynamicsClient;
    private readonly INotificationClient _notificationClient;
    private readonly IOptions<NotifyOptions> _notifyOptions;
    private readonly ILogger<SubmitIPUploadHandler> _logger;

    public SubmitIPUploadHandler(
        IMapperFactory mapperFactory, 
        IDynamicsStoreProvider dynamicsStoreProvider, 
        IBackgroundDynamicsQueue backgroundDynamicsQueue,
        IDynamicsClient dynamicsClient,
        INotificationClient notificationClient,
        IOptions<NotifyOptions> notifyOptions,
        ILogger<SubmitIPUploadHandler> logger)
    {
        _mapperFactory = mapperFactory;
        _dynamicsStoreProvider = dynamicsStoreProvider;
        _backgroundDynamicsQueue = backgroundDynamicsQueue;
        _dynamicsClient = dynamicsClient;
        _notificationClient = notificationClient;
        _notifyOptions = notifyOptions;
        _logger = logger;
    }
    
    public async Task<SubmitIPUploadResponse> HandleAsync(SubmitIPUploadRequest request, CancellationToken cancellationToken)
    {
        JsonMessage[] jsonMessages = CreateJsonMessages(request.FileContents);

        string reference = ReferenceNumbers.Generate();

        await StoreMessageAsync(jsonMessages, reference, request.UserId, request.IsEmployeeUpload, cancellationToken);

        await SubmitMessagesToDynamics(jsonMessages, reference, request.UserId, request.IsEmployeeUpload, cancellationToken);
        
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
        string userId,
        bool isEmployeeSubmission, 
        CancellationToken cancellationToken)
    {
        DateTimeOffset submissionDate = TimeProvider.System.GetUtcNow();
        
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
        
        await _backgroundDynamicsQueue.QueueAsync(async _ =>
        {
            _logger.LoadSubmittedDynamicsMessages(reference);
            DynamicsSubmission[] submissions = await _dynamicsStoreProvider.GetByReferenceAsync(reference, cancellationToken);
            
            _logger.SendingGovNotifyEmail(reference);
            await SendEmailAsync(reference, userId, submissionDate, isEmployeeSubmission, submissions);
        });
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

    private Task SendEmailAsync(
        string reference, 
        string userId,
        DateTimeOffset submissionDate, 
        bool isEmployeeSubmission, 
        DynamicsSubmission[] submissions)
    {
        bool submissionFailed = submissions.Any(s => s.ErrorInfo is not null);
        Dictionary<String, dynamic> personalisation = GetPersonalisation(reference, submissionDate, isEmployeeSubmission, submissionFailed);

        SendEmail(userId, reference, personalisation);

        if (submissionFailed && _notifyOptions.Value.IPUploadBccEmail is not null)
        {
            SendEmail(_notifyOptions.Value.IPUploadBccEmail, reference, personalisation);
        }

        return Task.CompletedTask;
    }

    private void SendEmail(string email, string reference, Dictionary<String, dynamic> personalisation)
    {
        EmailNotificationResponse response = _notificationClient.SendEmail(email, _notifyOptions.Value.IPUploadTemplateId, personalisation);

        if (string.IsNullOrWhiteSpace(response.id))
        {
            _logger.EmailFailed(reference);
        }
    }
    
    private static Dictionary<String, dynamic> GetPersonalisation(
        string reference,
        DateTimeOffset submissionDate,
        bool isEmployeeSubmission,
        bool submissionFailed)
    {
        return new Dictionary<String, dynamic>
        {
            {"formType", isEmployeeSubmission ? "RP14A" : "RP14"},
            {"referenceNumber", reference},
            {"succeeded/failed", submissionFailed ? "failed" : "succeeded"},
            {"uploadDateAndTime", $"{submissionDate:F}"},
            {"rejectedState", ""} // TODO:
        };
    }
}