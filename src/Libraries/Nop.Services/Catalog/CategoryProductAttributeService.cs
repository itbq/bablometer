using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    public partial class CategoryProductAttributeService : ICategoryProductAttributeService
    {
        #region Constants

        private const string CATEGORYPRODUCTATTRIBUTEGROUPS_ALL_KEY = "Nop.categoryproductattributegroup.all";
        private const string CATEGORYPRODUCTATTRIBUTEGROUPS_BY_ID_KEY = "Nop.categoryproductattributegroup.id-{0}";
        private const string CATEGORYPRODUCTATTRIBUTEGROUPS_PATTERN_KEY = "Nop.categoryproductattributegroup.";

        private const string CATEGORYPRODUCTATTRIBUTES_ALL_KEY = "Nop.categoryproductattribute.all";
        private const string CATEGORYPRODUCTATTRIBUTES_BY_ID_KEY = "Nop.categoryproductattribute.id-{0}";
        private const string CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY = "Nop.categoryproductattribute.";

        private const string CATEGORYPRODUCTATTRIBUTEVALUES_ALL_KEY = "Nop.categoryproductattributevalue.all-{0}";
        private const string CATEGORYPRODUCTATTRIBUTEVALUES_BY_ID_KEY = "Nop.categoryproductattributevalue.id-{0}";
        private const string CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY = "Nop.categoryproductattributevalue.";
        private const string CATEGORYPRODUCTATTRIBUTEVALUES_BY_CATEGORY_AND_GROUP_KEY = "Nop.categoryproductattributevalue.cag-{0}-{1}";
        private const string CATEGORYPRODUCTATTRIBUTEVALUES_BY_CATEGORY_KEY = "Nop.categoryproductattributevalue.cag-{0}";

        private const string CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_ALL_KEY = "Nop.categorytocategoryproductattributegroup.all-{0}";
        private const string CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_BY_ID_KEY = "Nop.categorytocategoryproductattributegroup.id-{0}";
        private const string CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_PATTERN_KEY = "Nop.categorytocategoryproductattributegroup.";


        #endregion

        #region Fields

        private readonly IRepository<CategoryProductAttribute> _categoryProductAttributeRepository;
        private readonly IRepository<CategoryProductAttributeGroup> _categoryProductAttributeGroupRepository;
        private readonly IRepository<CategoryProductAttributeValue> _categoryProductAttributeValueRepository;
        private readonly IRepository<CategoryToCategoryProductAttributeGroup> _categoryToCategoryProductAttributeGroupRepository;

        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region ctor

        public CategoryProductAttributeService(ICacheManager cacheManager,
           IRepository<CategoryProductAttribute> categoryProductAttributeRepository,
           IRepository<CategoryProductAttributeGroup> categoryProductAttributeGroupRepository,
           IRepository<CategoryProductAttributeValue> categoryProductAttributeValueRepository,
            IRepository<CategoryToCategoryProductAttributeGroup> categoryToCategoryProductAttributeGroupRepository,
           IEventPublisher eventPublisher
           )
        {
            _cacheManager = cacheManager;
            _categoryProductAttributeRepository = categoryProductAttributeRepository;
            _categoryProductAttributeGroupRepository = categoryProductAttributeGroupRepository;
            _categoryProductAttributeValueRepository = categoryProductAttributeValueRepository;
            _categoryToCategoryProductAttributeGroupRepository = categoryToCategoryProductAttributeGroupRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region product category group
        public IList<CategoryProductAttributeGroup> GetAllCategoryProductAttributeGroups()
        {

            string key = CATEGORYPRODUCTATTRIBUTEGROUPS_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _categoryProductAttributeGroupRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = query.ToList();
                return productAttributes;
            });

        }
        public void DeleteCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryProductAttributeGroup categoryProductAttributeGroup)
        {
            if (categoryProductAttributeGroup == null)
                throw new ArgumentNullException("categoryProductAttributeGroup");

            _categoryProductAttributeGroupRepository.Delete(categoryProductAttributeGroup);

            //cache
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEGROUPS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryProductAttributeGroup);
        }

        public Core.Domain.Catalog.CategoryProductAttributeGroup GetCategoryProductAttributeGroupById(int categoryProductAttributeGroupId)
        {
            if (categoryProductAttributeGroupId == 0)
                return null;

            string key = string.Format(CATEGORYPRODUCTATTRIBUTEGROUPS_BY_ID_KEY, categoryProductAttributeGroupId);
            return _cacheManager.Get(key, () =>
            {
                var pa = _categoryProductAttributeGroupRepository.GetById(categoryProductAttributeGroupId);
                return pa;
            });
        }

        public void InsertCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryProductAttributeGroup categoryProductAttributeGroup)
        {
            if (categoryProductAttributeGroup == null)
                throw new ArgumentNullException("categoryProductAttributeGroup");

            _categoryProductAttributeGroupRepository.Insert(categoryProductAttributeGroup);

            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEGROUPS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryProductAttributeGroup);
        }

        public void UpdateCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryProductAttributeGroup categoryProductAttributeGroup)
        {
            if (categoryProductAttributeGroup == null)
                throw new ArgumentNullException("categoryProductAttributeGroup");

            _categoryProductAttributeGroupRepository.Update(categoryProductAttributeGroup);

            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEGROUPS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryProductAttributeGroup);
        }
        #endregion

        #region product category attribute

        public void DeleteCategoryProductAttribute(Core.Domain.Catalog.CategoryProductAttribute categoryProductAttribute)
        {
            if (categoryProductAttribute == null)
                throw new ArgumentNullException("categoryProductAttribute");

            _categoryProductAttributeRepository.Delete(categoryProductAttribute);

            //cache            
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryProductAttribute);
        }

        public IList<Core.Domain.Catalog.CategoryProductAttribute> GetCategoryProductAttributesByCategoryProductGroupId(int categoryProductGroupId)
        {
            string key = string.Format(CATEGORYPRODUCTATTRIBUTES_ALL_KEY, categoryProductGroupId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _categoryProductAttributeRepository.Table
                            orderby pva.DisplayOrder
                            where pva.CategoryProductGroupId == categoryProductGroupId
                            select pva;
                var categoryproductattributes = query.ToList();
                return categoryproductattributes;
            });
        }

        public Core.Domain.Catalog.CategoryProductAttribute GetCategoryProductAttributeById(int categoryProductAttributeId)
        {
            if (categoryProductAttributeId == 0)
                return null;

            string key = string.Format(CATEGORYPRODUCTATTRIBUTES_BY_ID_KEY, categoryProductAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var pa = _categoryProductAttributeRepository.GetById(categoryProductAttributeId);
                return pa;
            });
        }

        public void InsertCategoryProductAttribute(Core.Domain.Catalog.CategoryProductAttribute categoryProductAttribute)
        {
            if (categoryProductAttribute == null)
                throw new ArgumentNullException("categoryProductAttribute");

            _categoryProductAttributeRepository.Insert(categoryProductAttribute);


            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryProductAttribute);
        }

        public void UpdateCategoryProductAttribute(Core.Domain.Catalog.CategoryProductAttribute categoryProductAttribute)
        {
            if (categoryProductAttribute == null)
                throw new ArgumentNullException("categoryProductAttribute");

            _categoryProductAttributeRepository.Update(categoryProductAttribute);


            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryProductAttribute);
        }

        #endregion

        #region product category attribute value

        public void DeleteCategoryProductAttributeValue(Core.Domain.Catalog.CategoryProductAttributeValue categoryProductAttributeValue)
        {
            if (categoryProductAttributeValue == null)
                throw new ArgumentNullException("categoryProductAttributeValue");

            _categoryProductAttributeValueRepository.Delete(categoryProductAttributeValue);

            //cache            

            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(categoryProductAttributeValue);
        }

        public IList<Core.Domain.Catalog.CategoryProductAttributeValue> GetCategoryProductAttributeValues(int categoryProductAttributeId)
        {
            string key = string.Format(CATEGORYPRODUCTATTRIBUTEVALUES_ALL_KEY, categoryProductAttributeId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _categoryProductAttributeValueRepository.Table
                            orderby pva.DisplayOrder
                            where pva.CategoryProductAttributeId == categoryProductAttributeId
                            select pva;
                var categoryproductattributes = query.ToList();
                return categoryproductattributes;
            });
        }

        public Core.Domain.Catalog.CategoryProductAttributeValue GetCategoryProductAttributeValueById(int categoryProductAttributeValueId)
        {
            if (categoryProductAttributeValueId == 0)
                return null;

            string key = string.Format(CATEGORYPRODUCTATTRIBUTEVALUES_BY_ID_KEY, categoryProductAttributeValueId);
            return _cacheManager.Get(key, () =>
            {
                var pa = _categoryProductAttributeValueRepository.GetById(categoryProductAttributeValueId);
                return pa;
            });
        }

        public void InsertCategoryProductAttributeValue(Core.Domain.Catalog.CategoryProductAttributeValue categoryProductAttributeValue)
        {
            if (categoryProductAttributeValue == null)
                throw new ArgumentNullException("categoryProductAttributeValue");

            _categoryProductAttributeValueRepository.Insert(categoryProductAttributeValue);

            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(categoryProductAttributeValue);
        }

        public void UpdateCategoryProductAttributeValue(Core.Domain.Catalog.CategoryProductAttributeValue categoryProductAttributeValue)
        {
            if (categoryProductAttributeValue == null)
                throw new ArgumentNullException("categoryProductAttributeValue");

            _categoryProductAttributeValueRepository.Update(categoryProductAttributeValue);

            _cacheManager.RemoveByPattern(CATEGORYPRODUCTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(categoryProductAttributeValue);
        }
        #endregion

        #region category to category product attribute

        public void DeleteCategoryToCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryToCategoryProductAttributeGroup item)
        {
            if (item == null)
                throw new ArgumentNullException("CategoryToCategoryProductAttributeGroup");

            _categoryToCategoryProductAttributeGroupRepository.Delete(item);

            //cache            
            _cacheManager.RemoveByPattern(CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(item);
        }

        public CategoryToCategoryProductAttributeGroup GetCategoryToCategoryProductAttributeGroupById(int id)
        {
            if (id == 0)
                return null;

            string key = string.Format(CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_BY_ID_KEY, id);
            return _cacheManager.Get(key, () =>
            {
                var pa = _categoryToCategoryProductAttributeGroupRepository.GetById(id);
                return pa;
            });
        }

        public void InsertCategoryToCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryToCategoryProductAttributeGroup item)
        {
            if (item == null)
                throw new ArgumentNullException("CategoryToCategoryProductAttributeGroup");

            _categoryToCategoryProductAttributeGroupRepository.Insert(item);

            _cacheManager.RemoveByPattern(CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(item);
        }

        public void UpdateCategoryToCategoryProductAttributeGroup(Core.Domain.Catalog.CategoryToCategoryProductAttributeGroup item)
        {
            if (item == null)
                throw new ArgumentNullException("CategoryToCategoryProductAttributeGroup");

            _categoryToCategoryProductAttributeGroupRepository.Update(item);


            _cacheManager.RemoveByPattern(CATEGORYTOCATEGORYPRODUCTATTRIBUTEGROUP_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(item);
        }

        public CategoryToCategoryProductAttributeGroup GetCategoryToCategoryProductAttributeGroupByCategoryIdAndGroupId(int categoryId, int groupId)
        {
            if (categoryId == 0 || groupId == 0)
                return null;

            string key = string.Format(CATEGORYPRODUCTATTRIBUTEVALUES_BY_CATEGORY_AND_GROUP_KEY, categoryId, groupId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _categoryToCategoryProductAttributeGroupRepository.Table
                            orderby pva.DisplayOrder
                            where pva.CategoryId == categoryId && pva.CategoryProductAttributeGroupId == groupId
                            select pva;
                var categoryproductattribute = query.FirstOrDefault();
                return categoryproductattribute;
            });
        }

        public IList<CategoryToCategoryProductAttributeGroup> GetCategoryToCategoryProductAttributeGroupByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return null;
            string key = string.Format(CATEGORYPRODUCTATTRIBUTEVALUES_BY_CATEGORY_KEY, categoryId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _categoryToCategoryProductAttributeGroupRepository.Table
                            orderby pva.DisplayOrder
                            where pva.CategoryId == categoryId
                            select pva;
                var categoryproductattributes = query.ToList();
                return categoryproductattributes;
            });

        }
        #endregion

    }
}
