using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.CompanyInformation
{
    public partial class CompanyListModel:BaseNopModel
    {
        public IList<CompanyInformationModel> Compannies { get; set; }
    }
}