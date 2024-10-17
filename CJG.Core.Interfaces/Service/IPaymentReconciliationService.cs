using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System.IO;

namespace CJG.Core.Interfaces.Service
{
	public interface IPaymentReconciliationService : IService
	{
		PageList<ReconciliationReport> GetReports(int page = 1, int quantity = 10, string sort = "DateAdded");

		ReconciliationReport Reconcile(Stream stream, bool createNew = false);

		void ManualReconcile(ReconciliationPayment payment);
		void Delete(ReconciliationReport report);
	}
}
