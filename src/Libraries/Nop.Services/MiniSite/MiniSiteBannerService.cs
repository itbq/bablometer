using Nop.Core.Data;
using Nop.Core.Domain.MiniSite;
using Nop.Services.Events;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public class MiniSiteBannerService : IMiniSiteBannerService
    {
        private readonly IRepository<BannerMiniSite> _repository;
        private readonly IPictureService _pictureService;
        private readonly IEventPublisher _eventPublisher;

        public MiniSiteBannerService(IRepository<BannerMiniSite> repository,
            IPictureService pictureService,
            IEventPublisher eventPublisher)
        {
            this._repository  = repository;
            this._pictureService = pictureService;
            this._eventPublisher = eventPublisher;
        }

        public BannerMiniSite GetById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("Bannerminisite");
            return _repository.GetById(id);
        }

        public void Insert(BannerMiniSite banner)
        {
            if (banner == null)
                throw new ArgumentNullException("BannerMiniSite");
            _repository.Insert(banner);

            _eventPublisher.EntityInserted(banner);
        }


        public void Delete(BannerMiniSite banner)
        {
            if (banner == null)
                throw new ArgumentNullException("BannerMiniSite");
            if (banner.BannerPictureId != 0)
            {
                _pictureService.DeletePicture(_pictureService.GetPictureById(banner.BannerPictureId));
            }
            _repository.Delete(banner);

            _eventPublisher.EntityDeleted(banner);
        }

        public void Update(BannerMiniSite banner)
        {
            if (banner == null)
                throw new ArgumentNullException("BannerMiniSite");
            _repository.Update(banner);

            _eventPublisher.EntityUpdated(banner);
        }
    }
}
