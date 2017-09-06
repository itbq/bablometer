using Nop.Web.Models.BuyingRequest;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public class UploadCatalogModel
    {
        public int ProductItemType { get; set; }
        public int SelectedCategoryId { get; set; }
        public IList<LangaugeListModel> SelectedLanguages { get; set; }
        public IList<LanguageModel> AviableLanguages { get; set; }
        
        [UIHint("Download")]
        public int DownloadId { get; set; }
    }
}