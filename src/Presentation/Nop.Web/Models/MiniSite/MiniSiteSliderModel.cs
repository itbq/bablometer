using Nop.Web.Framework;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.MiniSite
{
    public class MiniSiteSliderModel
    {
        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        public int Picture1Id { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        public int Picture2Id { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        public int Picture3Id { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Picture")]
        public int Picture4Id { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link1 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link2 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link3 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Link")]
        [AllowHtml]
        public string Link4 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Title1 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Title2 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Title3 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.Text")]
        [AllowHtml]
        public string Title4 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string Text1 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string Text2 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string Text3 { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.NivoSlider.SubText")]
        [AllowHtml]
        public string Text4 { get; set; }

        public IList<LanguageModel> AviableLanguages { get; set; }

        public LanguageModel SelectedLanguage { get; set; }

        public int SelectedLanguageId { get; set; }
    }
}