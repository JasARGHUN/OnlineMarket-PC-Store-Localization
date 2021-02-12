using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models.ViewModels;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ContactController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region CallBack
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CallBackViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CallBack.Add(model.CallBack);
                _unitOfWork.Save();
            }
            return RedirectToAction("CallBackCompleted");
        }

        public IActionResult CallBackCompleted()
        {
            return View();
        }
        #endregion
    }
}
