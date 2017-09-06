using Nop.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    public interface ICatalogExportManager
    {
        /// <summary>
        /// Generates excel template fo catalog upload
        /// </summary>
        /// <param name="categoryId">category of products for template</param>
        /// <param name="FileName">template result filename</param>
        /// File stream where to save generated excel
        /// <returns>template path</returns>
        string GenerateExcel(int categoryId, Stream stream, int languageId);

        /// <summary>
        /// Parse Excel file and get entered products
        /// </summary>
        /// <param name="file">file to parce</param>
        /// <param name="customerId">id of customer, who uploaded file</param>
        /// <param name="categoryId">id of category where to put products</param>
        /// <param name="productItemType">Id of item type</param>
        void ImportExcelFile(Download file, int customerId, int categoryId, int productItemType, IList<int> languages, string path, int languageid);
    }
}
