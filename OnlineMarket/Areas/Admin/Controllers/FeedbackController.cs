using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Utility;
using ReflectionIT.Mvc.Paging;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
    public class FeedbackController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index(string filter, int page = 1, string sortExpression = "CallTime")
        {
            var qry = _unitOfWork.CallBack.GetAll().AsQueryable(); //_repository.Orders.AsNoTracking().Where(o => !o.Shipped);
            var model = PagingList.Create(qry, 20, page, sortExpression, "CallTime");

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> FeedBackDetails(int? id)
        {
            var product = await _unitOfWork.CallBack.Get(id.Value);

            if (product == null)
            {
                Response.StatusCode = 404;
                return View("ProductNotFound", id.Value);
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFeedBack(int id)
        {
            var model = await _unitOfWork.CallBack.Get(id);

            if (model != null)
            {
                TempData["message"] = $"{model.Id} was deleted";
            }

            await _unitOfWork.CallBack.Remove(model);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Marked(int id)
        {
            var model = _unitOfWork.CallBack
                .GetFirstOrDefault(x => x.Id == id);

            if (model != null)
            {
                model.Marked = true;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
