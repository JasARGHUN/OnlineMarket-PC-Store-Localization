using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace OnlineMarket.Models.ViewModels
{
    public class AppInfoCreateViewModel
    {
        public List<IFormFile> AppImages { get; set; } //Application image.
    }
}
