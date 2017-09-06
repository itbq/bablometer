using FluentValidation;
using Nop.Web.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.News
{
    public class NewsPagingFilteringModelValidator:AbstractValidator<NewsPagingFilteringModel>
    {
        public NewsPagingFilteringModelValidator()
        {
        }
    }
}