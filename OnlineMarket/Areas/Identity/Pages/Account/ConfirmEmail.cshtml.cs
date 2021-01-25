using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace OnlineMarket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHtmlLocalizer<ConfirmEmailModel> _localizer;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager,
            IHtmlLocalizer<ConfirmEmailModel> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            var notFound = _localizer["Unable to load user with ID"];
            if (user == null)
            {
                return NotFound($"{notFound} '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            var statusMessageSuccsess = _localizer["Thank you for confirming your email."];
            var statusMessageError = _localizer["Error confirming your email."];

            StatusMessage = result.Succeeded ? $"{statusMessageSuccsess}" : $"{statusMessageError}";
            return Page();
        }
    }
}
