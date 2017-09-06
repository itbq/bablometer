using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;


namespace Nop.Web.Models.Common
{
    public partial class OffersProductModel : BaseNopModel
    {
        public  int Id { get; set; }
        public  string Name { get; set; }
        public  string MetaTitle { get; set; }
        public  string PictureThumbnailUrl { get; set; }
        public  string ShortDescription { get; set; }
        public  string FullDescription { get; set; }
        public  CategoryProductAttributeValue ProductAttributeValue { get; set; }
        public  double? Rating { get; set; }
        public  double? BankRating { get; set; }
        public  string SeName { get; set; }
        public string OrderingLink { get; set; }
    }
}