using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IAttachmentService : IService
	{
		Attachment Get(int id);
		Attachment GetAttachment(int grantApplicationId, int attachmentId);
		Attachment GetBusinessLicenseAttachment(int organizationId, int attachmentId);
		Attachment Update(Attachment attachment, bool commit = false);

		Attachment Add(Attachment attachment, bool commit = false);
		void Delete(Attachment attachment);
	}
}
