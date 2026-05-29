namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorSummary
{
    public string Category { get; init; }

    public ErrorPropertySummary[] Properties { get; set; } = [];

    public void AddProperty(ErrorPropertySummary propertySummary)
    {
        List<ErrorPropertySummary> propertySummaryList = [..Properties, propertySummary];
        Properties = propertySummaryList.ToArray();
    }
}