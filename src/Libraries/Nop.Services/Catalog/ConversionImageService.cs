using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public class ConversionImageService : IConversionImageService
    {
        private readonly IRepository<ConversionImage> _repository;
        private readonly IEventPublisher _eventPublisher;

        public ConversionImageService(IRepository<ConversionImage> repository,
            IEventPublisher eventPublisher)
        {
            this._repository = repository;
            this._eventPublisher = eventPublisher;
        }

        public void Insert(ConversionImage image)
        {
            if (image == null)
                throw new ArgumentNullException("Conversion image");

            _repository.Insert(image);

            _eventPublisher.EntityInserted(image);
        }

        public void Update(ConversionImage image)
        {
            if (image == null)
                throw new ArgumentNullException("Conversion image");

            _repository.Update(image);

            _eventPublisher.EntityUpdated(image);
        }

        public void Delete(ConversionImage image)
        {
            if (image == null)
                throw new ArgumentNullException("Conversion image");

            _repository.Delete(image);
            _eventPublisher.EntityDeleted(image);
        }

        public ConversionImage GetConversionImageById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
