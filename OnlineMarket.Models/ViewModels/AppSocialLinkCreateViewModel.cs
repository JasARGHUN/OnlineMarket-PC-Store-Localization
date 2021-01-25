using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace OnlineMarket.Models.ViewModels
{
    public class AppSocialLinkCreateViewModel
    {
        [Required(ErrorMessage = "Enter URL address")]
        public string UrlAddress { get; set; }
        public List<IFormFile> AppSocialImgs { get; set; } //Application social image.
    }
}
