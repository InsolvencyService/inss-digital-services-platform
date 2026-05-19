namespace Inss.FormsSubmission.Service.Handlers;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}