using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents attribute control type for search
    /// </summary>
    public enum  SearchAttributeControlType
    {
        /// <summary>
        /// Simple textbox with text
        /// </summary>
        TextBoxText = 1,
        /// <summary>
        /// Textbox with double validation
        /// </summary>
        TextBoxReal = 2,

        /// <summary>
        /// Toddler That selects less or equal
        /// </summary>
        ToddlerMin = 3,

        /// <summary>
        /// Toddler that selects more or equal
        /// </summary>
        ToddlerMax = 4,

        /// <summary>
        /// Checkboxes
        /// </summary>
        CheckBox = 5,

        /// <summary>
        /// Group of checkboxes
        /// </summary>
        CheckBoxGroup = 6,

        /// <summary>
        /// Dropdown with values
        /// </summary>
        DropDown = 7,

        /// <summary>
        /// Toddler with string values
        /// </summary>
        //ToddlerString = 8,

        /// <summary>
        /// Money conrol (double validation + Currency selector)
        /// </summary>
        Money = 9,

        /// <summary>
        /// Toddler with min and max values
        /// </summary>
        ToddlerIntBetween = 10,
    }
}
