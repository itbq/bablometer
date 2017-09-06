using Nop.Core.Domain.MiniSite;
using Nop.Web.Areas.MiniSite.Models.Preview;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class PreviewController :BaseNopController
    {
        public ActionResult Index(string CssFolderName, int LayoutType, string PageName)
        {
            if (!String.IsNullOrEmpty(PageName))
            {
                return View(PageName, "", new PreviewModel()
                    {
                        CssTemplateName = CssFolderName,
                        PageName = PageName
                    });
            }
            else
            {
                switch (LayoutType)
                {
                    case (int)LayoutHtmlType.First:
                        {
                            return View("1", "", new PreviewModel()
                            {
                                CssTemplateName = CssFolderName,
                                PageName = "1"
                            });
                        }
                    case (int)LayoutHtmlType.Second:
                        {
                            return View("2", "", new PreviewModel()
                            {
                                CssTemplateName = CssFolderName,
                                PageName = "2"
                            });
                        }
                    case (int)LayoutHtmlType.Third:
                        {
                            return View("3", "", new PreviewModel()
                            {
                                CssTemplateName = CssFolderName,
                                PageName = "3"
                            });
                        }
                }
            }
            return View();
        }

    }
}
