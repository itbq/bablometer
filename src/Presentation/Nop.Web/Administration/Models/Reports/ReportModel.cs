using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Reports
{
    public class ReportModel
    {
        [NopResourceDisplayName("ITBSFA.StartDate.Title")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }
        [NopResourceDisplayName("ITBSFA.EndDate.Title")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }
        public int ReportTypeId { get; set; }
        public int ReferenceId { get; set; }
        public string ReportName { get; set; }
    }
}