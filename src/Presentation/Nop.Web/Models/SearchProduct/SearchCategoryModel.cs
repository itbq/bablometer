
using Nop.Web.Framework.Mvc;


namespace Nop.Web.Models.SearchProduct
{
    public class SearchCategoryModel : BaseNopModel
    {
        public string CateogyTitle { get; set; }
        public int CategoryId { get; set; }
        public string SeName { get; set; }
    }
}