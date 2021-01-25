using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using System.Linq;


namespace OnlineMarket.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IUnitOfWork _repository;

        public NavigationMenuViewComponent(IUnitOfWork repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(_repository.Product.GetAll()
                .Select(x => x.Category.Name)
                .Distinct()
                .OrderBy(x => x));
        }
    }
}
