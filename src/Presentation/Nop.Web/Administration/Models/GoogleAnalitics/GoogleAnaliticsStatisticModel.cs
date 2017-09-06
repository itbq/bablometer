using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.GoogleAnalitics
{
    public class GoogleAnaliticsStatisticModel
    {
        public string Visitors { get; set; }
        public string AverageTimeOnSite { get; set; }
        public string UniquePageViews { get; set; }
        public string ExitRate { get; set; }
        public string NewToOldVisitorsRate { get; set; }
    }

    public class GoogleAnaliticsStats
    {
        [NopResourceDisplayName("Admin.Statistics.Type")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Statistics.Today")]
        public string Today { get; set; }

        [NopResourceDisplayName("Admin.Statistics.Weekly")]
        public string Weekly { get; set; }

        [NopResourceDisplayName("Admin.Statistics.Month")]
        public string Mounthly { get; set; }

        [NopResourceDisplayName("Admin.Statistics.Year")]
        public string Year { get; set; }
    }
}