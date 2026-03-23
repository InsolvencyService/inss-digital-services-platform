using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public sealed class SummaryModel : PageModel
{
    [JsonIgnore]
    public SummaryInfo[] Overview { get; set; } = [];
    
    public sealed class SummaryInfo
    {
        public required string Title { get; init; }
        
        public ContentPath? ChangeUrl { get; init; }
        
        public required string[] Values { get; init; }
    }
}