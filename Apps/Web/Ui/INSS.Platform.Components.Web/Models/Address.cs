using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Components.Web.Models
{
    public class Address
    {
        [Required(ErrorMessage = "Enter address line 1")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(ErrorMessage = "Enter town or city")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Enter postcode")]
        public string Postcode { get; set; }
    }
}
