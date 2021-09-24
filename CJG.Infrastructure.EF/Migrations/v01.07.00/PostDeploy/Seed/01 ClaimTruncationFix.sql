PRINT 'Fixing Truncation issue - Updating [ClaimEligibleCosts], [ParticipantCosts], [Claims]'
-- There is an issue with SQL truncation, for some reason if the percentage is 100% it will not return 100%, but will round down by one penny.

SELECT DISTINCT ga.Id
	, ga.FileNumber
	, cec.ClaimId
	, cec.ClaimVersion
INTO #InvalidClaimEligibleCosts
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE cec.ClaimParticipantEmployerContribution = 0.01
	OR cec.AssessedParticipantEmployerContribution = 0.01

SELECT DISTINCT ga.Id
	, ga.FileNumber
	, cec.ClaimId
	, cec.ClaimVersion
INTO #InvalidParticipantCosts
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE pc.ClaimEmployerContribution = 0.01
	OR pc.AssessedEmployerContribution = 0.01

UPDATE pc
SET pc.ClaimEmployerContribution = CASE WHEN (ga.ReimbursementRate = 1) THEN 0 ELSE pc.ClaimParticipantCost - ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1) END
	, pc.ClaimReimbursement = CASE WHEN (ga.ReimbursementRate = 1) THEN pc.ClaimParticipantCost ELSE ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1) END
	, pc.AssessedEmployerContribution = CASE WHEN (ga.ReimbursementRate = 1) THEN 0 ELSE pc.AssessedParticipantCost - ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1) END
	, pc.AssessedReimbursement = CASE WHEN (ga.ReimbursementRate = 1) THEN pc.AssessedParticipantCost ELSE ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1) END
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN #InvalidParticipantCosts i ON c.Id = i.ClaimId AND c.ClaimVersion = i.ClaimVersion

UPDATE cec
SET cec.ClaimParticipantEmployerContribution = CASE WHEN (ga.ReimbursementRate = 1) THEN 0 ELSE cec.ClaimMaxParticipantCost - ROUND(cec.ClaimMaxParticipantCost * ga.ReimbursementRate, 2, 1) END
	, cec.ClaimMaxParticipantReimbursementCost = CASE WHEN (ga.ReimbursementRate = 1) THEN cec.ClaimMaxParticipantCost ELSE ROUND(cec.ClaimMaxParticipantCost * ga.ReimbursementRate, 2, 1) END
	, cec.AssessedParticipantEmployerContribution = CASE WHEN (ga.ReimbursementRate = 1) THEN 0 ELSE cec.AssessedMaxParticipantCost - ROUND(cec.AssessedMaxParticipantCost * ga.ReimbursementRate, 2, 1) END
	, cec.AssessedMaxParticipantReimbursementCost = CASE WHEN (ga.ReimbursementRate = 1) THEN cec.AssessedMaxParticipantCost ELSE ROUND(cec.AssessedMaxParticipantCost * ga.ReimbursementRate, 2, 1) END
FROM dbo.ClaimEligibleCosts cec
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN #InvalidClaimEligibleCosts i ON c.Id = i.ClaimId AND c.ClaimVersion = i.ClaimVersion

UPDATE c
SET c.TotalClaimReimbursement = SumClaimReimbursement
	, c.TotalAssessedReimbursement = SumAssessedReimbursement
FROM dbo.Claims c
INNER JOIN (
	SELECT DISTINCT
		cec.ClaimId
		, cec.ClaimVersion 
		, SUM(pc.ClaimReimbursement) AS SumClaimReimbursement
		, SUM(pc.AssessedReimbursement) AS SumAssessedReimbursement
	FROM dbo.ClaimEligibleCosts cec
	INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
	INNER JOIN (
		SELECT ClaimId
			, ClaimVersion
		FROM #InvalidParticipantCosts
		UNION
		SELECT ClaimId
			, ClaimVersion
		FROM #InvalidClaimEligibleCosts
		) i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
	GROUP BY cec.ClaimId	
		, cec.ClaimVersion
	) cs ON c.Id = cs.ClaimId AND c.ClaimVersion = cs.ClaimVersion

DROP TABLE #InvalidClaimEligibleCosts
DROP TABLE #InvalidParticipantCosts