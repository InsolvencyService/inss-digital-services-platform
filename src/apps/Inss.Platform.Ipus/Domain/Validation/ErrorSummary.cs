namespace Inss.Platform.Ipus.Domain.Validation;

public sealed class ErrorSummary
{
    public string Category { get; init; }

    public ErrorPropertySummary[] Properties { get; set; } = [];

    internal void AddProperty(ErrorPropertySummary propertySummary)
    {
        List<ErrorPropertySummary> propertySummaryList = [..Properties, propertySummary];
        Properties = propertySummaryList.ToArray();
    }
}