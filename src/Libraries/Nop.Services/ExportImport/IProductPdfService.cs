using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    public interface IProductPdfService
    {
        void GenerateProductPdf(int productId, int languageId, MemoryStream stream);
        void GeneratePdfItextSharp(int productId, int languageId, MemoryStream stream, string imageFolderPath);
        void GenerateMobileProductPdf(int productId, int languageId, MemoryStream stream, string imageFolderpath);
    }
}
