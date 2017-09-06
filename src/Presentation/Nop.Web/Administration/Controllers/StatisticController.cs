using Nop.Admin.Models.Statistics;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.RequestServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class StatisticController : Controller
    {
        private readonly IProductService _productService;
        private readonly IRequestService _requestService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        public StatisticController(IProductService productService,
            IRequestService requestService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            this._productService = productService;
            this._requestService = requestService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
        }

        public PartialViewResult DealsStatistics()
        {
            var model = new DealsStatisticRangeModel(_localizationService);
            var requests = _requestService.GetAllRequests();
            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            DateTime t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                model.Accepted.TodayCount = requests.Where(x => (x.Accepted.HasValue && x.Accepted.Value) && x.ResponsedOnUtc > startTime1).Count();
                model.Pending.TodayCount = requests.Where(x => x.CreatedOnUtc > startTime1 && x.Accepted == null).Count();
                model.Rejected.TodayCount = requests.Where(x => (x.Accepted.HasValue && !x.Accepted.Value) && x.ResponsedOnUtc > startTime1).Count();
                model.Total.TodayCount = requests.Where(x => (x.Accepted.HasValue ? x.ResponsedOnUtc : x.CreatedOnUtc) > startTime1).Count();
            }

            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                model.Accepted.WeeklyCount = requests.Where(x => (x.Accepted.HasValue && x.Accepted.Value) && x.ResponsedOnUtc > startTime2).Count();
                model.Pending.WeeklyCount = requests.Where(x => x.CreatedOnUtc > startTime2 && x.Accepted == null).Count();
                model.Rejected.WeeklyCount = requests.Where(x => (x.Accepted.HasValue && !x.Accepted.Value) && x.ResponsedOnUtc > startTime2).Count();
                model.Total.WeeklyCount = requests.Where(x => (x.Accepted.HasValue ? x.ResponsedOnUtc : x.CreatedOnUtc) > startTime2).Count();
            }

            //month
            DateTime t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                model.Accepted.MonthCount = requests.Where(x => (x.Accepted.HasValue && x.Accepted.Value) && x.ResponsedOnUtc > startTime3).Count();
                model.Pending.MonthCount = requests.Where(x => x.CreatedOnUtc > startTime3 && x.Accepted == null).Count();
                model.Rejected.MonthCount = requests.Where(x => (x.Accepted.HasValue && !x.Accepted.Value) && x.ResponsedOnUtc > startTime3).Count();
                model.Total.MonthCount = requests.Where(x => (x.Accepted.HasValue ? x.ResponsedOnUtc : x.CreatedOnUtc) > startTime3).Count();
            }

            //year
            DateTime t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                model.Accepted.YearCount = requests.Where(x => (x.Accepted.HasValue && x.Accepted.Value) && x.ResponsedOnUtc > startTime4).Count();
                model.Pending.YearCount = requests.Where(x => x.CreatedOnUtc > startTime4 && x.Accepted == null).Count();
                model.Rejected.YearCount = requests.Where(x => (x.Accepted.HasValue && !x.Accepted.Value) && x.ResponsedOnUtc > startTime4).Count();
                model.Total.YearCount = requests.Where(x => (x.Accepted.HasValue ? x.ResponsedOnUtc : x.CreatedOnUtc) > startTime4).Count();
            }

            model.Accepted.TotalCount = requests.Where(x => x.Accepted.HasValue && x.Accepted.Value).Count();
            model.Pending.TotalCount = requests.Where(x => x.Accepted == null).Count();
            model.Rejected.TotalCount = requests.Where(x => x.Accepted.HasValue && !x.Accepted.Value).Count();
            model.Total.TotalCount = requests.Count();
            return PartialView(model);
        }

        public ActionResult ProductsStatistics()
        {
            var model = new ProductsStatisticRangeModel(_localizationService);
            var products = _productService.GetAllProducts().Where(x=>!x.Deleted && x.Name !="tmp_product");
            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            DateTime t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                model.Product.TodayCount = products.Where(x => x.CreatedOnUtc > startTime1).Count();
                model.Service.TodayCount = products.Where(x => x.CreatedOnUtc > startTime1).Count();
                model.ProductBuyingRequest.TodayCount = products.Where(x => x.CreatedOnUtc > startTime1).Count();
                model.ServiceBuyingRequest.TodayCount = products.Where(x => x.CreatedOnUtc > startTime1).Count();
            }

            //week
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                model.Product.WeeklyCount = products.Where(x => x.CreatedOnUtc > startTime2).Count();
                model.Service.WeeklyCount = products.Where(x => x.CreatedOnUtc > startTime2).Count();
                model.ProductBuyingRequest.WeeklyCount = products.Where(x => x.CreatedOnUtc > startTime2).Count();
                model.ServiceBuyingRequest.WeeklyCount = products.Where(x => x.CreatedOnUtc > startTime2).Count();
            }

            //month
            DateTime t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                model.Product.MonthCount = products.Where(x => x.CreatedOnUtc > startTime3).Count();
                model.Service.MonthCount = products.Where(x => x.CreatedOnUtc > startTime3).Count();
                model.ProductBuyingRequest.MonthCount = products.Where(x => x.CreatedOnUtc > startTime3).Count();
                model.ServiceBuyingRequest.MonthCount = products.Where(x => x.CreatedOnUtc > startTime3).Count();
            }

            //year
            DateTime t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                model.Product.YearCount = products.Where(x => x.CreatedOnUtc > startTime4).Count();
                model.Service.YearCount = products.Where(x => x.CreatedOnUtc > startTime4).Count();
                model.ProductBuyingRequest.YearCount = products.Where(x => x.CreatedOnUtc > startTime4).Count();
                model.ServiceBuyingRequest.YearCount = products.Where(x => x.CreatedOnUtc > startTime4).Count();
            }

            model.Product.TotalCount = products.Count();
            model.Service.TotalCount = products.Count();
            model.ProductBuyingRequest.TotalCount = products.Count();
            model.ServiceBuyingRequest.TotalCount = products.Count();
            
            return PartialView(model);
        }
    }
}
