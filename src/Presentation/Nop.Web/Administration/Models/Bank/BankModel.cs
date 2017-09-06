using Nop.Admin.Models.Media;
using Nop.Admin.Validators.Bank;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;

namespace Nop.Admin.Models.Bank
{
    [Validator(typeof(BankModelValidator))]
    public class BankModel : BaseNopEntityModel
    {        
        [NopResourceDisplayName("ITB.Admin.Bank.Title")]
        public string BankTitle { get; set; }

        [NopResourceDisplayName("ITB.Admin.Bank.Image")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        public string Email { get; set; }

        public string LogoUrl { get; set; }

        [NopResourceDisplayName("ITB.Admin.Bank.Rating")]
        public double Rating { get; set; }
    }
}