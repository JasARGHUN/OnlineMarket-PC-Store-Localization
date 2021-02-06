using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using OnlineMarket.Models.ViewModels;
using OnlineMarket.Utility;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHtmlLocalizer<HomeController> _localizer;

        public int pageSize = 9; // Object count in 1 page.

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork,
            IHtmlLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        #region ReflectionIT Pagination
        /* Index(string filter, string category, int page = 1)
        public IActionResult Index(string filter, string category, int page = 1)
        {
            //IEnumerable<Product> itemList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            // includeProperties: "Category" automatically include a Category in a Product
            var qry = _unitOfWork.Product.GetAll(includeProperties: "Category");

            if (!string.IsNullOrWhiteSpace(filter))
            {
                qry = qry.Where(p => p.Name.Contains(filter));
            }

            var itemList = PagingList.Create(qry, 3, page);
            
            itemList.RouteValue = new RouteValueDictionary {{ "filter", filter}};

            // Refresh Shopping Cart section begin.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                // Configure Sessions for Cart
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);
            }

            // Refresh Shopping Cart section end.

            return View(itemList);
        }
        */
        #endregion

        public IActionResult Index(int? product, string name, string category, int page = 1,
            SortState sortOrder = SortState.NameAsc)
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").Where(p => category == null || p.Category.Name == category)
                .OrderBy(p => p.Id);

            if (product != null && product != 0)
            {
                productList = productList.Where(p => p.Id == product);
            }
            if (!String.IsNullOrEmpty(name))
            {
                var value = name.ToUpper();
                productList = productList.Where(p => p.Name.Contains(value));
            }

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    productList = productList.OrderByDescending(s => s.Name); break;
                case SortState.PriceAsc:
                    productList = productList.OrderBy(s => s.Price); break;
                case SortState.PriceDesc:
                    productList = productList.OrderByDescending(s => s.Price); break;
                case SortState.CategoryAsc:
                    productList = productList.OrderBy(s => s.Category.Name); break;
                case SortState.CategoryDesc:
                    productList = productList.OrderByDescending(s => s.Category.Name); break;
                default:
                    productList = productList.OrderBy(s => s.Name); break;
            }

            var counts = productList.Count();
            var items = productList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PagingInfo pagingInfo = new PagingInfo(counts, page, pageSize);

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);
            }

            ProductViewModel productsListView = new ProductViewModel
            {
                PagingInfo = pagingInfo,
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(_unitOfWork.Product.GetAll().ToList(), product, name),
                Products = items,
                Categories = _unitOfWork.Category.GetAll()
            };

            return View(productsListView);
        }

        public IActionResult Details(int? id)
        {
            // includeProperties: "Category" automatically include a Category in a Product
            var model = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, includeProperties: "Category");

            ShoppingCart cartModel = new ShoppingCart()
            {
                Product = model,
                ProductId = model.Id
            };

            return View(cartModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // ?
        public async Task<IActionResult> Details(ShoppingCart item)
        {
            item.Id = 0;

            if (ModelState.IsValid)
            {
                // Add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                item.ApplicationUserId = claim.Value;

                // includeProperties: "Product" automatically include a Product in a ShoppingCart
                ShoppingCart cartModel = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == item.ApplicationUserId &&
                    x.ProductId == item.ProductId, includeProperties: "Product");

                if(cartModel == null)
                {
                    // No records exists in database for that product for that user
                    await _unitOfWork.ShoppingCart.Add(item);
                }
                else
                {
                    cartModel.Count += item.Count;
                    //_unitOfWork.ShoppingCart.Update(cartModel);
                }

                _unitOfWork.Save();

                // Configure Sessions for Cart
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == item.ApplicationUserId).ToList().Count();

                //HttpContext.Session.SetObject(SD.SessionShoppingCart, item);
                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);

                TempData[SD.Success] = $"{_localizer["Product added to basket"].Value}";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // includeProperties: "Category" automatically include a Category in a Product
                var model = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == item.ProductId, includeProperties: "Category");

                ShoppingCart cartModel = new ShoppingCart()
                {
                    Product =  model,
                    ProductId = model.Id
                };

                TempData[SD.Success] = $"{cartModel.Product.Name} {_localizer["added from backet"].Value}";

                return View(cartModel);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult EmptyPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.Now.AddDays(30)
                });

            return LocalRedirect(returnUrl);
        }
    }
}