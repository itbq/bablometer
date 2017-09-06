using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class HomeController : BaseNopController
    {

        public ActionResult Index()
        {
            return View();
        }

    }
}
