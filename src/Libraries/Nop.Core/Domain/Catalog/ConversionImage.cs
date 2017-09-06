using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class ConversionImage : BaseEntity, ILocalizedEntity
    {
        public virtual int PictureId {get; set;}
        public virtual int CategoryAttributeGroupId { get; set; }
        public virtual CategoryProductAttributeGroup AttributeGroup { get; set; }
        public virtual string Name { get; set; }
    }
}
