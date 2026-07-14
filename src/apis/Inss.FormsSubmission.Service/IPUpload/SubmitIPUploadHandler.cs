using System.Net;
using System.Text.Json;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.IPUpload.Clients;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;
using Inss.FormsSubmission.Service.IPUpload.Processing;
using Inss.FormsSubmission.Service.IPUpload.Services;

namespace Inss.FormsSubmission.Service.IPUpload;

public sealed class SubmitIPUploadHandler : IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IDynamicsStoreProvider _dynamicsStoreProvider;
    private readonly IBackgroundDynamicsQueue _backgroundDynamicsQueue;
    private readonly IDynamicsClient _dynamicsClient;
    private readonly INotifyEmailService _notifyEmailService;
    private readonly ILogger<SubmitIPUploadHandler> _logger;

    public SubmitIPUploadHandler(
        IMapperFactory mapperFactory, 
        IDynamicsStoreProvider dynamicsStoreProvider, 
        IBackgroundDynamicsQueue backgroundDynamicsQueue,
        IDynamicsClient dynamicsClient,
        INotifyEmailService notifyEmailService,
        ILogger<SubmitIPUploadHandler> logger)
    {
        _mapperFactory = mapperFactory;
        _dynamicsStoreProvider = dynamicsStoreProvider;
        _backgroundDynamicsQueue = backgroundDynamicsQueue;
        _dynamicsClient = dynamicsClient;
        _notifyEmailService = notifyEmailService;
        _logger = logger;
    }
    
    public async Task<SubmitIPUploadResponse> HandleAsync(SubmitIPUploadRequest request, CancellationToken cancellationToken)
    {
        JsonMessage[] jsonMessages = CreateJsonMessages(request.FileContents);

        string reference = ReferenceNumbers.Generate();

        await StoreMessageAsync(jsonMessages, reference, request.UserId, request.IsEmployeeUpload, cancellationToken);

        await SubmitMessagesToDynamicsAsync(jsonMessages, reference, request.UserId, request.IsEmployeeUpload, cancellationToken);
        
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

    private async Task SubmitMessagesToDynamicsAsync(
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

            _logger.UpdateSubmissionEmailReceipts();
            
            foreach (DynamicsSubmission submission in submissions)
            {
                await _dynamicsStoreProvider.StoreAsync(submission, cancellationToken);
            }
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
        dynamicsSubmission.ErrorInfo = submitResponse.Error is not null ? JsonSerializer.Deserialize<ErrorInfo>(submitResponse.Error) : null;

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

        _notifyEmailService.SendExternalEmail(userId, reference, submissionDate, isEmployeeSubmission, submissions);

        if (submissionFailed)
        {
            _notifyEmailService.SendInternalEmail(reference, submissionDate, isEmployeeSubmission, submissions);
        }

        return Task.CompletedTask;
    }
}