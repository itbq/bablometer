using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Topics;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using System;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class TopicController : BaseNopController
    {
        #region Fields
        private const int maxHeaderTopicsCount = 3;
        private const int maxFooterTopicsCount = 5;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public TopicController(ITopicService topicService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topics = _topicService.GetAllTopics()
                .OrderByDescending(x => x.Priority)
                .OrderByDescending(x => x.DisplayInMenu)
                .ToList();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => x.ToModel()),
                Total = topics.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topics = _topicService.GetAllTopics()
                .OrderByDescending(x => x.Priority)
                .OrderByDescending(x => x.DisplayInMenu)
                .ToList();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => x.ToModel()),
                Total = topics.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new TopicModel();   
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();
            if (model.DisplayInMenu)
            {
                int count = _topicService.GetHeaderTopicsCount();
                if (count == maxHeaderTopicsCount)
                {
                    ModelState.AddModelError("DisplayInMenu", _localizationService.GetResource("ITB.Admin.Topics.Header.MaxCount"));
                }
            }

            if (model.DisplayInFooterMenu)
            {
                int count = _topicService.GetFooterTopicsCount();
                if (count == maxFooterTopicsCount)
                {
                    ModelState.AddModelError("DisplayInFooterMenu", _localizationService.GetResource("ITB.Admin.Topics.Footer.MaxCount"));
                }
            }

            if (!String.IsNullOrEmpty(model.SystemName))
            {
                var oldTopic = _topicService.GetTopicBySystemName(model.SystemName);
                if (oldTopic != null)
                {
                    ModelState.AddModelError("SystemName", _localizationService.GetResource("ITB.Admin.Topics.SystemName.Exists"));
                }
            }
            if (ModelState.IsValid)
            {
                var topic = model.ToEntity();
                _topicService.InsertTopic(topic);   

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            var model = topic.ToModel();
            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");
            if (!String.IsNullOrEmpty(model.SystemName))
            {
                if (model.SystemName != topic.SystemName)
                {
                    var oldTopic = _topicService.GetTopicBySystemName(model.SystemName);
                    if (oldTopic != null)
                    {
                        ModelState.AddModelError("SystemName", _localizationService.GetResource("ITB.Admin.Topics.SystemName.Exists"));
                    }
                }
            }

            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");

            if (model.DisplayInMenu && !topic.DisplayInMenu)
            {
                int count = _topicService.GetHeaderTopicsCount();
                if (count == maxHeaderTopicsCount)
                {
                    ModelState.AddModelError("DisplayInMenu", _localizationService.GetResource("ITB.Admin.Topics.Header.MaxCount"));
                }
            }

            if (model.DisplayInFooterMenu && !topic.DisplayInFooterMenu)
            {
                int count = _topicService.GetFooterTopicsCount();
                if (count == maxFooterTopicsCount)
                {
                    ModelState.AddModelError("DisplayInFooterMenu", _localizationService.GetResource("ITB.Admin.Topics.Footer.MaxCount"));
                }
            }

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                _topicService.UpdateTopic(topic);
                
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", topic.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            _topicService.DeleteTopic(topic);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("List");
        }
        
        #endregion
    }
}
