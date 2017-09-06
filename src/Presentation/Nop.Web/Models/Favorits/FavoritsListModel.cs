using Nop.Core;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Favorits
{
    public partial class FavoritsListModel:BaseNopModel
    {
        public IPagedList<FavoritItemModel> Favorits { get; set; }
        public FavoritItemPagingFilteringModel PagingContext { get; set; }
    }
}