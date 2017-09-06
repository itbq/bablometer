using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{

    public class ConversionImageModel : BaseNopEntityModel, ILocalizedModel<ConversionImageLocalizedModel>
    {

        public IList<ConversionImageLocalizedModel> Locales { get; set; }

        [AllowHtml]
        public string Name { get; set; }
        public int PictureId {get; set;}
        public int GroupModelId { get; set; }
    }

    public class ConversionImageLocalizedModel : ILocalizedModelLocal
    {
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [AllowHtml]
        public string Name { get; set; }
        public int LanguageId { get; set; }
    }
}