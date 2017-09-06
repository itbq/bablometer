using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Statistics
{
    public class DealsStatisticRangeModel
    {
        public DealsStatisticRangeModel(ILocalizationService localizationService)
        {
            this.Pending = new DealsStatisticModel();
            this.Pending.Name = localizationService.GetResource("Admin.Statistic.Pending");
            this.Rejected = new DealsStatisticModel();
            this.Rejected.Name = localizationService.GetResource("Admin.Statistics.Rejected");
            this.Accepted = new DealsStatisticModel();
            this.Accepted.Name = localizationService.GetResource("Admin.Statistics.Accepted");
            this.Total = new DealsStatisticModel();
            this.Total.Name = localizationService.GetResource("Admin.Statistics.Total");
        }

        public DealsStatisticModel Pending { get; set; }
        public DealsStatisticModel Accepted { get; set; }
        public DealsStatisticModel Rejected { get; set; }
        public DealsStatisticModel Total { get; set; }

        public IEnumerable<DealsSource> GetTotalPercentage()
        {
            var list = new List<DealsSource>();
            var pending = new DealsSource()
            {
                Percentage = (int)((double)this.Pending.TotalCount/this.Total.TotalCount*100),
                Source = this.Pending.Name
            };
            list.Add(pending);
            var accepted = new DealsSource()
            {
                Percentage = (int)Math.Ceiling((double)this.Accepted.TotalCount / this.Total.TotalCount * 100),
                Source = this.Accepted.Name
            };

            list.Add(accepted);

            var rejected = new DealsSource()
            {
                Percentage = 100 - pending.Percentage - accepted.Percentage,
                Source = this.Rejected.Name
            };
            list.Add(rejected);
            return list;
        }

        public IEnumerable<DealsSource> GetTodayPercentage()
        {
            var list = new List<DealsSource>();
            var pending = new DealsSource()
            {
                Percentage = this.Total.TodayCount == 0 ? 0 :(int)((double)this.Pending.TodayCount / this.Total.TodayCount * 100),
                Source = this.Pending.Name
            };
            list.Add(pending);
            var accepted = new DealsSource()
            {
                Percentage = this.Total.TodayCount == 0 ? 0 : (int)Math.Ceiling((double)this.Accepted.TodayCount / this.Total.TodayCount * 100),
                Source = this.Accepted.Name
            };

            list.Add(accepted);

            var rejected = new DealsSource()
            {
                Percentage = this.Total.TodayCount == 0 ? 0 : (100 - pending.Percentage - accepted.Percentage),
                Source = this.Rejected.Name
            };
            list.Add(rejected);
            return list;
        }

        public IEnumerable<DealsSource> GetWeeklyPercentage()
        {
            var list = new List<DealsSource>();
            var pending = new DealsSource()
            {
                Percentage = this.Total.WeeklyCount == 0 ? 0 : (int)((double)this.Pending.WeeklyCount / this.Total.WeeklyCount * 100),
                Source = this.Pending.Name
            };
            list.Add(pending);
            var accepted = new DealsSource()
            {
                Percentage = this.Total.WeeklyCount == 0 ? 0 : (int)Math.Ceiling((double)this.Accepted.WeeklyCount / this.Total.WeeklyCount * 100),
                Source = this.Accepted.Name
            };

            list.Add(accepted);

            var rejected = new DealsSource()
            {
                Percentage = this.Total.WeeklyCount == 0 ? 0 : (100 - pending.Percentage - accepted.Percentage),
                Source = this.Rejected.Name
            };
            list.Add(rejected);
            return list;
        }

        public IEnumerable<DealsSource> GetMonthPercentage()
        {
            var list = new List<DealsSource>();
            var pending = new DealsSource()
            {
                Percentage = this.Total.MonthCount == 0 ? 0 : (int)((double)this.Pending.MonthCount / this.Total.MonthCount * 100),
                Source = this.Pending.Name
            };
            list.Add(pending);
            var accepted = new DealsSource()
            {
                Percentage = this.Total.MonthCount == 0 ? 0 : (int)Math.Ceiling((double)this.Accepted.MonthCount / this.Total.MonthCount * 100),
                Source = this.Accepted.Name
            };

            list.Add(accepted);

            var rejected = new DealsSource()
            {
                Percentage = this.Total.MonthCount == 0 ? 0 : 100 - pending.Percentage - accepted.Percentage,
                Source = this.Rejected.Name
            };
            list.Add(rejected);
            return list;
        }

        public IEnumerable<DealsSource> GetYearPercentage()
        {
            var list = new List<DealsSource>();
            var pending = new DealsSource()
            {
                Percentage = this.Total.YearCount == 0 ? 0 : (int)((double)this.Pending.YearCount / this.Total.YearCount * 100),
                Source = this.Pending.Name
            };
            list.Add(pending);
            var accepted = new DealsSource()
            {
                Percentage = this.Total.YearCount == 0 ? 0 : (int)Math.Ceiling((double)this.Accepted.YearCount / this.Total.YearCount * 100),
                Source = this.Accepted.Name
            };

            list.Add(accepted);

            var rejected = new DealsSource()
            {
                Percentage = this.Total.YearCount == 0 ? 0 : 100 - pending.Percentage - accepted.Percentage,
                Source = this.Rejected.Name
            };
            list.Add(rejected);
            return list;
        }
    }

    public class DealsSource
    {
        public string Source { get; set; }
        public int Percentage { get; set; }
    }
}