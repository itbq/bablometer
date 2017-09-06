using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Brand
{
    public partial class BrandModel : BaseNopEntityModel, ILocalizedModel<BrandLocalizedModel>
    {
        public BrandModel()
        {
            Locales = new List<BrandLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Brand.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Brand.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.Brand.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [NopResourceDisplayName("Admin.Brand.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Brand.LogoId")]
        public int LogoId { get; set; }

        [NopResourceDisplayName("Admin.Brand.ProductCount")]
        public int ProductCount { get; set; }

        public IList<BrandLocalizedModel> Locales { get; set; }
    }

    public partial class BrandLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Brand.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Brand.Description")]
        [AllowHtml]
        public string Description { get; set; }
    }
}
