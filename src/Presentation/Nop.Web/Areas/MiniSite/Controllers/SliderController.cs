using Nop.Core;
using Nop.Services.Media;
using Nop.Web.Areas.MiniSite.Models.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Localization;
using Nop.Web.Controllers;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class SliderController : BaseNopController
    {
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;

        public SliderController(IWorkContext workContext,
            IPictureService pictureService)
        {
            this._workContext = workContext;
            this._pictureService = pictureService;
        }

        public ActionResult Index()
        {
            var model = new List<SliderItemModel>();
            model = _workContext.CurrentMiniSite.SliderItems.Select(x => new SliderItemModel()
            {
                PictureUrl = _pictureService.GetPictureUrl(x.PictureId,showDefaultPicture:false),
                Text = x.GetLocalized(s=>s.ShortText,_workContext.WorkingLanguage.Id,false),
                Title = "<strong><a href=\"" + x.Url + "\" >" + x.GetLocalized(s => s.TitleText, _workContext.WorkingLanguage.Id, false) + "</a></strong><div>" + x.GetLocalized(s => s.ShortText, _workContext.WorkingLanguage.Id, false) + "</div>",
                Url = x.Url
            }).ToList();

            return View(model);
        }

    }
}
