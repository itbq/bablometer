using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.News;
using Nop.Services.Events;
using System.Collections.Generic;
using Nop.Services.Helpers;

namespace Nop.Services.News
{
    /// <summary>
    /// News service
    /// </summary>
    public partial class NewsService : INewsService
    {
        #region Constants
        private const string NEWS_BY_ID_KEY = "Nop.news.id-{0}";
        private const string NEWS_FEATURED = "Nop.news.featured";
        private const string NEWS_PATTERN_KEY = "Nop.news.";
        #endregion

        #region Fields

        private readonly IRepository<NewsItem> _newsItemRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public NewsService(IRepository<NewsItem> newsItemRepository, ICacheManager cacheManager, IEventPublisher eventPublisher,
            IDateTimeHelper dateTimeHelper)
        {
            _newsItemRepository = newsItemRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        public virtual void DeleteNews(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            _newsItemRepository.Delete(newsItem);

            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(newsItem);
        }

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        public virtual NewsItem GetNewsById(int newsId)
        {
            if (newsId == 0)
                return null;

            string key = string.Format(NEWS_BY_ID_KEY, newsId);
            return _cacheManager.Get(key, () =>
            {
                var n = _newsItemRepository.GetById(newsId);
                return n;
            });
        }

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        public virtual IPagedList<NewsItem> GetAllNews(int languageId,
            int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both))
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }
            query = query.Where(n => ((n.ExtendedProfileOnly & miniSite) == n.ExtendedProfileOnly));
            query = query.OrderByDescending(b => b.PublishingDate);

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        public virtual IPagedList<NewsItem> GetAllCompanyNews(int customerId, int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both))
        {
            var query = _newsItemRepository.Table;
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.CustomerId == customerId);
            }
            query = query.Where(n => ((n.ExtendedProfileOnly & miniSite) == n.ExtendedProfileOnly));
            query = query.OrderByDescending(b => b.CreatedOnUtc);

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }

        public virtual PagedList<NewsItem> GetAllCompanyNews(int languageId, int CustomerId,
            int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both))
        {
            var query = _newsItemRepository.Table;
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);
            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
                if (CustomerId > 0)
                {
                    query = query.Where(n => n.CustomerId == CustomerId);
                }
                else
                {
                    if (CustomerId != -1)
                    {
                        query = query.Where(n => n.CustomerId == null);
                    }
                    else
                    {
                        query = query.Where(n => n.CustomerId != null);
                    }
                }
            }
            query = query.Where(n => ((n.ExtendedProfileOnly & miniSite) == n.ExtendedProfileOnly));
            query = query.OrderByDescending(b => b.PublishingDate);

            var news = new PagedList<NewsItem>(query, pageIndex, pageSize);
            return news;
        }
        
        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void InsertNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Insert(news);

            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(news);
        }

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        public virtual void UpdateNews(NewsItem news)
        {
            if (news == null)
                throw new ArgumentNullException("news");

            _newsItemRepository.Update(news);

            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(news);
        }
        
        /// <summary>
        /// Update news item comment totals
        /// </summary>
        /// <param name="newsItem">News item</param>
        public virtual void UpdateCommentTotals(NewsItem newsItem)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            int approvedCommentCount = 0;
            int notApprovedCommentCount = 0;
            var newsComments = newsItem.NewsComments;
            foreach (var nc in newsComments)
            {
                if (nc.IsApproved)
                    approvedCommentCount++;
                else
                    notApprovedCommentCount++;
            }

            newsItem.ApprovedCommentCount = approvedCommentCount;
            newsItem.NotApprovedCommentCount = notApprovedCommentCount;
            UpdateNews(newsItem);
        }

        #endregion


        public IPagedList<NewsItem> GetAllCompanyNews(int languageId, int customerId, int pageIndex, int pageSize, DateTime? startCreationDate, DateTime? endCreationDate, DateTime? startPublishDate, DateTime? endPublishDate, bool? approved, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both))
        {
            if (approved.HasValue && !approved.Value && (startPublishDate.HasValue || endPublishDate.HasValue))
            {
                return new PagedList<NewsItem>(new List<NewsItem>(), pageIndex, pageSize);
            }

            var query = _newsItemRepository.Table;
            var utcNow = DateTime.UtcNow;
            IQueryable<NewsItem> news = query.Where(n => n.CustomerId == customerId);
            if (languageId > 0)
                news = news.Where(n => n.LanguageId == languageId);
                //.Where(n=>n.LanguageId == languageId);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            if (startPublishDate.HasValue || endPublishDate.HasValue)
                approved = true;
            if (approved.HasValue && approved.Value)
            {
                news = news.Where(x => x.Published == approved.Value);
                if (approved.Value)
                {
                    if (startPublishDate.HasValue)
                    {
                        startPublishDate = _dateTimeHelper.ConvertToUtcTime(startPublishDate.Value.AddHours(23).AddMinutes(59), timeZone);
                        news = news.Where(x => x.PublishingDate.Value >= startPublishDate.Value);
                    }

                    if (endPublishDate.HasValue)
                    {
                        endPublishDate = _dateTimeHelper.ConvertToUtcTime(endPublishDate.Value.AddHours(23).AddMinutes(59), timeZone);
                        news = news.Where(x => x.PublishingDate.Value <= endPublishDate.Value);
                    }
                }
            }
            else
            {
                if(approved.HasValue)
                    news = news.Where(x => !x.Published);
            }
            
            if (startCreationDate.HasValue)
            {
                startCreationDate = _dateTimeHelper.ConvertToUtcTime(startCreationDate.Value.AddHours(23).AddMinutes(59), timeZone);
                news = news.Where(x => x.CreatedOnUtc >= startCreationDate.Value);
            }

            if (endCreationDate.HasValue)
            {
                endCreationDate = _dateTimeHelper.ConvertToUtcTime(endCreationDate.Value.AddHours(23).AddMinutes(59), timeZone);
                news = news.Where(x => x.CreatedOnUtc <= endCreationDate.Value);
            }
            news = news.Where(n => ((n.ExtendedProfileOnly & miniSite) == n.ExtendedProfileOnly));
            news = news.OrderByDescending(b => b.CreatedOnUtc);

            var resultNews = new PagedList<NewsItem>(news, pageIndex, pageSize);
            return resultNews;
        }

        public NewsItem GetFeaturedNew(int languageId)
        {
            string key = string.Format(NEWS_FEATURED);
            return _cacheManager.Get(key, () =>
            {
                var n = _newsItemRepository.Table.Where(x => x.Featured && x.LanguageId == languageId && !((x.ExtendedProfileOnly & (int)NewsDisplayEnum.MiniSite) == (int)NewsDisplayEnum.MiniSite)).FirstOrDefault();
                return n;
            });
        }

        public IList<NewsItem> GetNews()
        {
            return _newsItemRepository.Table.ToList();
        }
    }
}
