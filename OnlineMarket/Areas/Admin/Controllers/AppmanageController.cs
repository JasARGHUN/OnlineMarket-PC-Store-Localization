using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.DataAccess.Repository.IRepository;
using OnlineMarket.Models;
using OnlineMarket.Models.ViewModels;
using OnlineMarket.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee + "," + SD.Role_SuperAdmin)]
    public class AppmanageController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IInfoRepository _infoRepository;
        private readonly IAppDataRepository _appRepository;
        private readonly IAppAddressRepository _addressRepository;

        public AppmanageController(IWebHostEnvironment hostingEnvironment, IInfoRepository infoRepository, 
            IAppDataRepository appRepository, IAppAddressRepository addressRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _infoRepository = infoRepository;
            _appRepository = appRepository;
            _addressRepository = addressRepository;
        }

        #region Application Name and Logo
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditInfo()
        {
            var info = _infoRepository.GetInfo(1);

            var infoEditViewModel = new AppInfoEditViewModel
            {
                Id = info.AppInfoID,
                AppName = info.AppName,
                ExistingImagePath = info.AppImage
            };

            return View(infoEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditInfo(AppInfoEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var info = _infoRepository.GetInfo(model.Id = 1);

                info.AppName = model.AppName;

                if (model.AppImages != null)
                {
                    if (model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    info.AppImage = ProcessUploadAppImage(model);
                }

                await _infoRepository.UpdateAsync(info);
                TempData["message"] = $"Application {model.AppName} data has been updated..";
                return RedirectToAction("Index");
            }
            return View();
        }

        private string ProcessUploadAppImage(AppInfoCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.AppImages != null && model.AppImages.Count > 0)
            {
                foreach (IFormFile photo in model.AppImages)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }
        #endregion

        #region Social Link
        [HttpGet]
        public IActionResult CreateSocialData()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSocialData(AppSocialLinkCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueSocialFileName = ProcessUploadAppSocialImage(model);

                var newAppSocialAddress = new AppSocialAddress
                {
                    UrlAddress = model.UrlAddress,
                    AppSocialImg = uniqueSocialFileName
                };

                _appRepository.Add(newAppSocialAddress);

                TempData["message"] = $"Link {model.UrlAddress} was created.";
                return RedirectToAction("SocialList");
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditSocialData(int id)
        {
            var data = _appRepository.GetInfo(id);

            var item = new AppSocialLinkUpdateViewModel
            {
                Id = data.Id,
                UrlAddress = data.UrlAddress,
                ExistingSocialImagePath = data.AppSocialImg
            };

            TempData["message"] = $"Object {item.UrlAddress} was selected.";
            return View(item);
        }

        [HttpPost]
        public IActionResult EditSocialData(AppSocialLinkUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = _appRepository.GetInfo(model.Id);
                item.UrlAddress = model.UrlAddress;

                if (model.AppSocialImgs != null)
                {
                    if (model.ExistingSocialImagePath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images", model.ExistingSocialImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    item.AppSocialImg = ProcessUploadAppSocialImage(model);
                }

                _appRepository.Update(item);

                TempData["message"] = $"Object {item.UrlAddress} was edited.";

                return RedirectToAction("SocialList");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSocialAddress(int id)
        {
            var item = await _appRepository.Delete(id);
            if (item != null)
            {
                TempData["message"] = $"Url address: {item.UrlAddress} was deleted";
            }
            return RedirectToAction("SocialList");
        }

        public IActionResult SocialList()
        {
            var obj = _appRepository.AppSocialAddress;
            return View(obj);
        }

        private string ProcessUploadAppSocialImage(AppSocialLinkCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.AppSocialImgs != null && model.AppSocialImgs.Count > 0)
            {
                foreach (IFormFile photo in model.AppSocialImgs)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }
        #endregion

        #region Address

        [HttpGet]
        public IActionResult CreateAppAddressData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppAddressData(AppAddressCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniquePictureFileName = ProcessUploadAppAddressImage(model);

                var newAppAddress = new AppAddress
                {
                    Address = model.Address,
                    Picture = uniquePictureFileName,
                    Phone = model.Phone,
                    City = model.City,
                    Description = model.Description,
                    Email = model.Email
                };

                await _addressRepository.AddAsync(newAppAddress);

                TempData["message"] = $"Address {model.Address} was created.";
                return RedirectToAction("AddressList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditAppAddressData(int id)
        {
            var model = await _addressRepository.GetInfoAsync(id);

            var item = new AppAddressUpdateViewModel
            {
                Id = model.Id,
                Address = model.Address,
                Phone = model.Phone,
                City = model.City,
                Email = model.Email,
                Description = model.Description,
                AppPicturePath = model.Picture
            };

            TempData["message"] = $"Object {item.Address} was selected.";
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditAppAddressData(AppAddressUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await _addressRepository.GetInfoAsync(model.Id);

                item.Address = model.Address;
                item.City = model.City;
                item.Phone = model.Phone;
                item.Email = model.Email;
                item.Description = model.Description;

                if (model.AppPicture != null)
                {
                    if (model.AppPicturePath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images", model.AppPicturePath);
                        System.IO.File.Delete(filePath);
                    }
                    item.Picture = ProcessUploadAppAddressImage(model);
                }

                await _addressRepository.UpdateAsync(item);

                TempData["message"] = $"Object {item.Address} was edited.";

                return RedirectToAction("AddressList");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var item = await _addressRepository.DeleteAsync(id);

            if (item != null)
            {
                TempData["message"] = $"Url address: {item.Address} was deleted";
            }

            return RedirectToAction("AddressList");
        }

        public IActionResult AddressList()
        {
            var obj = _addressRepository.AppAddress;
            return View(obj);
        }

        private string ProcessUploadAppAddressImage(AppAddressCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.AppPicture != null && model.AppPicture.Count > 0)
            {
                foreach (IFormFile photo in model.AppPicture)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }
        #endregion

    }
}
