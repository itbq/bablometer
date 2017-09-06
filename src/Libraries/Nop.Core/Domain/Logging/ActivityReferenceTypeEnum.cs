using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Logging
{
    public enum ActivityReferenceTypeEnum
    {
        None = 0,
        Product = 1,
        Category = 2,
        Search = 3,
        Banner = 4,
        BannerClick = 5,
        InnerPage = 6
    }
}
