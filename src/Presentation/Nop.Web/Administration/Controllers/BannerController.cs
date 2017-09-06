using Nop.Admin.Models.Banner;
using Nop.Admin.Models.Media;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Services.BannerService;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using System.IO;
using Nop.Services.Localization;
using Nop.Admin.Models.Banners;
using Nop.Services.Catalog;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class BannerController : BaseNopController
    {
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;

        public BannerController(IBannerService bannerService,
            IPictureService pictureService,
            ICustomerActivityService customerActivityService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ICategoryService categoryService)
        {
            this._bannerService = bannerService;
            this._pictureService = pictureService;
            this._customerActivityService = customerActivityService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._categoryService = categoryService;
        }

        [NonAction]
        private int GetBannerHeight(BannerTypeEnum type)
        {
            switch (type)
            {
                case BannerTypeEnum.HomePage:
                case BannerTypeEnum.Productpage:
                case BannerTypeEnum.TextPage:
                    {
                        return 140;
                    }
                case BannerTypeEnum.CatalogPage:
                    {
                        return 100;
                    }
                default:
                    {
                        return 140;
                    }
            }
        }

        [NonAction]
        private int GetBannerWidth(BannerTypeEnum type)
        {
            switch (type)
            {
                case BannerTypeEnum.HomePage:
                case BannerTypeEnum.Productpage:
                case BannerTypeEnum.TextPage:
                    {
                        return 238;
                    }
                case BannerTypeEnum.CatalogPage:
                    {
                        return 720;
                    }
                default:
                    {
                        return 238;
                    }
            }
        }

        public ActionResult List()
        {
            int languageId = _workContext.WorkingLanguage.Id;
            var homePageBanners = _bannerService.GetAllBanners()
                .Select(x =>
                {
                    var model = new BannerModel();
                    model.Url = x.Url;
                    model.Title = x.Title;
                    model.Alt = x.Alt;
                    model.PictureId = x.PictureId;
                    model.Id = x.Id;
                    model.NetBanner = x.NetBanner;
                    model.Size = GetBannerWidth(x.BannerType);
                    model.DisplayOnMain = x.DisplayOnMain;
                    model.Height = GetBannerHeight(x.BannerType);
                    model.BannerType = x.BannerType;
                    if (x.BannerType == BannerTypeEnum.Productpage)
                    {
                        var category = _categoryService.GetCategoryById(x.CategoryId.Value);
                        model.CategoryName = category.Name;
                        model.CategoryId = category.Id;
                    }
                    model.BannerTypeId = x.BannerTypeId;
                    model.BannerTypeString = x.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
                    return model;
                })
                .Select(x =>
                {
                    x.PictureModel = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(x.PictureId, x.Size),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId)
                    };
                    return x;
                })
                .ToList();

            var viewModel = new BannerListModel()
            {
                HomePageBanners = homePageBanners
            };
            return View(viewModel);
        }

        public ActionResult Add()
        {
            var model = new BannerModel();
            var categories = _categoryService.GetAllCategories();
            model.AviableCategories = categories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(BannerModel model)
        {
            ModelState.Remove("Id");

            if (model.PictureId!=0 || !String.IsNullOrEmpty(model.NetBanner))
            {
                var banner = new Banner()
                {
                    Title = model.Title,
                    Alt = model.Alt,
                    PictureId = model.PictureId,
                    Url = model.Url,
                    DisplayOnMain = model.DisplayOnMain,
                    BannerTypeId = model.BannerTypeId,
                    NetBanner=model.NetBanner,
                    Size = GetBannerWidth((BannerTypeEnum)model.BannerTypeId),
                    Height = GetBannerHeight((BannerTypeEnum)model.BannerTypeId),
                    CategoryId = model.CategoryId,
                };
                if (banner.BannerType == BannerTypeEnum.Productpage)
                {
                    banner.CategoryId = model.CategoryId;
                }
                _bannerService.InsertBanner(banner);
                _customerActivityService.InsertActivity("EditBanner", "Banner edited");
                SuccessNotification(_localizationService.GetResource("Admin.Banner.Success"));
                return RedirectToAction("List");
            }
            var categories = _categoryService.GetAllCategories();
            model.AviableCategories = categories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();
            ErrorNotification(_localizationService.GetResource("Admin.Bannner.Error"));

            ViewBag.errror = true;
            return View(model);
        }

        public ActionResult Delete(int Id)
        {
            var banner = _bannerService.GetBannerById(Id);
            if (banner != null)
            {
                foreach (var lang in _languageService.GetAllLanguages())
                {
                    int pictureId = banner.GetLocalized(x => x.PictureId,lang.Id,false);
                    if (pictureId != 0)
                    {
                        var picture = _pictureService.GetPictureById(pictureId);
                        if (picture != null)
                            _pictureService.DeletePicture(picture);
                    }
                }
                _bannerService.DeleteBannner(banner);
            }
            return RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            var banner = _bannerService.GetBannerById(id);
            var model = new BannerModel();
            if (banner.BannerType == BannerTypeEnum.Productpage)
            {
                var category = _categoryService.GetCategoryById(banner.CategoryId.Value);
                model.CategoryName = category.Name;
                model.CategoryId = category.Id;
            }
            var categories = _categoryService.GetAllCategories();
            model.AviableCategories = categories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == model.CategoryId
            }).ToList();
            model.Id = banner.Id;
            model.Size = GetBannerWidth(banner.BannerType);
            model.Height = GetBannerHeight(banner.BannerType);
            model.DisplayOnMain = banner.DisplayOnMain;
            model.BannerType = banner.BannerType;
            model.Alt = banner.Alt;
            model.PictureId = banner.PictureId;
            model.Title = banner.Title;
            model.Url = banner.Url;
            model.NetBanner = banner.NetBanner;
            model.BannerTypeId = banner.BannerTypeId;
            model.BannerType = banner.BannerType;
            model.BannerTypeString = banner.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BannerModel model)
        {
            var oldBanner = _bannerService.GetBannerById(model.Id);

            if (model.PictureId != 0 || !String.IsNullOrEmpty(model.NetBanner))
            {
                int prevPictureId = oldBanner.PictureId;
                var picture = _pictureService.GetPictureById(prevPictureId);
                if (prevPictureId != model.PictureId)
                {
                    if (picture != null)
                        _pictureService.DeletePicture(picture);
                    oldBanner.PictureId = model.PictureId;
                }
                oldBanner.CategoryId = model.CategoryId;
                oldBanner.Title = model.Title;
                oldBanner.Alt = model.Alt;
                oldBanner.NetBanner = model.NetBanner;
                oldBanner.BannerTypeId = model.BannerTypeId;
                oldBanner.Url = model.Url;
                oldBanner.Size = GetBannerWidth((BannerTypeEnum)model.BannerTypeId);
                oldBanner.Height = GetBannerHeight((BannerTypeEnum)model.BannerTypeId);
                _bannerService.UpdateBanner(oldBanner);
                _customerActivityService.InsertActivity("EditBanner", "Banner edited");
                SuccessNotification(_localizationService.GetResource("Admin.Banner.Success"));
                return RedirectToAction("List");
            }
            var categories = _categoryService.GetAllCategories();
            model.AviableCategories = categories.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = x.Id == model.CategoryId
            }).ToList();
            ErrorNotification(_localizationService.GetResource("Admin.Bannner.Error"));
            ViewBag.errror = true;
            return View(model);
        }
    }
}
