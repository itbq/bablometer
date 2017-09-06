using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public enum CustomerInformationProductSearchControlType
    {
        Gender = 1,
        TextBox = 2,
        NumberTextBoxExact = 3,
        NumberToddlerMore = 4,
        NumberToddlerLess = 5,
        NumberToddlerBetween = 6,
        DropDownList = 7,
        Checkboxes = 8,
        ToddlerString = 9,
        MoneyLess = 10,
        MoneyMore = 11,
        MonyBetween = 12
    }
}
