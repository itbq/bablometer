using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public enum CustomerInformationProductAddControlType
    {
        TextBox = 2,
        NumberTextBoxRange = 3,
        NumberTextBoxValue = 4,
        DropDownList = 6,
        CheckBoxes = 7,
        ReferenceValue = 8,
        NumberTextBoxLess = 9,
        NumberTetBoxMore = 10,
        MoneyLess = 11,
        MoneyMore = 12,
        MoneyBetween = 13,
    }
}
