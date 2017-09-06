using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Topics;
using System.Collections.Generic;

namespace Nop.Web.Models.Common
{
    public partial class FooterModel : BaseNopModel
    {
        public string StoreName { get; set; }
        public IList<TopicModel> Topics { get; set; } 
    }
}