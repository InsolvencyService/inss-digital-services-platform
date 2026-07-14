using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ValidationResultExtensions
{
    extension(List<ValidationResult> results)
    {
        public void AddResult(string message, string[] properties)
        {
            results.Add(new ValidationResult(message, properties));
        }
    }
}