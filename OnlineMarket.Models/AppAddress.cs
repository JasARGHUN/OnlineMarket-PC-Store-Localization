using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models
{
    public class AppAddress
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        public string Picture { get; set; }

        [MaxLength(200, ErrorMessage = "Only 200 characters")]
        public string Description { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
