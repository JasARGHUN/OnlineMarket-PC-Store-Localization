using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;

namespace OnlineMarket.ViewComponents
{
    public class AppNameOnlyViewComponent : ViewComponent
    {
        private readonly IInfoRepository _infoRepository;
        public AppNameOnlyViewComponent(IInfoRepository infoRepository)
        {
            _infoRepository = infoRepository;
        }
        public IViewComponentResult Invoke()
        {
            var info = _infoRepository.GetInfo(1);
            return View(info);
        }
    }
}
