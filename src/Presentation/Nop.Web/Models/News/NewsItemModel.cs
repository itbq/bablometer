using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.News;
using Nop.Web.Framework;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.News
{
    [Validator(typeof(NewsItemValidator))]
    public partial class NewsItemModel : BaseNopEntityModel
    {
        public NewsItemModel()
        {
            Comments = new List<NewsCommentModel>();
            AddNewComment = new AddNewsCommentModel();
        }

        public string SeName { get; set; }

        [NopResourceDisplayName("Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("Short Description")]
        [AllowHtml]
        public string Short { get; set; }

        [NopResourceDisplayName("Full Description")]
        [AllowHtml]
        public string Full { get; set; }

        [UIHint("NewsPicture")]
        public int PictureId { get; set; }

        [UIHint("NewsPicture")]
        public int CatalogPictureId { get; set; }

        public PictureModel CatalogPicture { get; set; }
        public PictureModel Picture { get; set; }
        public bool AllowComments { get; set; }

        public int NumberOfComments { get; set; }

        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode=true,DataFormatString = "DD.MM.YY")]
        public DateTime StartDate { get; set; }

        [NopResourceDisplayName("End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }


        public int CustomerId { get; set; }

        public string Company { get; set; }
        public string CompanySeName { get; set; }

        public int CompanyId { get; set; }

        public IList<LanguageModel> AviableLanguages { get; set; }
        public IList<NewsCommentModel> Comments { get; set; }

        public int WorkingLanguageId { get; set; }

        public int Language { get; set; }

        public AddNewsCommentModel AddNewComment { get; set; }

        public DateTime? PublishingDate { get; set; }

        public bool Published { get; set; }

        public int PageNumber { get; set; }

        public bool Featured { get; set; }

        public string MiniSite { get; set; }

        public int ExtendedProfileDisplay { get; set; }
    }
}