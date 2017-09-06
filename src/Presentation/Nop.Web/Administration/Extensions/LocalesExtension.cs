using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Event;
using Nop.Admin.Models.Footer;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Extensions
{
    public static class LocalesExtension
    {
        public static bool CheckLocales(this EventModel entity)
        {
            if (entity.Locales.Count == 0)
            {
                return false;
            }
            var locale = entity.Locales
                .Where(x => x.Title != null && x.ShortDescription != null && x.FullDescription != null)
                .ToList();
            if (locale.Count() > 0)
            {
                entity.Title = locale[0].Title;
                entity.ShortDescription = locale[0].ShortDescription;
                entity.FullDescription = locale[0].FullDescription;
                return true;
            }
            return false;
        }


        public static bool CheckLocales(this AddressModel model)
        {
            if (model.Locales.Count == 0)
            {
                return false;
            }
            var locale = model.Locales
                .Where(x => x.City != null && x.Address1 != null && x.FirstName != null)
                .ToList();
            if (locale.Count() > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// some method that checks are aviable valid  model value in loacles 
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="entity">model</param>
        /// <param name="checker">function that checks is selected locale  valid</param>
        /// <param name="processor">function that saves valid locale to entity</param>
        /// <returns>true if there is at least one correct locale value</returns>
        public static bool CheckLocales<T>(this T entity, Func<T, bool> checker,Action<T,T> processor) where T : ILocalizedModel<T>
        {
            var locale = entity.Locales
                .Where(x => checker(x)).ToList();
            if (locale.Count > 0)
            {
                processor(locale[0],entity);
                return true;
            }
            return false;
        }
    }
}