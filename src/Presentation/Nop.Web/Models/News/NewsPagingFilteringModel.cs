using Nop.Web.Framework;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Validators.News;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using System.Web.Mvc;
using System.Threading;

namespace Nop.Web.Models.News
{
    [Validator(typeof(NewsPagingFilteringModelValidator))]
    public partial class NewsPagingFilteringModel : BasePageableModel
    {
        [UIHint("DateNullable")]
        [NopResourceDisplayName("Creation Date from")]
        public virtual DateTime? CreationStartDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Creation Date to")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public virtual DateTime? CreationEndDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Publish Date from")]
        public virtual DateTime? PublishStartDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Creation Date to")]
        public virtual DateTime? PublishEndDate { get; set; }

        [NopResourceDisplayName("Approved Status")]
        public bool? Approved { get; set; }

        public int CustomerId { get; set; }

        public int LanguageId { get; set; }

        public int DisplayPlace { get; set; }
    }
}