using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Bank
{
    public class BankListModel
    {
        [NopResourceDisplayName("ITB.Admin.Bank.Title")]
        public string BankSearchName { get; set; }
        
        public GridModel<BankModel> BankList{get; set;}
    }
}