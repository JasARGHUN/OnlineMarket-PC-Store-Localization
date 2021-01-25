using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models.ViewModels
{
    public class AppInfoEditViewModel : AppInfoCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please, enter Application name")]
        public string AppName { get; set; } //Application name.
        public string ExistingImagePath { get; set; }
    }
}
