PRINT 'Fixing Participants without costs in submitted claims'

SELECT DISTINCT pc.ParticipantEnrollmentId
INTO #ParticipantsWithoutCosts
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
WHERE c.ClaimState >= 21
GROUP BY c.Id
	, c.ClaimVersion
	, c.ClaimNumber
	, c.ClaimState
	, pc.ParticipantEnrollmentId
HAVING SUM(pc.ClaimParticipantCost) = 0

DELETE FROM dbo.ParticipantCosts
WHERE ParticipantEnrollmentId IN (
	SELECT ParticipantEnrollmentId
	FROM #ParticipantsWithoutCosts
	)

SELECT ParticipantFormId
INTO #ParticipantFormsWithoutCosts
FROM dbo.ParticipantEnrollments
WHERE Id IN (
	SELECT ParticipantEnrollmentId
	FROM #ParticipantsWithoutCosts
	)
	
DELETE FROM dbo.ParticipantEnrollments
WHERE Id IN (
	SELECT ParticipantEnrollmentId
	FROM #ParticipantsWithoutCosts
	)

DELETE FROM dbo.ParticipantForms
WHERE Id IN (
	SELECT ParticipantFormId
	FROM #ParticipantFormsWithoutCosts
	)

DROP TABLE #ParticipantsWithoutCosts
DROP TABLE #ParticipantFormsWithoutCosts