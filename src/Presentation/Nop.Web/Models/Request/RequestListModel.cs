using FluentValidation.Attributes;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Request
{
    public partial class RequestListModel
    {
        public IPagedList<RequestOverviewModel> Requests { get; set; }
        public RequestPagableModel PagingContext { get; set; }
    }

    [Validator(typeof(RequestValidator))]
    public partial class RequestOverviewModel : BaseNopEntityModel
    {
        public string ProductTitle { get; set; }
        public string ProductSeName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductId { get; set; }
        public string BrandName { get; set; }
        public ProductItemTypeEnum ItemType { get; set; }

        public DateTime RequestDate { get; set; }
        public string CompanyName { get; set; }
        public string CompanySeName { get; set; }

        public bool? Status { get; set; }
        public string RequestComment { get; set; }
        public string RequestAnswer { get; set; }
        public int Quantity { get; set; }
        public bool IsNew { get; set; }
    }
}