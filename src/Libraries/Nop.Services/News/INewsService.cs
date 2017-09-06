using Nop.Core;
using Nop.Core.Domain.News;
using System;
using System.Collections.Generic;

namespace Nop.Services.News
{
    /// <summary>
    /// News service interface
    /// </summary>
    public partial interface INewsService
    {
        /// <summary>
        /// Deletes a news
        /// </summary>
        /// <param name="newsItem">News item</param>
        void DeleteNews(NewsItem newsItem);

        /// <summary>
        /// Gets a news
        /// </summary>
        /// <param name="newsId">The news identifier</param>
        /// <returns>News</returns>
        NewsItem GetNewsById(int newsId);

        /// <summary>
        /// Gets all news
        /// </summary>
        /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>News items</returns>
        IPagedList<NewsItem> GetAllNews(int languageId,
            int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both));
        
        PagedList<NewsItem> GetAllCompanyNews(int languageId, int CustomerId,
            int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both));

        IPagedList<NewsItem> GetAllCompanyNews(int customerId, int pageIndex, int pageSize, bool showHidden = false, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both));

        IPagedList<NewsItem> GetAllCompanyNews(int languageId, int customerId, int pageIndex, int pageSize,
            DateTime? startCreationDate, DateTime? endCreationDate, DateTime? startPublishDate,
            DateTime? endPublishDate, bool? approved, int miniSite = (int)(NewsDisplayEnum.Main | NewsDisplayEnum.Both));
        /// <summary>
        /// Inserts a news item
        /// </summary>
        /// <param name="news">News item</param>
        void InsertNews(NewsItem news);

        /// <summary>
        /// Updates the news item
        /// </summary>
        /// <param name="news">News item</param>
        void UpdateNews(NewsItem news);

        /// <summary>
        /// Update news item comment totals
        /// </summary>
        /// <param name="newsItem">News item</param>
        void UpdateCommentTotals(NewsItem newsItem);

        /// <summary>
        /// Get featured newItem
        /// </summary>
        /// <returns></returns>
        NewsItem GetFeaturedNew(int languageId);

        IList<NewsItem> GetNews();
    }
}
