using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public interface IConversionImageService
    {
        void Insert(ConversionImage image);
        void Update(ConversionImage image);
        void Delete(ConversionImage image);
        ConversionImage GetConversionImageById(int id);
    }
}
