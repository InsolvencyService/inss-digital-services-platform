namespace Inss.Platform.Ipus.Application.Shared;

public record CaseReferenceRequest(string CaseReference);

public record CaseReferenceResponse(string CaseReference, string CompanyName);