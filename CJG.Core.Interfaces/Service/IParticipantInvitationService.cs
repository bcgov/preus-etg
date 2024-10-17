using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IParticipantInvitationService : IService
	{
		ParticipantInvitation UpdateParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation RemoveParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation RemoveParticipantNotInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation SendParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation GetInvitation(int grantApplicationId, int invitationId);

		ParticipantInvitation CompleteIndividualInvitation(ParticipantForm participantForm, ParticipantInvitation participantInvitation);
	}
}