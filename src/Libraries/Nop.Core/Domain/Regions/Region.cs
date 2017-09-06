using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Regions
{
    public class Region : BaseEntity
    {
        private ICollection<City> _cities;
        private ICollection<Product> _products;
        /// <summary>
        /// Region name
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// region code
        /// </summary>
        public virtual int Code { get; set; }

        /// <summary>
        /// region cities
        /// </summary>
        public virtual ICollection<City> Cities
        {
            get { return _cities ?? (_cities = new List<City>()); }
            set { this._cities = value; }
        }

        public virtual ICollection<Product> Products
        {
            get { return _products ?? (_products = new List<Product>()); }
            set {this._products = value;}
        }
    }
}
