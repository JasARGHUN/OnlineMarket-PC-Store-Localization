using Microsoft.AspNetCore.Mvc;

namespace OnlineMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {
        public ContactController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
