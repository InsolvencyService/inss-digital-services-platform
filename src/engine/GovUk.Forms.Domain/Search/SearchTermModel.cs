using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Search;

public class SearchTermModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }
}