using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.NivoSlider.Models
{
    public class SliderItemListModel
    {
        public IList<SliderItemModel> Slides { get; set; }
        public bool Returning { get; set; }
    }
}
