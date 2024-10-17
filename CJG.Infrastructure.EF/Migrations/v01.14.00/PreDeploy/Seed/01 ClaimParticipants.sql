PRINT 'Backup [ClaimParticipants]'

CREATE TABLE #ClaimParticipants (
	[ClaimId] INT NOT NULL
	, [ClaimVersion] INT NOT NULL
	, [ParticipantFormId] INT NOT NULL
)

INSERT #ClaimParticipants
 ([ClaimId], [ClaimVersion], [ParticipantFormId]) 
SELECT
	ClaimId,
	ClaimVersion,
	ParticipantFormId
FROM [dbo].[ClaimParticipants]

CHECKPOINT