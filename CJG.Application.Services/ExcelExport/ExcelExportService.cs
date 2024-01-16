using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML;
using ClosedXML.Attributes;
using ClosedXML.Excel;

namespace CJG.Application.Services.ExcelExport
{
	public class ExcelExportService : IExcelExportService
	{
		public byte[] GetExcelContent<T>(IEnumerable<T> items, string worksheetName = null)
		{
			using var stream = new MemoryStream();

			var wb = new XLWorkbook();
			var ws = wb.AddWorksheet();

			if (!string.IsNullOrWhiteSpace(worksheetName))
				ws.Name = worksheetName;

			ws.ShowRowColHeaders = true;

			CreateExcelHeaders<T>(ws);

			ws.Cell("A2").InsertData(items.ToList());
			ws.Columns().AdjustToContents(5d, 50d);

			ws.Rows().Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
			ws.Rows().AdjustToContents();

			wb.SaveAs(stream);

			return stream.ToArray();
		}

		private void CreateExcelHeaders<T>(IXLWorksheet ws)
		{
			var itemProperties = typeof(T).GetProperties();
			foreach (var header in itemProperties)
			{
				if (!header.HasAttribute<XLColumnAttribute>())
					continue;

				var attribute = header.GetCustomAttributes().FirstOrDefault() as XLColumnAttribute;
				if (attribute == null)
					continue;

				if (attribute.Ignore)
					continue;

				var columnName = !string.IsNullOrWhiteSpace(attribute.Header) ? attribute.Header : header.Name;

				CreateHeaderCell(ws, attribute.Order, columnName);
			}
		}

		private static void CreateHeaderCell(IXLWorksheet ws, int columnIndex, string columnName)
		{
			var headerCell = ws.Cell(1, columnIndex);
			headerCell.SetValue(columnName);
			headerCell.Style.Font.Bold = true;
		}
	}
}
