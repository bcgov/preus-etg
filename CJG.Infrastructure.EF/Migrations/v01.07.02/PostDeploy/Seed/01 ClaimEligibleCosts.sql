PRINT 'Fixing [ClaimEligibleCosts]'

UPDATE cec
SET 
	cec.ClaimCost = ROUND(cec.ClaimParticipants * cec.ClaimMaxParticipantCost, 2)
FROM dbo.ClaimEligibleCosts cec
WHERE cec.ClaimParticipants > 0
	AND cec.ClaimMaxParticipantCost != ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1)
