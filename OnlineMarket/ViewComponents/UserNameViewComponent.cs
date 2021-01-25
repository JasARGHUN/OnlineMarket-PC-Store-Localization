using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using System.Security.Claims;

namespace OnlineMarket.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserNameViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var model = _unitOfWork.User.GetFirstOrDefault(i => i.Id == claims.Value);

            return View(model);
        }
    }
}
