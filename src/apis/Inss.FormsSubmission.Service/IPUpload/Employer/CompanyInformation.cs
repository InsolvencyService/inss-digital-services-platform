using System.Text.Json.Serialization;

namespace Inss.FormsSubmission.Service.IPUpload.Employer;

public class CompanyInformation
{
    [JsonPropertyName("correlation_id")]
    public Guid CorrelationId { get; init; }

    [JsonPropertyName("case_reference")]
    public string CaseReference { get; init; }

    [JsonPropertyName("company")]
    public Company Company { get; init; }

    [JsonPropertyName("paye")]
    public Paye Paye { get; init; }

    [JsonPropertyName("directors")]
    public List<Director> Directors { get; init; }

    [JsonPropertyName("directors_claiming_redundancy")]
    public bool? DirectorsClaimingRedundancy { get; init; }

    [JsonPropertyName("shareholders")]
    public List<Shareholder> Shareholders { get; init; }

    [JsonPropertyName("legally_associated_companies")]
    public bool? LegallyAssociatedCompanies { get; init; }

    [JsonPropertyName("associated_companies")]
    public List<AssociatedCompany> AssociatedCompanies { get; init; }

    [JsonPropertyName("employees")]
    public Employees Employees { get; init; }

    [JsonPropertyName("insolvency")]
    public Insolvency Insolvency { get; init; }

    [JsonPropertyName("transfer")]
    public Transfer Transfer { get; init; }

    [JsonPropertyName("pay_records_contact")]
    public PayRecordsContact PayRecordsContact { get; init; }

    [JsonPropertyName("insolvency_practitioner")]
    public InsolvencyPractitioner InsolvencyPractitioner { get; init; }
}