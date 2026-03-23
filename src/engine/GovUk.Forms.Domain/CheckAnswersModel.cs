using System.Text.Json.Serialization;

namespace GovUk.Forms.Domain;

public sealed class CheckAnswersModel : PageModel
{
    [JsonIgnore]
    public CheckAnswersItem[] Items { get; set; } = [];
    
    public string Information { get; init; } = "Check your answers before continuing";
    
    public sealed class CheckAnswersItem
    {
        public required string Title { get; init; }
        
        public required string ChangeUrl { get; init; }
        
        public required string[] Values { get; init; }
    }
}