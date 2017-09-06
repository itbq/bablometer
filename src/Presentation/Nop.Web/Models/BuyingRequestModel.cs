using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Brand;
using Nop.Web.Models.BuyingRequest;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.BuyingRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Core.Domain.Localization;
using Nop.Web.Models.Common;

namespace Nop.Web.Models
{
    [Validator(typeof(BuyingRequestValidator))]
    public partial class BuyingRequestModel:BaseNopModel
    {
        [NopResourceDisplayName("ETF.Product.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("ETF.Product.Short")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("ETF.Product.Full")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("ETF.Product.Comment")]
        [AllowHtml]
        public string OrderingComments { get; set; }

        public bool IsService { get; set; }

        public IList<int> UploadedPicturesIds { get; set; }

        [NopResourceDisplayName("Profile.Catalog.Brand")]
        [AllowHtml]
        public string Brand { get; set; }

        public IEnumerable<BrandModel> AviableBrands { get; set; }

        [NopResourceDisplayName("ETF.Front.Product.Details.Keywords")]
        [AllowHtml]
        public string Keywords { get; set; }

        public int ProductId { get; set; }

        [UIHint("ListPictureUpload")]
        public int PictureId { get; set; }
        
        public IEnumerable<ProductTagModel> AviableTags { get; set; }

        public CategoryNavigationModel CategoryModel { get; set; }

        public List<CategoryAttributeValueModel> SelectedAttributes { get; set; }

        public List<BuyingRequestLanguageModel> AviableLanguages { get; set; }

        public List<ProductDetailsModel.ProductPriceModel> ProductPrices { get; set; }

        public CategoryNavigationModel[] Categories { get; set; }
        public int ProductItemTypeId { get; set; }
        public int SelectedCategoryId { get; set; }

        public int WorkingLanguage { get; set; }

        public int PictureCount { get; set; }

        public bool PostBack { get; set; }

        public bool HaveConverionsImages { get; set; }
    }
}