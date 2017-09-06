using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.News
{
    [Flags]
    public enum NewsDisplayEnum
    {
        Both = 1,
        MiniSite = 2,
        Main = 4
    }
}
