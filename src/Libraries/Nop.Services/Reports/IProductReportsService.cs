using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Reports
{
    public interface IProductReportsService
    {
        void GenerateReport(ReportTypesEnum reportType, int referenceId, Stream stream, DateTime? startDate, DateTime? endDate);
    }
}
