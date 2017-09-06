using DotNetOpenAuth.OAuth2;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Services;
using Google.Apis.Util;
using Nop.Admin.Models.GoogleAnalitics;
using Nop.Core.Domain.Seo;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class GoogleAnaliticsController : Controller
    {
        private readonly GoogleAnaliticsSettings _googleAnliticsSettings;
        private readonly ISettingService _settingService;
        private readonly IDownloadService _downloadService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GoogleAnaliticsController(GoogleAnaliticsSettings googleAnliticsSettings,
            ISettingService settingService,
            IDownloadService downloadService,
            IDateTimeHelper dateTimeHelper)
        {
            this._googleAnliticsSettings = googleAnliticsSettings;
            this._settingService = settingService;
            this._downloadService = downloadService;
            this._dateTimeHelper = dateTimeHelper;
        }


        public ActionResult Index()
        {
            var model = new List<GoogleAnaliticsStats>();
            var scope = AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue();
            string clientId = _googleAnliticsSettings.ClientId;
            string keyPassword = "notasecret";
            var todayStat = new GoogleAnaliticsStatisticModel();
            var week = new GoogleAnaliticsStatisticModel();
            var mounth = new GoogleAnaliticsStatisticModel();
            var year = new GoogleAnaliticsStatisticModel();

            var download = _downloadService.GetDownloadById(_googleAnliticsSettings.PrivateKeyId);
            
            DateTime nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            DateTime t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            if (!timeZone.IsInvalidTime(t1))
            {
                DateTime startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
                todayStat = GetAnaliticsDataMethod(scope, clientId, keyPassword, download, startTime1, nowDt);
            }

            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime t2 = today.AddDays(-(today.DayOfWeek - fdow));
            if (!timeZone.IsInvalidTime(t2))
            {
                DateTime startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
                week = GetAnaliticsDataMethod(scope, clientId, keyPassword, download, startTime2, nowDt);
            }

            DateTime t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            if (!timeZone.IsInvalidTime(t3))
            {
                DateTime startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
                mounth = GetAnaliticsDataMethod(scope, clientId, keyPassword, download, startTime3, nowDt);
            }

            DateTime t4 = new DateTime(nowDt.Year, 1, 1);
            if (!timeZone.IsInvalidTime(t4))
            {
                DateTime startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
                year = GetAnaliticsDataMethod(scope, clientId, keyPassword, download, startTime4, nowDt);
            }

            model.Add(new GoogleAnaliticsStats()
            {
                Name = "Visitors",
                Today = todayStat.Visitors,
                Weekly = week.Visitors,
                Mounthly = mounth.Visitors,
                Year = year.Visitors
            });

            model.Add(new GoogleAnaliticsStats()
            {
                Name = "Unique page views",
                Today = todayStat.UniquePageViews,
                Weekly = week.UniquePageViews,
                Mounthly = mounth.UniquePageViews,
                Year = year.UniquePageViews
            });

            model.Add(new GoogleAnaliticsStats()
            {
                Name = "Average time on site",
                Today = todayStat.AverageTimeOnSite,
                Weekly = week.AverageTimeOnSite,
                Mounthly = mounth.AverageTimeOnSite,
                Year = year.AverageTimeOnSite
            });

            model.Add(new GoogleAnaliticsStats()
            {
                Name = "Exit rate",
                Today = todayStat.ExitRate,
                Weekly = week.ExitRate,
                Mounthly = mounth.ExitRate,
                Year = year.ExitRate
            });

            model.Add(new GoogleAnaliticsStats()
            {
                Name = "New to old visitors rate",
                Today = todayStat.NewToOldVisitorsRate,
                Weekly = week.NewToOldVisitorsRate,
                Mounthly = mounth.NewToOldVisitorsRate,
                Year = year.NewToOldVisitorsRate
            });
            return View(model);
        }

        private GoogleAnaliticsStatisticModel GetAnaliticsDataMethod(string scope, string clientId, string keyPassword, Core.Domain.Media.Download download, DateTime startDate, DateTime endDate)
        {
            var model = new GoogleAnaliticsStatisticModel();
            AuthorizationServerDescription desc = GoogleAuthenticationServer.Description;

            //Create a certificate object to use when authenticating
            X509Certificate2 key = new X509Certificate2(download.DownloadBinary, keyPassword, X509KeyStorageFlags.Exportable);

            //Now, we will log in and authenticate, passing in the description
            //and key from above, then setting the accountId and scope
            AssertionFlowClient client = new AssertionFlowClient(desc, key)
            {
                ServiceAccountId = clientId,
                Scope = scope,
            };

            //Finally, complete the authentication process
            //NOTE: This is the first change from the update above
            OAuth2Authenticator<AssertionFlowClient> auth =
                new OAuth2Authenticator<AssertionFlowClient>(client, AssertionFlowClient.GetState);

            //First, create a new service object
            //NOTE: this is the second change from the update
            //above. Thanks to James for pointing this out
            AnalyticsService gas = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = auth });

            //Create our query
            //The Data.Ga.Get needs the parameters:
            //Analytics account id, starting with ga:
            //Start date in format YYYY-MM-DD
            //End date in format YYYY-MM-DD
            //A string specifying the metrics
            DataResource.GaResource.GetRequest r =
                gas.Data.Ga.Get("ga:" + _googleAnliticsSettings.AccountId, startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"), "ga:visitors,ga:uniquePageviews,ga:AvgTimeOnSite,ga:exitRate");

            //Specify some addition query parameters
            //r.Sort = "-ga:visitors";
            //r.Sort = "-ga:AvgTimeOnSite";
            //r.Sort = "-ga:uniquePageviews";
            //r.Sort = "-ga:exitRate";
            r.MaxResults = 50;

            //Execute and fetch the results of our query
            GaData d = r.Fetch();
            //Write the column headers
            //Write the data
            foreach (var row in d.Rows)
            {
                model.Visitors = row[0];
                model.UniquePageViews = row[1];
                model.AverageTimeOnSite = row[2];
                if (row[3].Length > 5)
                {
                    model.ExitRate = row[3].Substring(0, 5);
                }
                else
                {
                    model.ExitRate = row[3];
                }
            }

            r = gas.Data.Ga.Get("ga:" + _googleAnliticsSettings.AccountId, startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"), "ga:visitors");
            r.Dimensions = "ga:visitorType";
            r.MaxResults = 50;

            //Execute and fetch the results of our query
            d = r.Fetch();
            if (d.Rows.Count == 1)
            {
                if (d.Rows[0][0] != "Returning Visitor")
                {
                    model.NewToOldVisitorsRate = "100%";
                }
                else
                {
                    model.NewToOldVisitorsRate = "0%";
                }
            }
            else
            {
                model.NewToOldVisitorsRate = (((double)(int.Parse(d.Rows[0][1])) / (int.Parse(d.Rows[1][1]))) * 100).ToString() + "%";
            }

            if (model.NewToOldVisitorsRate.Length > 5)
            {
                model.NewToOldVisitorsRate = model.NewToOldVisitorsRate.Substring(0, 5);
            }

            if (model.AverageTimeOnSite.Length - model.AverageTimeOnSite.IndexOf(".") > 3)
            {
                model.AverageTimeOnSite = model.AverageTimeOnSite.Substring(0, model.AverageTimeOnSite.IndexOf(".") + 3);
            }

            return model;
        }

        public ActionResult Setup()
        {
            var model = new AnaliticsSetupModel()
            {
                AccountId = _googleAnliticsSettings.AccountId,
                ClientId = _googleAnliticsSettings.ClientId,
                PrivateKeyDownnloadId = _googleAnliticsSettings.PrivateKeyId
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Setup(AnaliticsSetupModel model)
        {
            if (ModelState.IsValid)
            {
                _googleAnliticsSettings.AccountId = model.AccountId;
                _googleAnliticsSettings.ClientId = model.ClientId;
                if (_googleAnliticsSettings.PrivateKeyId != model.PrivateKeyDownnloadId)
                {
                    var download = _downloadService.GetDownloadById(_googleAnliticsSettings.PrivateKeyId);
                    if (download != null)
                        _downloadService.DeleteDownload(download);
                }
                _googleAnliticsSettings.PrivateKeyId = model.PrivateKeyDownnloadId;
                _settingService.SaveSetting(_googleAnliticsSettings);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
