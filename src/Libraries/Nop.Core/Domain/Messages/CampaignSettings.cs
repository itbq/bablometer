using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Messages
{
    public class CampaignSettings:ISettings
    {
        public DateTime LastNewsLetter { get; set; }
        public int DefaultItemCount { get; set; }
    }
}
