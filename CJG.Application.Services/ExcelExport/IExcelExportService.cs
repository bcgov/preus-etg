using System.Collections.Generic;

namespace CJG.Application.Services.ExcelExport
{
	public interface IExcelExportService
	{
		byte[] GetExcelContent<T>(IEnumerable<T> items, string worksheetName = null);
	}
}