PRINT 'Inserting [ClaimParticipants]'

-- Copy back into the table the claim participants from the backup.
INSERT [dbo].[ClaimParticipants]
 ([ClaimId], [ClaimVersion], [ParticipantFormId]) 
SELECT DISTINCT
	ClaimId,
	ClaimVersion,
	ParticipantFormId
FROM #ClaimParticipants

-- Insert new claim participants by determining which participants are part of which claim (this is for ETG).
INSERT [dbo].[ClaimParticipants]
 ([ClaimId], [ClaimVersion], [ParticipantFormId]) 
SELECT DISTINCT
	c.Id,
	c.ClaimVersion,
	pc.ParticipantFormId
FROM dbo.[Claims] c
INNER JOIN dbo.[ClaimEligibleCosts] cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
INNER JOIN dbo.[ParticipantCosts] pc ON cec.Id = pc.ClaimEligibleCostId

DROP TABLE #ClaimParticipants

CHECKPOINT