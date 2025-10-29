using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Web.Components.Models
{
    public class Address
    {
        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Postcode { get; set; }
    }
}
