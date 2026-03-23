using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class AddAnotherModel : PageModel
{
    [Copyable]
    public bool AddAnotherItem { get; set; }
    
    public PageModelList Items { get; init; } = [];

    [Copyable]
    public string? Question { get; init; }

    [Copyable]
    public string? Hint { get; init; }

    [Copyable]
    public int GroupLength { get; set; }
    
    [JsonIgnore]
    public AddAnotherSummaryModel[] SummaryInfo { get; set; } = [];
    
    public sealed class AddAnotherSummaryModel
    {
        public required string Value { get; init; }
        
        public required string ChangeUrl { get; init; }
        
        public required string RemoveUrl { get; init; }
    }
}

