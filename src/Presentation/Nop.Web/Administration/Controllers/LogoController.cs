using Nop.Admin.Models.Common;
using Nop.Admin.Models.Media;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class LogoController : Controller
    {
        public ActionResult Index()
        {
            var model = new LogoModel();
            model.ImageUrl = "/Content/BSFA/images/logo.png";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LogoModel model)
        {
            if (ModelState.IsValid)
            {
                string filePath = Server.MapPath("~/Content/BSFA/images/logo.png");
                using (FileStream stream = new FileStream(filePath, FileMode.Truncate))
                {
                    model.File.InputStream.CopyTo(stream);
                    stream.Close();
                }
            }

            return Index();
        }
    }
}
