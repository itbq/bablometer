namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an attribute control type
    /// </summary>
    public enum AttributeControlType
    {
        /// <summary>
        /// Dropdown list
        /// </summary>
        DropdownList = 1,
        /// <summary>
        /// Checkboxes
        /// </summary>
        Checkboxes = 3,
        /// <summary>
        /// TextBox
        /// </summary>
        TextBox = 4,
        /// <summary>
        /// Toddler int Picker
        /// </summary>
        ToddlerInt = 70,
        /// <summary>
        /// Money Picker
        /// </summary>
        Money = 90,
        /// <summary>
        /// Money range picker
        /// </summary>
        MoneyRange = 100,
    }
}