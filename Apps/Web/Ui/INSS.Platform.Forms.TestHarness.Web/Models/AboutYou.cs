using INSS.Platform.Components.Web.Models;
using INSS.Platform.Components.Web.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Forms.TestHarness.Web.Models
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

        public Address PostalAddress { get; set; } = new Address();
    }
}
