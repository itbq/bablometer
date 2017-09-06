using Nop.Core;
using Nop.Services.Favorits;
using Nop.Web.Models.Favorits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Favorit;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Core.Domain.Localization;
using Nop.Services.Seo;
using Nop.Services.Logging;
using Nop.Services.Media;

namespace Nop.Web.Controllers
{
    public class FavoritController : Controller
    {
        private readonly IFavoritsService _favoritsService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly ILanguageService _languageService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerActivityService _customerActivityService;

        public FavoritController(IFavoritsService favoritsService,
            IWorkContext workContext,
            ICategoryService categoryService,
            ILanguageService languageService,
            IPictureService pictureService)
        {
            this._favoritsService = favoritsService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._languageService = languageService;
            this._pictureService = pictureService;
        }

        /// <summary>
        /// Filter favorits tha do not belongs to provuded category
        /// </summary>
        /// <param name="products">favorits to filter</param>
        /// <param name="categoryid">category id</param>
        /// <returns></returns>
        [NonAction]
        protected List<FavoritItem> FilterFavoritsByCategoryId(List<FavoritItem> favorits, int categoryid, List<FavoritItem> resultfavorits = null)
        {
            if (resultfavorits == null)
                resultfavorits = new List<FavoritItem>();
            var childCategories = _categoryService.GetAllCategoriesByParentCategoryId(categoryid);
            if (childCategories.Count == 0)
            {
                resultfavorits.AddRange(favorits.Where(x => x.Product.ProductCategories.First().CategoryId == categoryid));
            }
            else
            {
                foreach (var category in childCategories)
                {
                    FilterFavoritsByCategoryId(favorits, category.Id, resultfavorits);
                }
            } return resultfavorits;
        }
        
        /// <summary>
        /// Convert favoritItem to FavoritItemModel
        /// </summary>
        /// <param name="item">item to convert</param>
        /// <returns></returns>
        [NonAction]
        protected FavoritItemModel PrepareFavoritItemModel(FavoritItem item)
        {
            var model = new FavoritItemModel();
            model.Id = item.Id;
            model.ProductId = item.ProductId;

            var category = item.Product.ProductCategories.First().Category;
            model.CategoryString = category.Name;
            while (category.ParentCategoryId != 0)
            {
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
                model.CategoryString = model.CategoryString.Insert(0, category.GetLocalized(x => x.Name) + "->");
            }

            var languages = new OrderedLanguageCultures();
            model.ProductTitle = item.Product.GetLocalized(p => p.Name, _workContext.WorkingLanguage.Id, true);
            //process favorit product language specific information
            #region
            if (model.ProductTitle != null)
            {
                model.ProductDescription = item.Product.GetLocalized(p => p.ShortDescription, _workContext.WorkingLanguage.Id);
                model.ProductSeName = item.Product.GetSeName(_workContext.WorkingLanguage.Id);
            }
            else
            {
                for (int i = 0; i < languages.Cultures.Count; i++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[i]).FirstOrDefault().Id;
                    model.ProductTitle = item.Product.GetLocalized(p => p.Name, langid, false);
                    if (model.ProductTitle != null)
                    {
                        model.ProductDescription = item.Product.GetLocalized(p => p.ShortDescription, langid);
                        model.ProductSeName = item.Product.GetSeName(langid);
                        break;
                    }
                }
            }
            #endregion

            //process favorit product company information
            #region
            model.CompanyName = item.Product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _workContext.WorkingLanguage.Id, false);
            if (model.CompanyName != null)
            {
                model.CompanySeName = item.Product.Customer.CompanyInformation.GetSeName(_workContext.WorkingLanguage.Id);
            }
            else
            {
                for (int i = 0; i < languages.Cultures.Count; i++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[i]).FirstOrDefault().Id;
                    model.CompanyName = item.Product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, langid, false);
                    if (model.CompanyName != null)
                    {
                        model.CompanySeName = item.Product.Customer.CompanyInformation.GetSeName(langid);
                        break;
                    }
                }
            }
            #endregion

            var picture = item.Product.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
            if (picture == null && item.Product.ProductPictures.Count > 0)
            {
                picture = item.Product.ProductPictures.First();
                model.PictureUrl = _pictureService.GetPictureUrl(picture.PictureId, showDefaultPicture: false);
            }
            else
            {
                if(picture != null)
                    model.PictureUrl = _pictureService.GetPictureUrl(picture.PictureId, showDefaultPicture: false);
            }
            return model;
        }

        public ActionResult List(FavoritItemPagingFilteringModel command)
        {
            var customer = _workContext.CurrentCustomer;
            if(!customer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            command.PageSize = 10;
            var favorits = _favoritsService.GetCustomerFavorits(customer.Id);

            if (command.SelectedCategoryId != 0)
            {
                favorits = FilterFavoritsByCategoryId((List<FavoritItem>)favorits, command.SelectedCategoryId);
            }

            var model = new FavoritsListModel();
            var favoritsModel = favorits.Select(x => PrepareFavoritItemModel(x)).ToList();
            model.Favorits = new PagedList<FavoritItemModel>(favoritsModel, command.PageIndex, command.PageSize);
            model.PagingContext = new FavoritItemPagingFilteringModel();
            model.PagingContext.LoadPagedList(model.Favorits);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddToFavorits(int productId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (productId == 0)
                return new JsonResult();
            var favorit = new FavoritItem()
            {
                ProductId = productId,
                CustomerId = _workContext.CurrentCustomer.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            _favoritsService.Insert(favorit);
            return new JsonResult();
        }

        public ActionResult Remove(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (id == 0)
                return new JsonResult();
            var favorit = _favoritsService.GetFavoritById(id);
            _favoritsService.DeleteFavorit(favorit);
            return RedirectToAction("List");
        }
    }
}
