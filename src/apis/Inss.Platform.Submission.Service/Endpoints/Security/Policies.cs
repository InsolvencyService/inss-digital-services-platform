using Microsoft.AspNetCore.Authorization;

namespace Inss.Platform.Submission.Service.Endpoints.Security;

public static class Policies
{
    public const string SubmissionPolicy = "Submission";
    
    extension(AuthorizationBuilder builder)
    {
        public AuthorizationBuilder AddSubmissionPolicy()
        {
            return builder.AddPolicy(SubmissionPolicy, policy => policy.RequireClaim("submission", "submit"));
        }
    }
}