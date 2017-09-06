using Nop.Admin.Models.MiniSite;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.MiniSite;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.MiniSite;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class MiniSitesController : BaseNopController
    {
        private readonly IMiniSiteService _miniSiteService;
        private readonly IMiniSiteEmailSender _miniSiteEmailSender;
        private readonly ILanguageService _languageService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly StoreInformationSettings _storeInformationSettings; 

        public MiniSitesController(IMiniSiteService miniSiteService,
            IMiniSiteEmailSender miniSiteEmailSender,
            ILanguageService languageService,
            IPictureService pictureService,
            IWebHelper webHelper,
            StoreInformationSettings storeInformationSettings)
        {
            this._miniSiteService = miniSiteService;
            this._miniSiteEmailSender = miniSiteEmailSender;
            this._languageService = languageService;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._storeInformationSettings = storeInformationSettings;
        }


        public ActionResult List()
        {
            var miniSites = _miniSiteService.GetAllMiniSites();
            var model = new GridModel<MiniSiteShortModel>();
            model.Data = miniSites.Select(x => new MiniSiteShortModel()
            {
                DomainName = x.DomainName,
                UserName = x.Customer.Username,
                Active = x.Active,
                Id = x.Id
            });

            model.Total = miniSites.Count();
            return View(model);
        }

        [HttpPost,GridAction]
        public ActionResult List(GridCommand command)
        {
            var miniSites = _miniSiteService.GetAllMiniSites();
            var model = new GridModel<MiniSiteShortModel>();
            model.Data = miniSites.Select(x => new MiniSiteShortModel()
            {
                DomainName = x.DomainName,
                UserName = x.Customer.Username,
                Active = x.Active,
                Id = x.Id
            });

            model.Total = miniSites.Count();

            return new JsonResult()
            {
                Data = model
            };
        }

        [HttpPost, GridAction]
        public ActionResult Edit(MiniSiteShortModel model, GridCommand command)
        {
            string storeUrl = (new Uri(_storeInformationSettings.StoreUrl)).Host;
            var miniSite = _miniSiteService.GetMiniSiteById(model.Id);
            if (miniSite.Active != model.Active)
            {
                if (miniSite.Active)
                {
                    _miniSiteEmailSender.SendMiniSiteRejectEmail(miniSite.Id, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id);
                    try
                    {
                        string result = _webHelper.DeleteWebSiteBinding(miniSite.DomainName, storeUrl);
                        SuccessNotification(result + "\nBinding deleted succesfully");
                    }
                    catch (FaultException ex)
                    {
                        if (ex.Message != "Binding not found!")
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    _miniSiteEmailSender.SendMiniSiteAcceptEmail(miniSite.Id, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id);
                    string result = _webHelper.AddWebSiteBinding(model.DomainName, storeUrl);
                    SuccessNotification(result + "\nBinding added succesfully");
                }
                miniSite.Active = model.Active;
            }
            else
            {
                if (miniSite.DomainName != model.DomainName)
                {
                    if (model.Active)
                    {
                        try
                        {
                            string result = _webHelper.DeleteWebSiteBinding(miniSite.DomainName, storeUrl);
                            SuccessNotification(result + "\nBinding deleted succesfully");
                        }
                        catch (FaultException ex)
                        {
                            if (ex.Message != "Binding not found!")
                            {
                                throw ex;
                            }
                        }
                        string res = _webHelper.AddWebSiteBinding(model.DomainName, storeUrl);
                        SuccessNotification(res + "\nBinding added succesfully");
                    }
                }
            }
            miniSite.DomainName = model.DomainName;
            _miniSiteService.UpdateMiniSite(miniSite);

            return List(command);
        }

        [HttpPost,GridAction]
        public ActionResult Delete(int id,GridCommand command)
        {
            string storeUrl = (new Uri(_storeInformationSettings.StoreUrl)).Host;
            var miniSite = _miniSiteService.GetMiniSiteById(id);
            _miniSiteEmailSender.SendMiniSiteRejectEmail(miniSite.Id, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id);
            if (miniSite.Active)
            {
                try
                {
                    string res = _webHelper.DeleteWebSiteBinding(miniSite.DomainName, storeUrl);
                    SuccessNotification(res + "\nBinding deleted succesfully");
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message != "Binding not found!")
                    {
                        throw ex;
                    }
                }
            }
            if (miniSite.LogoId != 0)
            {
                var picture = _pictureService.GetPictureById(miniSite.LogoId);
                if (picture != null)
                    _pictureService.DeletePicture(picture);
            }
            _miniSiteService.DeeteMiniSite(miniSite);

            return List(command);
        }
    }
}
