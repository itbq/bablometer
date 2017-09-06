using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.NivoSlider.Models
{
    public class SliderItemModel : BaseNopEntityModel, ILocalizedModel<SliderItemLocalizedModel>
    {
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string SubText { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link { get; set; }

        public string PictureUrl { get; set; }
        public IList<SliderItemLocalizedModel> Locales { get; set; }
    }

    public class SliderItemLocalizedModel : ILocalizedModelLocal
    {

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Text { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string SubText { get; set; }
    }
}
