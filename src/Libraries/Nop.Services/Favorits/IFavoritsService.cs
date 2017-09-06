using Nop.Core.Domain.Favorit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Favorits
{
    public interface IFavoritsService
    {
        /// <summary>
        /// Get favorit by id
        /// </summary>
        /// <param name="id">favorit id</param>
        /// <returns></returns>
        FavoritItem GetFavoritById(int id);

        /// <summary>
        /// Delete specified favorit
        /// </summary>
        /// <param name="item">favorit item to delete</param>
        void DeleteFavorit(FavoritItem item);

        /// <summary>
        /// Get customer favorits products
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns></returns>
        IList<FavoritItem> GetCustomerFavorits(int customerId);

        /// <summary>
        /// Add favorit item
        /// </summary>
        /// <param name="entity">favorit item to add</param>
        void Insert(FavoritItem entity);

        /// <summary>
        /// Check is this specified prodict favorit for this customer
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="productId">product id</param>
        /// <returns></returns>
        bool IsItemFavorit(int customerId, int productId);
    }
}
