using Nop.Admin.Models.Paging;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.News;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class PagingController : Controller
    {
        private readonly NewsSettings _newsSettings;
        private readonly ISettingService _settingService;
        private readonly CatalogSettings _catalogSettings;

        public PagingController(NewsSettings newsSettings,
            ISettingService settingService,
            CatalogSettings catalogSettings)
        {
            this._newsSettings = newsSettings;
            this._settingService = settingService;
            this._catalogSettings = catalogSettings;
        }

        public ActionResult Index()
        {
            var model = new PagingModel()
            {
                NewsListPageSize = _newsSettings.NewsArchivePageSize,
                RecentCompanyNewsPageSize = _newsSettings.LatestNewsNumber,
                NewsRssPageSize = _settingService.GetSettingByKey<int>("Rss.News.Count"),
                EventsPageSize = _settingService.GetSettingByKey<int>("event.eventsnumber"),
                ItemCatalogPageSize = _catalogSettings.ItemCatalogPageSize,
                SellerCatalogPageSize = _catalogSettings.SellerCatalogPageSize,
                ActiveRequestsPageSize = _catalogSettings.ActiveRequestsPageSize,
                RequestsHistoryPageSize = _catalogSettings.RequestHistoryPageSize,
                ManageItemsPageSize = _catalogSettings.ManageItemsPageSize,
                RecentlyViewedProductsNumber = _catalogSettings.RecentlyViewedProductsNumber,
                RecentlyViewedSellersNumber = _catalogSettings.RecentlyViewedSellersNumber,
                ItemsRssCount = _settingService.GetSettingByKey<int>("Rss.Product.Count"),
                SellerRssCount = _settingService.GetSettingByKey<int>("Rss.Seller.Count"),
            };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PagingModel model)
        {
            if (ModelState.IsValid)
            {
                var setting = _settingService.GetSettingByKey("NewsSettings.NewsArchivePageSize");
                setting.Value = model.NewsListPageSize.ToString();
                _settingService.SetSetting(setting.Name,setting.Value);

                setting = _settingService.GetSettingByKey("NewsSettings.LatestNewsNumber");
                setting.Value = model.RecentCompanyNewsPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("Rss.News.Count");
                setting.Value = model.NewsRssPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("event.eventsnumber");
                setting.Value = model.EventsPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.ItemCatalogPageSize");
                setting.Value = model.ItemCatalogPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.SellerCatalogPageSize");
                setting.Value = model.SellerCatalogPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.ActiveRequestsPageSize");
                setting.Value = model.ActiveRequestsPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.RequestHistoryPageSize");
                setting.Value = model.RequestsHistoryPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.ManageItemsPageSize");
                setting.Value = model.ManageItemsPageSize.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.RecentlyViewedProductsNumber");
                setting.Value = model.RecentlyViewedProductsNumber.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("CatalogSettings.RecentlyViewedSellersNumber");
                setting.Value = model.RecentlyViewedSellersNumber.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("Rss.Product.Count");
                setting.Value = model.ItemsRssCount.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);

                setting = _settingService.GetSettingByKey("Rss.Seller.Count");
                setting.Value = model.SellerRssCount.ToString();
                _settingService.SetSetting(setting.Name, setting.Value);
            }
            return View(model);
        }

    }
}
