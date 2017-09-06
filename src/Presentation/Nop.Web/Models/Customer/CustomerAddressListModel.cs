using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using System.Web.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAddressListModel : BaseNopModel
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
            NewAddress = new AddressModel();
        }

        public int MaxContactCount { get; set; }
        public IList<AddressModel> Addresses { get; set; }
        public AddressModel NewAddress { get; set; }
        public CustomerNavigationModel NavigationModel { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public int LanguageId { get; set; }
    }
}