using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Event
{
    public partial class EventInfoModel: BaseNopEntityModel
    {
        public EventInfoModel()
        {
            PictureModel = new PictureModel();
            CatalogPictureModel = new PictureModel();
        }
        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public PictureModel PictureModel { get; set; }

        public PictureModel CatalogPictureModel { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string SeName { get; set; }

        public int PictureId { get; set; }

        public int CatalogPictureid { get; set; }
    }
}