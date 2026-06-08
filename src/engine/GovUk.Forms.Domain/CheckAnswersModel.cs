using System.Text.Json.Serialization;

namespace GovUk.Forms.Domain;

public class CheckAnswersModel : PageModel
{
    [JsonIgnore]
    public CheckAnswersItem[] Items { get; set; } = [];
}