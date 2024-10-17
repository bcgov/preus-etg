using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantAgreementService : IService
	{
		GrantAgreement Get(int id);

		GrantAgreement Add(GrantAgreement newGrantAgreement);

		GrantAgreement Update(GrantAgreement grantAgreement);

		void AcceptGrantAgreement(GrantApplication grantApplication);

		void CancelGrantAgreement(GrantApplication grantApplication, string reason);

		void RejectGrantAgreement(GrantApplication grantApplication, string reason);

		DocumentTemplate GetDocumentTemplate(DocumentTypes documentType);

		void GenerateDocuments(GrantApplication grantApplication);

		void RemoveGrantAgreement(GrantApplication grantApplication);

		bool AgreementUpdateRequired(GrantApplication grantApplication);

		void UpdateAgreement(GrantApplication grantApplication);

	}
}
