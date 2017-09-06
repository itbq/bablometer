using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Favorits
{
    public partial class FavoritItemModel:BaseNopEntityModel
    {
        /// <summary>
        /// Item pictureUrl
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// Favorit product title
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// favorit product sename
        /// </summary>
        public string ProductSeName { get; set; }

        /// <summary>
        /// favorit product short description
        /// </summary>
        public string ProductDescription { get; set; }
        
        /// <summary>
        /// favorit product item type
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// String that represents path to favoriit product category
        /// </summary>
        public string CategoryString { get; set; }

        /// <summary>
        /// favorit product brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// favorit product company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// favorit product company SeName
        /// </summary>
        public string CompanySeName { get; set; }

        public int ProductId { get; set; }
    }
}