using FluentValidation;
using Nop.Admin.Models.Banner;
using Nop.Admin.Models.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators
{
    public partial class BannerValidator:AbstractValidator<BannerModel>
    {
        public BannerValidator()
        {
            //RuleFor(x => x.Url)
            //    .NotEmpty()
            //    .WithMessage("Url reqquired");
            //RuleFor(x => x.PictureId)
            //    .NotEmpty()
            //    .WithMessage("Picture reauired");
        }
    }
}