using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models
{
    public class AppSocialAddress
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter URL address")]
        public string UrlAddress { get; set; }
        public string AppSocialImg { get; set; } //Application social image.
    }
}
