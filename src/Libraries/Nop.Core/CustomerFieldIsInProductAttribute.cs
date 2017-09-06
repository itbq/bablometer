using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class CustomerFieldIsInProductAttribute:System.Attribute
    {
        private string _alias;
        public string Alias { get { return _alias; } }
        public CustomerFieldIsInProductAttribute(string alias)
        {
            this._alias = alias;
        }
    }
}
