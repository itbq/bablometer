using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Services.Logging;
using Nop.Web.Models.Media;
using Nop.Web.Models.CompanyInformation;
using Nop.Services.News;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Controllers
{
    public partial class CompanyController : BaseNopController
    {
        private readonly ICompanyInformationService _companyInformationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureSerice;
        private readonly IGenericAttributeService _genericAttibuteService;
        private readonly INewsService _newsService;
        private readonly IProductViewService _productViewService;
        private readonly ICompanyInformationViewService _companyinformationViewService;
        private readonly IProductService _productService;
        private readonly NewsSettings _newsSettings;
        private readonly CatalogSettings _catalogSettings;

        public CompanyController(ICompanyInformationService companyInformationService,
            ICustomerService customerService,
            IWorkContext workContext,
            IPictureService pictureService,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            IProductViewService productViewService,
            ICompanyInformationViewService companyinformationViewService,
            IProductService productService,
            NewsSettings newsSettings,
            CatalogSettings catalogSettings)
        {
            this._companyInformationService = companyInformationService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._pictureSerice = pictureService;
            this._genericAttibuteService = genericAttributeService;
            this._newsService = newsService;
            this._productViewService = productViewService;
            this._productService = productService;
            this._companyinformationViewService = companyinformationViewService;
            this._newsSettings = newsSettings;
            this._catalogSettings = catalogSettings;
        }

        /// <summary>
        /// Check is company locale filled in correctly
        /// </summary>
        /// <param name="model">company model</param>
        /// <returns></returns>
        [NonAction]
        protected bool CheckCompanyLocale(CompanyInformationModel model)
        {
            return model.CompanyDescription != null && model.CompanyName != null;
        }

        /// <summary>
        /// Prepare company list model
        /// </summary>
        /// <param name="sellers">what type of companies to prepare: sellers/buyers</param>
        /// <returns></returns>
        [NonAction]
        protected CompanyListModel PrepareCompanyListModel(bool sellers)
        {
            var companies = _companyInformationService.GetAllCompanies()
                .Select(x =>
                {
                    var model = x.ToModel(_workContext.WorkingLanguage.Id);
                    model.Picture = new PictureModel()
                    {
                        Title = model.CompanyName,
                        ImageUrl = _pictureSerice.GetPictureUrl(x.GetAttribute<int>(SystemCustomerAttributeNames.PictureId))
                    };
                    return model;
                })
                .Where(x=>x.Seller == sellers)
                .Where(x=>CheckCompanyLocale(x));
            return new CompanyListModel() { Compannies = companies.ToList() };
        }

        /// <summary>
        /// Prepare company list model
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected CompanyListModel PrepareCompanyListModel()
        {
            var companies = _companyInformationService.GetAllCompanies()
                .Select(x =>
                {
                    var model = x.ToModel(_workContext.WorkingLanguage.Id);
                    model.Picture = new PictureModel()
                    {
                        Title = model.CompanyName,
                        ImageUrl = _pictureSerice.GetPictureUrl(x.GetAttribute<int>(SystemCustomerAttributeNames.PictureId))
                    };
                    return model;
                })
                .Where(x => CheckCompanyLocale(x));

            return new CompanyListModel() { Compannies = companies.ToList() };
        }

        /// <summary>
        /// Prepare recently added companies list
        /// </summary>
        /// <param name="sellers">prepare sellers/buyers</param>
        /// <returns></returns>
        [NonAction]
        protected CompanyListModel PrepareRecentCompanies(bool sellers)
        {
            var companies = _companyInformationService.GetAllCompanies()
                .Select(x =>
                {
                    var model = x.ToModel(_workContext.WorkingLanguage.Id);
                    model.Picture = new PictureModel()
                    {
                        Title = model.CompanyName,
                        ImageUrl = _pictureSerice.GetPictureUrl(x.GetAttribute<int>(SystemCustomerAttributeNames.PictureId))
                    };
                    return model;
                })
                .Where(x => x.Seller == sellers);
            companies = companies.Select(x =>
            {
                x.Customer = _customerService.GetCustomerById(x.CustomerId);
                return x;
            })
            .OrderBy(x=>x.Customer.CreatedOnUtc);
            return new CompanyListModel() { Compannies = companies.ToList() };
        }

        /// <summary>
        /// Prepare productShortModel
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="languageId">language id</param>
        /// <returns></returns>
        protected ProductShortModel PrepareProductModel(int productId, int languageId)
        {
            var product = _productService.GetProductById(productId);
            var model = new ProductShortModel()
            {
                ProductName = product.Name,
                ProductSeName = product.GetSeName(0)
            };

            var pictureId = product.ProductPictures.Where(x => x.DisplayOrder == 0).Select(x => x.PictureId).FirstOrDefault();
            if (pictureId == 0)
            {
                pictureId = product.ProductPictures.Select(x => x.PictureId).FirstOrDefault();
            }


            if (pictureId != 0)
            {
                model.PictureUrl = _pictureSerice.GetPictureUrl(pictureId, showDefaultPicture: false,targetSize:100);
            }
            return model;

        }

        /// <summary>
        /// Display public company profile
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>
        public ActionResult CompanyInfo(int companyid)
        {
            var Company = _companyInformationService.GetCompanyInformation(companyid);
            if (Company.Customers.First().Deleted)
            {
                throw new HttpException(404, "Not found");
            }

            bool languageRedirect = _workContext.WorkingLanguage.UniqueSeoCode == "ru" || _workContext.WorkingLanguage.UniqueSeoCode == "en";
            if (Company.Customers.First().UserMiniSite != null && Company.Customers.First().UserMiniSite.Active && languageRedirect)
            {
                return Redirect("http://" + Company.Customers.First().UserMiniSite.DomainName);
            }
            var model = Company.ToModel(_workContext.WorkingLanguage.Id);
            if (model.Seller)
            {
                var companyDocuments = Company.CompanyDocuments.Where(x => x.IsLegal);
                model.LegalDocumennts = companyDocuments.Select(x => new UploadModel()
                {
                    CompanyId = x.CompanyId.GetValueOrDefault(),
                    DownloadId = x.DownloadGuid,
                    FileExtension = x.Extension,
                    FileName = x.Filename,
                    FileSize = x.FileSize,
                    IsLegal = x.IsLegal,
                    Id = x.Id
                }).ToList();
                if (model.LegalDocumennts == null)
                    model.LegalDocumennts = new List<UploadModel>();
            }
            model.CompanyDocuments = Company.CompanyDocuments.Where(x => !x.IsLegal).Select(x => new UploadModel()
            {
                CompanyId = x.CompanyId.GetValueOrDefault(),
                DownloadId = x.DownloadGuid,
                FileExtension = x.Extension,
                FileName = x.Filename,
                FileSize = x.FileSize,
                IsLegal = x.IsLegal,
                Id = x.Id
            }).ToList();
            if (model.CompanyDocuments == null)
                model.CompanyDocuments = new List<UploadModel>();
            model.Picture = new PictureModel()
            {
                Title = model.CompanyName,
                ImageUrl = _pictureSerice.GetPictureUrl(Company.GetAttribute<int>(SystemCustomerAttributeNames.PictureId),showDefaultPicture:false)
            };

            _companyinformationViewService.AddCompanyInformationView(companyid, _workContext.CurrentCustomer.Id);

            return View(model);
        }
        
        /// <summary>
        /// Display company list
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyList()
        {
            var model = PrepareCompanyListModel();
            return View(model);
        }

        /// <summary>
        /// Display popular company products block
        /// </summary>
        /// <param name="customerId">customer id </param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult PopularProducts(int customerId)
        {
            var products = _productViewService.GetPopularCustomerProducts(customerId,_workContext.WorkingLanguage.Id);
            var model = products.Select(x => PrepareProductModel(x,_workContext.WorkingLanguage.Id));
            return View(model);
        }

        /// <summary>
        /// Dispaly Recently viewed companies by speciifiied customer
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RecentlyViewedSellers(int customerId)
        {
            var companies = _companyinformationViewService.RecentlyViewevCompaniesSellers(customerId, _workContext.WorkingLanguage.Id);
            var model = companies.Where(x=>x.Customers.First().IsSeller()).Select(x => new CompanyInformationModel()
            {
                CompanyName = x.GetLocalized(c=>c.CompanyName,_workContext.WorkingLanguage.Id),
                CompanyDescription = x.GetLocalized(c=>c.CompanyDescription,_workContext.WorkingLanguage.Id),
                CompanySeName = x.GetSeName(_workContext.WorkingLanguage.Id),
                Picture = new PictureModel()
                    {
                        ImageUrl = _pictureSerice.GetPictureUrl(x.GetAttribute<int>(SystemCustomerAttributeNames.PictureId),showDefaultPicture:false)
                    }
            });
            return View(model);
        }

        /// <summary>
        /// Display recently viewed products by specified customer
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RecentlyViewedProducts(int customerId)
        {
            var products = _productViewService.GetCustomerProductViews(customerId, _workContext.WorkingLanguage.Id)
                .Select(x => x.ProductId);
            var model = products.Select(x => PrepareProductModel(x, _workContext.WorkingLanguage.Id));
            return View(model);
        }
        
        [ChildActionOnly]
        public ActionResult RecentlyAddedCompanyProducts(int customerId)
        {
            var products = _productService.GetAllProducts().Where(x => !x.Deleted && x.Published && x.CustomerId == customerId)
                .Where(x=>x.GetLocalized(p=>p.Name,_workContext.WorkingLanguage.Id,false) != null)
                .OrderByDescending(x=>x.CreatedOnUtc)
                .Select(x=>x.Id);
            var model = products.Select(x => PrepareProductModel(x, _workContext.WorkingLanguage.Id)).Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();
            return View(model);
        }
    }
}