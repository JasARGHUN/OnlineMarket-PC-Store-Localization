﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using OnlineMarket.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using OnlineMarket.Utility;
using OnlineMarket.DataAccess.Data;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHost;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }

        // GET: Admin/Product
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Index(string filter, int page = 1, string sortExpression = "Name")
        {
            var qry = _unitOfWork.Product.GetAll(includeProperties: "Category");

            if (!string.IsNullOrWhiteSpace(filter))
            {
                qry = qry.Where(p => p.Name.Contains(filter));
            }

            var model = PagingList.Create(qry, 20, page, sortExpression, "Name");

            model.RouteValue = new RouteValueDictionary { { "filter", filter } };
            model.Action = "Index";

            return View(model);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductViewModel item = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                // This code for create
                return View(item);
            }

            // This code for edit
            item.Product = await _unitOfWork.Product.Get(id.GetValueOrDefault());

            if (item.Product == null)
            {
                return NotFound();
            }
            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel item)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _webHost.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if(item.Product.ImageUrl != null)
                    {
                        // Update data with image
                        var imagePath = Path.Combine(webRootPath, item.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using(var fileStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        await files[0].CopyToAsync(fileStreams);
                    }

                    item.Product.ImageUrl = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    // Update data without update image
                    if(item.Product.Id != 0)
                    {
                        Product model = await _unitOfWork.Product.Get(item.Product.Id);
                        item.Product.ImageUrl = model.ImageUrl;
                    }
                }

                if (item.Product.Id == 0)
                {
                    await _unitOfWork.Product.Add(item.Product);
                }
                else
                {
                    await _unitOfWork.Product.UpdateAsync(item.Product);
                }

                _unitOfWork.Save();

                TempData[SD.Success] = $"Completed!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                item.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

                if(item.Product.Id != 0)
                {
                    item.Product = await _unitOfWork.Product.Get(item.Product.Id);
                }
            }

            return View(item);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category").OrderByDescending(x => x.Id);
            return Json(new { data = allObj });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _unitOfWork.Product.Get(id);

            if (model == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string webRootPath = _webHost.WebRootPath;

            var imagePath = Path.Combine(webRootPath, model.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            await _unitOfWork.Product.Remove(model);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"The object {model.Name} has been deleted" });
        }

        #endregion
    }
}
