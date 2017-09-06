using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Seo
{
    public class GoogleAnaliticsSettings:ISettings
    {
        /// <summary>
        /// Analitics cllient id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Analitics accountid
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Analitics Private key file Id
        /// </summary>
        public int PrivateKeyId { get; set; }
    }
}
