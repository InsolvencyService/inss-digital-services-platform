using INSS.Platform.Web.Components.Models;
using INSS.Platform.Web.Components.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Forms.Web.TestHarness.Models
{
    public class AboutYou
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal Salary { get; set; }

        public YesNoType Employed { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Telephone { get; set; }

        public Address PostalAddress { get; set; }
    }
}
