using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Statistics
{
    public class ProductsStatisticModel
    {
        [NopResourceDisplayName("Admin.Statistics.Type")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Statistics.Today")]
        public int TodayCount { get; set; }
        [NopResourceDisplayName("Admin.Statistics.Weekly")]
        public int WeeklyCount { get; set; }
        [NopResourceDisplayName("Admin.Statistics.Month")]
        public int MonthCount { get; set; }
        [NopResourceDisplayName("Admin.Statistics.Year")]
        public int YearCount { get; set; }
        [NopResourceDisplayName("Admin.Statistics.Total")]
        public int TotalCount { get; set; }
    }
}