using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public interface ISliderItemService
    {
        IList<SliderItem> GetMailPageSlides();
        void Insert(SliderItem item);
        void Delete(SliderItem item);
        void Update(SliderItem item);
    }
}
