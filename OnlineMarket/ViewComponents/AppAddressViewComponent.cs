using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;

namespace OnlineMarket.ViewComponents
{
    public class AppAddressViewComponent : ViewComponent
    {
        private readonly IAppAddressRepository _repository;

        public AppAddressViewComponent(IAppAddressRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            return View(_repository.GetAllAppAddress());
        }
    }
}
