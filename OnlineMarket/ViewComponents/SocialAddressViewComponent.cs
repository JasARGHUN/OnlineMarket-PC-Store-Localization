using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;

namespace OnlineMarket.ViewComponents
{
    public class SocialAddressViewComponent : ViewComponent
    {
        private IAppDataRepository _context;

        public SocialAddressViewComponent(IAppDataRepository context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View(_context.GetAllAppSocialAddress());
        }
    }
}
