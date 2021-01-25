using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models
{
    public class AppInfo
    {
        public int AppInfoID { get; set; }

        [Required(ErrorMessage = "Please, enter Application name")]
        public string AppName { get; set; } //Application name.
        public string AppImage { get; set; } //Application home image.

    }
}
