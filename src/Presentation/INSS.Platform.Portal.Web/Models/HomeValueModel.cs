using System.ComponentModel.DataAnnotations;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Models;

public class HomeValueModel : PageModel
{
    public HomeValueModel()
    {
        PathName = "home-value";
        Title = "Home Value";
        Controller = "HomeValue";
    }
    
    [Required(ErrorMessage = "Enter your home value")]
    [Range(100, 1_000_000, ErrorMessage = "The value must between £100 and £1,000,000")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Value { get; set; }
}