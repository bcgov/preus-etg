PRINT 'Fixing Financial Calculation bugs'

DECLARE @ClaimApproved INT = 25
DECLARE @Closed INT = 30

--------------------------------------------------------------------------------------------------------------------
-- Fix Closed and Approved Files/Claims
--------------------------------------------------------------------------------------------------------------------
SELECT DISTINCT
	ga.Id AS GrantApplicationId
INTO #ClosedOrApprovedFilesWithIssues
FROM dbo.EligibleCosts ec
INNER JOIN dbo.TrainingPrograms tp ON ec.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE ga.ApplicationStateInternal IN ( @ClaimApproved, @Closed )
	AND (
		ec.AgreedMaxParticipants = 0
		OR ec.AgreedMaxCost = 0
		OR ec.AgreedMaxParticipantCost = 0
		OR ec.AgreedMaxReimbursement = 0
	)

INSERT INTO #ClosedOrApprovedFilesWithIssues
SELECT DISTINCT
	ga.Id AS GrantApplicationId
FROM dbo.GrantApplications ga
INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
WHERE ga.ApplicationStateInternal IN ( @ClaimApproved, @Closed )
	AND (
		cec.ClaimParticipants = 0
		OR cec.ClaimCost = 0
		OR cec.ClaimMaxParticipantCost = 0
		OR cec.ClaimMaxParticipantReimbursementCost = 0
	)
	
INSERT INTO #ClosedOrApprovedFilesWithIssues
SELECT DISTINCT
	ga.Id AS GrantApplicationId
FROM dbo.GrantApplications ga
INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
WHERE ga.ApplicationStateInternal IN ( @ClaimApproved, @Closed )
	AND (
		cec.AssessedParticipants = 0
		OR cec.AssessedCost = 0
		OR cec.AssessedMaxParticipantCost = 0
		OR cec.AssessedMaxParticipantReimbursementCost = 0
	)

-- Fix

UPDATE ec
SET ec.AgreedMaxParticipants = 0
	, ec.AgreedMaxCost = 0
	, ec.AgreedMaxParticipantCost = 0
	, ec.AgreedMaxReimbursement = 0
	, ec.AgreedEmployerContribution = 0
FROM dbo.EligibleCosts ec
INNER JOIN dbo.TrainingPrograms tp ON ec.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN #ClosedOrApprovedFilesWithIssues i ON ga.Id = i.GrantApplicationId
WHERE ec.AgreedMaxParticipants = 0
	OR ec.AgreedMaxCost = 0
	OR ec.AgreedMaxParticipantCost = 0
	OR ec.AgreedMaxReimbursement = 0

UPDATE cec
SET cec.ClaimParticipants = 0
	, cec.ClaimCost = 0
	, cec.ClaimMaxParticipantCost = 0
	, cec.ClaimMaxParticipantReimbursementCost = 0
	, cec.ClaimParticipantEmployerContribution = 0
FROM dbo.GrantApplications ga
INNER JOIN #ClosedOrApprovedFilesWithIssues i ON ga.Id = i.GrantApplicationId
INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
WHERE cec.ClaimParticipants = 0
	OR cec.ClaimCost = 0
	OR cec.ClaimMaxParticipantCost = 0
	OR cec.ClaimMaxParticipantReimbursementCost = 0

UPDATE cec
SET cec.AssessedParticipants = 0
	, cec.AssessedCost = 0
	, cec.AssessedMaxParticipantCost = 0
	, cec.AssessedMaxParticipantReimbursementCost = 0
	, cec.AssessedParticipantEmployerContribution = 0
FROM dbo.GrantApplications ga
INNER JOIN #ClosedOrApprovedFilesWithIssues i ON ga.Id = i.GrantApplicationId
INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
WHERE cec.AssessedParticipants = 0
	OR cec.AssessedCost = 0
	OR cec.AssessedMaxParticipantCost = 0
	OR cec.AssessedMaxParticipantReimbursementCost = 0

--------------------------------------------------------------------------------------------------------------------
-- Save original data
--------------------------------------------------------------------------------------------------------------------

SELECT
	ga.GrantOpeningId
	, tp.GrantApplicationId

	, tp.Id AS TrainingProgramId
	, tp.EstimatedParticipants AS TP_EstimatedParticipants
	, tp.TotalEstimatedCost AS TP_TotalEstimatedCost
	, tp.TotalEstimatedReimbursement AS TP_TotalEstimatedReimbursement
	, tp.AgreedParticipants AS TP_AgreedParticipants
	, tp.TotalAgreedMaxCost AS TP_TotalAgreedMaxCost
	, tp.AgreedCommitment AS TP_AgreedCommitment

	, ec.Id AS EligibleCostId
	, ec.EstimatedParticipants AS EC_EstimatedParticipants
	, ec.EstimatedCost AS EC_EstimatedCost
	, ec.EstimatedParticipantCost AS EC_EstimatedParticipantCost
	, ec.EstimatedReimbursement AS EC_EstimatedReimbursement
	, ec.EstimatedEmployerContribution AS EC_EstimatedEmployerContribution
	, ec.AgreedMaxParticipants AS EC_AgreedMaxParticipants
	, ec.AgreedMaxCost AS EC_AgreedMaxCost
	, ec.AgreedMaxParticipantCost AS EC_AgreedMaxParticipantCost
	, ec.AgreedMaxReimbursement AS EC_AgreedMaxReimbursement
	, ec.AgreedEmployerContribution AS EC_AgreedEmployerContribution

	, c.Id AS ClaimId
	, c.ClaimVersion AS ClaimVersion
	, c.TotalClaimReimbursement AS C_TotalClaimReimbursement
	, c.TotalAssessedReimbursement AS C_TotalAssessedReimbursement

	, cec.Id As ClaimEligibleCostId
	, cec.ClaimParticipants AS CEC_ClaimParticipants
	, cec.ClaimCost AS CEC_ClaimCost
	, cec.ClaimMaxParticipantCost AS CEC_ClaimMaxParticipantCost
	, cec.ClaimMaxParticipantReimbursementCost AS CEC_ClaimMaxParticipantReimbursementCost
	, cec.ClaimParticipantEmployerContribution AS CEC_ClaimParticipantEmployerContribution
	, cec.AssessedParticipants AS CEC_AssessedParticipants
	, cec.AssessedCost AS CEC_AssessedCost
	, cec.AssessedMaxParticipantCost AS CEC_AssessedMaxParticipantCost
	, cec.AssessedMaxParticipantReimbursementCost AS CEC_AssessedMaxParticipantReimbursementCost
	, cec.AssessedParticipantEmployerContribution AS CEC_AssessedParticipantEmployerContribution

	, pc.Id AS ParticipantCostId
	, pc.ClaimParticipantCost AS PC_ClaimParticipantCost
	, pc.ClaimReimbursement AS PC_ClaimReimbursement
	, pc.ClaimEmployerContribution AS PC_ClaimEmployerContribution
	, pc.AssessedParticipantCost AS PC_AssessedParticipantCost
	, pc.AssessedReimbursement AS PC_AssessedReimbursement
	, pc.AssessedEmployerContribution AS PC_AssessedEmployerContribution
INTO #OriginalData
FROM dbo.Claims c 
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN #ClosedOrApprovedFilesWithIssues i ON ga.Id = i.GrantApplicationId
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id

DROP TABLE #ClosedOrApprovedFilesWithIssues
--------------------------------------------------------------------------------------------------------------------
-- Find Training Program and Eligible Costs issues
--------------------------------------------------------------------------------------------------------------------

-- Find all Training Program total issues.
SELECT DISTINCT tp.Id AS TrainingProgramId
INTO #TrainingProgramIssues
FROM dbo.TrainingPrograms tp
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN (
	SELECT DISTINCT
		ec.TrainingProgramId
		, MAX(ec.EstimatedParticipants) AS MaxEstimatedParticipants
		, SUM(ec.EstimatedCost) AS SumEstimatedCost
		, SUM(ec.EstimatedReimbursement) AS SumEstimatedReimbursement

		, MAX(ec.AgreedMaxParticipants) AS MaxAgreedMaxParticipants
		, SUM(ec.AgreedMaxCost) AS SumAgreedMaxCost
		, SUM(ec.AgreedMaxReimbursement) AS SumAgreedMaxReimbursement
	FROM dbo.EligibleCosts ec
	GROUP BY ec.TrainingProgramId
	) AS ec1 ON tp.Id = ec1.TrainingProgramId
WHERE 
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		tp.EstimatedParticipants < ec1.MaxEstimatedParticipants				-- There is an EligibleCost with greater participants
		OR tp.TotalEstimatedCost != ec1.SumEstimatedCost					-- The sum of the EligibleCosts is invalid
		OR tp.TotalEstimatedReimbursement != ec1.SumEstimatedReimbursement	-- The sum of the EligibleCosts is invalid

		OR tp.AgreedParticipants < ec1.MaxAgreedMaxParticipants				-- There is an EligibleCost with greater participants
		OR tp.TotalAgreedMaxCost != ec1.SumAgreedMaxCost					-- The sum of the EligibleCosts is invalid
		OR tp.AgreedCommitment != SumAgreedMaxReimbursement					-- The sum of the EligibleCosts is invalid
	)
	
-- Find all Eligible Costs calculation issues.
INSERT INTO #TrainingProgramIssues
SELECT DISTINCT ec.TrainingProgramId
FROM dbo.TrainingPrograms tp
INNER JOIN dbo.EligibleCosts ec ON tp.Id = ec.TrainingProgramId
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		ec.EstimatedParticipantCost != CASE WHEN (ec.EstimatedParticipants > 0) THEN ROUND(ec.EstimatedCost / ec.EstimatedParticipants, 2, 1) ELSE 0 END	-- The estimated participant cost is invalid (must be truncated)
		OR ec.EstimatedReimbursement != ROUND(ec.EstimatedCost * ga.ReimbursementRate, 2)																	-- The estimated reimbursement is invalid	
		OR ec.EstimatedEmployerContribution != ec.EstimatedCost - ROUND(ec.EstimatedCost * ga.ReimbursementRate, 2)											-- The estimated employer contribution is invalid

		OR ec.AgreedMaxParticipantCost != CASE WHEN (ec.AgreedMaxParticipants > 0) THEN ROUND(ec.AgreedMaxCost / ec.AgreedMaxParticipants, 2, 1) ELSE 0 END	-- The agreed participant cost is invalid (must be truncated)
		OR ec.AgreedMaxReimbursement != ROUND(ec.AgreedMaxCost * ga.ReimbursementRate, 2)																	-- The agreed reimbursement is invalid
		OR ec.AgreedEmployerContribution != ec.AgreedMaxCost - ROUND(ec.AgreedMaxCost * ga.ReimbursementRate, 2)											-- The agreed employer contribution is invalid
	)

SELECT DISTINCT otp.TrainingProgramId
INTO #TrainingProgramWithIssues
FROM #TrainingProgramIssues otp

--------------------------------------------------------------------------------------------------------------------
-- Save original data
--------------------------------------------------------------------------------------------------------------------

INSERT INTO #OriginalData
SELECT
	ga.GrantOpeningId
	, tp.GrantApplicationId

	, tp.Id AS TrainingProgramId
	, tp.EstimatedParticipants AS TP_EstimatedParticipants
	, tp.TotalEstimatedCost AS TP_TotalEstimatedCost
	, tp.TotalEstimatedReimbursement AS TP_TotalEstimatedReimbursement
	, tp.AgreedParticipants AS TP_AgreedParticipants
	, tp.TotalAgreedMaxCost AS TP_TotalAgreedMaxCost
	, tp.AgreedCommitment AS TP_AgreedCommitment

	, ec.Id AS EligibleCostId
	, ec.EstimatedParticipants AS EC_EstimatedParticipants
	, ec.EstimatedCost AS EC_EstimatedCost
	, ec.EstimatedParticipantCost AS EC_EstimatedParticipantCost
	, ec.EstimatedReimbursement AS EC_EstimatedReimbursement
	, ec.EstimatedEmployerContribution AS EC_EstimatedEmployerContribution
	, ec.AgreedMaxParticipants AS EC_AgreedMaxParticipants
	, ec.AgreedMaxCost AS EC_AgreedMaxCost
	, ec.AgreedMaxParticipantCost AS EC_AgreedMaxParticipantCost
	, ec.AgreedMaxReimbursement AS EC_AgreedMaxReimbursement
	, ec.AgreedEmployerContribution AS EC_AgreedEmployerContribution

	, c.Id AS ClaimId
	, c.ClaimVersion AS ClaimVersion
	, c.TotalClaimReimbursement AS C_TotalClaimReimbursement
	, c.TotalAssessedReimbursement AS C_TotalAssessedReimbursement

	, cec.Id As ClaimEligibleCostId
	, cec.ClaimParticipants AS CEC_ClaimParticipants
	, cec.ClaimCost AS CEC_ClaimCost
	, cec.ClaimMaxParticipantCost AS CEC_ClaimMaxParticipantCost
	, cec.ClaimMaxParticipantReimbursementCost AS CEC_ClaimMaxParticipantReimbursementCost
	, cec.ClaimParticipantEmployerContribution AS CEC_ClaimParticipantEmployerContribution
	, cec.AssessedParticipants AS CEC_AssessedParticipants
	, cec.AssessedCost AS CEC_AssessedCost
	, cec.AssessedMaxParticipantCost AS CEC_AssessedMaxParticipantCost
	, cec.AssessedMaxParticipantReimbursementCost AS CEC_AssessedMaxParticipantReimbursementCost
	, cec.AssessedParticipantEmployerContribution AS CEC_AssessedParticipantEmployerContribution

	, pc.Id AS ParticipantCostId
	, pc.ClaimParticipantCost AS PC_ClaimParticipantCost
	, pc.ClaimReimbursement AS PC_ClaimReimbursement
	, pc.ClaimEmployerContribution AS PC_ClaimEmployerContribution
	, pc.AssessedParticipantCost AS PC_AssessedParticipantCost
	, pc.AssessedReimbursement AS PC_AssessedReimbursement
	, pc.AssessedEmployerContribution AS PC_AssessedEmployerContribution
FROM dbo.Claims c 
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN #TrainingProgramWithIssues tpwi ON tp.Id = tpwi.TrainingProgramId
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id

--------------------------------------------------------------------------------------------------------------------
-- Fix Issues
--------------------------------------------------------------------------------------------------------------------

-- Eligible Costs
-- Ensure they don't exceed Agreement
UPDATE ec
SET ec.EstimatedParticipantCost = CASE WHEN (ec.EstimatedParticipants > 0) THEN ROUND(ec.EstimatedCost / ec.EstimatedParticipants, 2, 1) ELSE 0 END			-- Truncate participant costs
	, ec.AgreedMaxParticipantCost = CASE WHEN (ec.AgreedMaxParticipants > 0) THEN ROUND(ec.AgreedMaxCost / ec.AgreedMaxParticipants, 2, 1) ELSE 0 END		-- Truncate participant costs
FROM dbo.EligibleCosts ec
INNER JOIN #TrainingProgramWithIssues tpwi ON ec.TrainingProgramId = tpwi.TrainingProgramId

-- The Agreement set it to 0
UPDATE ec
SET ec.AgreedMaxParticipants = 0
	, ec.AgreedMaxCost = 0
	, ec.AgreedMaxParticipantCost = 0
	, ec.AgreedMaxReimbursement = 0
	, ec.AgreedEmployerContribution = 0
FROM dbo.EligibleCosts ec
INNER JOIN #TrainingProgramWithIssues tpwi ON ec.TrainingProgramId = tpwi.TrainingProgramId
WHERE ec.AgreedMaxParticipants = 0
	OR ec.AgreedMaxCost = 0
	OR ec.AgreedMaxReimbursement = 0

-- Training Program
UPDATE tp
SET tp.TotalAgreedMaxCost = ec.SumAgreedMaxCost
	, tp.AgreedCommitment = ec.SumAgreedMaxReimbursement
FROM dbo.TrainingPrograms tp
INNER JOIN (
	SELECT ec.TrainingProgramId
		, SUM(ec.AgreedMaxCost) AS SumAgreedMaxCost
		, SUM(ec.AgreedMaxReimbursement) AS SumAgreedMaxReimbursement
	FROM dbo.EligibleCosts ec
	INNER JOIN #TrainingProgramWithIssues tpwi ON ec.TrainingProgramId = tpwi.TrainingProgramId
	GROUP BY ec.TrainingProgramId
	) AS ec ON tp.Id = ec.TrainingProgramId

--------------------------------------------------------------------------------------------------------------------

--------------------------------------------------------------------------------------------------------------------

-- Find all Claim Eligible Costs calculation issues
-- Find all Claim Eligible Costs that exceed Agreement
SELECT DISTINCT
	c.TrainingProgramId
	, cec.ClaimId
	, cec.ClaimVersion
INTO #ClaimIssues
FROM dbo.ClaimEligibleCosts cec
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
WHERE 
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		cec.ClaimParticipants > ISNULL(ec.AgreedMaxParticipants, cec.AssessedParticipants)																																									-- The claimed participants is greater than the agreed
		OR cec.ClaimCost > ISNULL(ec.AgreedMaxCost, cec.AssessedCost)																																														-- The claimed cost is greater than the agreed
		OR cec.ClaimMaxParticipantCost != CASE WHEN (cec.ClaimParticipants > 0) THEN ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) ELSE 0 END																											-- The claimed participant cost is invalid (must be truncated)
		OR cec.ClaimMaxParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)																																				-- The claimed max participant cost is greater than the agreement
		OR cec.ClaimMaxParticipantReimbursementCost != CASE WHEN (cec.ClaimParticipants > 0) THEN ROUND(ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) * ga.ReimbursementRate, 2, 1) ELSE 0 END															-- The claimed participant reimbursement is invalid (must be truncated)
		OR cec.ClaimParticipantEmployerContribution != CASE WHEN (cec.ClaimParticipants > 0) THEN ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) - ROUND(ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) * ga.ReimbursementRate, 2, 1) ELSE 0 END	-- The claimed participant employer contribution is invalid (must be truncated)

		OR cec.AssessedParticipants > ISNULL(ec.AgreedMaxParticipants, cec.AssessedParticipants)																																											-- The assessed participants is greater than the agreed
		OR cec.AssessedCost > ISNULL(ec.AgreedMaxCost, cec.AssessedCost)																																																	-- The assessed cost is greater than the agreed
		OR cec.AssessedMaxParticipantCost != CASE WHEN (cec.AssessedParticipants > 0) THEN ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) ELSE 0 END																												-- The assessed participant cost is invalid (must be truncated)
		OR cec.AssessedMaxParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)
		OR cec.AssessedMaxParticipantReimbursementCost != CASE WHEN (cec.AssessedParticipants > 0) THEN ROUND(ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) * ga.ReimbursementRate, 2, 1) ELSE 0 END																-- The assessed participant reimbursement is invalid (must be truncated)
		OR cec.AssessedParticipantEmployerContribution != CASE WHEN (cec.AssessedParticipants > 0) THEN ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) - ROUND(ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) * ga.ReimbursementRate, 2, 1) ELSE 0 END	-- The assessed participant employer contribution is invalid (must be truncated)
	)

-- Find all Claim total issues.
INSERT INTO #ClaimIssues
SELECT DISTINCT
	c.TrainingProgramId 
	, c.Id AS ClaimId
	, c.ClaimVersion
FROM dbo.Claims c
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN (
	SELECT cec.ClaimId
		, cec.ClaimVersion
		, SUM(pc.ClaimReimbursement) AS SumClaimReimbursement
		, SUM(pc.AssessedReimbursement) AS SumAssessedReimbursement
	FROM dbo.ParticipantCosts pc
	INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
	GROUP BY 
		cec.ClaimId
		, cec.ClaimVersion
	) pc ON c.Id = pc.ClaimId AND c.ClaimVersion = pc.ClaimVersion
WHERE
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		c.TotalClaimReimbursement != pc.SumClaimReimbursement			-- The sum of the participant claimed reimbursements is invalid
		OR c.TotalAssessedReimbursement != pc.SumAssessedReimbursement	-- The sum of the participant assessed reimbursements is invalid
	)

-- Find all Participant Costs calculation issues.
-- Find all Participant Costs that exceed Agreement.
INSERT INTO #ClaimIssues
SELECT DISTINCT
	c.TrainingProgramId
	, cec.ClaimId
	, cec.ClaimVersion
FROM dbo.ClaimEligibleCosts cec
INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
WHERE
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		pc.ClaimParticipantCost > cec.AssessedMaxParticipantCost	
		OR pc.ClaimParticipantCost > cec.ClaimMaxParticipantCost																	-- The claimed participant costs are greater than the assessed
		OR pc.ClaimParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)							-- The claimed participant costs are greater than the agreed
		OR pc.ClaimReimbursement != ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1)										-- The claimed participant reimbursement is invalid (must be truncated)
		OR pc.ClaimEmployerContribution != pc.ClaimParticipantCost - ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1)	-- The claimed participant empoyer contribution is invalid (must be truncated)
	
		OR pc.AssessedParticipantCost > cec.AssessedMaxParticipantCost
		OR pc.AssessedParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)									-- The assessed participant costs are greater than the agreed
		OR pc.AssessedReimbursement != ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1)										-- The assessed participant reimbursement is invalid (must be truncated)
		OR pc.AssessedEmployerContribution != pc.AssessedParticipantCost - ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1)	-- The assessed participant empoyer contribution is invalid (must be truncated)
	)

-- Find all Claim Eligible Costs that have too may Participant Costs
INSERT INTO #ClaimIssues
SELECT DISTINCT
	c.TrainingProgramId
	, cec.ClaimId
	, cec.ClaimVersion
FROM dbo.ClaimEligibleCosts cec
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN (
	SELECT cec.ClaimId
		, cec.ClaimVersion
		, COUNT(*) AS NumberOfParticipantsWithCosts
	FROM dbo.ParticipantCosts pc
	INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
	WHERE pc.ClaimParticipantCost > 0
	GROUP BY 
		cec.ClaimId
		, cec.ClaimVersion
	) pc1 ON cec.ClaimId = pc1.ClaimId AND cec.ClaimVersion = pc1.ClaimVersion
INNER JOIN (
	SELECT cec.ClaimId
		, cec.ClaimVersion
		, COUNT(*) AS NumberOfParticipantsWithCosts
	FROM dbo.ParticipantCosts pc
	INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
	WHERE pc.AssessedParticipantCost > 0
	GROUP BY 
		cec.ClaimId
		, cec.ClaimVersion
	) pc2 ON cec.ClaimId = pc2.ClaimId AND cec.ClaimVersion = pc2.ClaimVersion
WHERE
	ga.ApplicationStateInternal NOT IN ( @Closed, @ClaimApproved )
	AND (
		cec.ClaimParticipants < pc1.NumberOfParticipantsWithCosts												-- The number of claimed participants with costs is greater than the agreed amount
		OR ISNULL(ec.AgreedMaxParticipants, cec.ClaimParticipants) < pc1.NumberOfParticipantsWithCosts			-- The number of claimed participants with costs is greater than the agreed amount
		OR cec.AssessedParticipants < pc2.NumberOfParticipantsWithCosts											-- The number of assessed participants with costs is greater than the agreed amount
		OR ISNULL(ec.AgreedMaxParticipants, cec.AssessedParticipants) < pc2.NumberOfParticipantsWithCosts		-- The number of assessed participants with costs is greater than the agreed amount
	)


--------------------------------------------------------------------------------------------------------------------
-- Save original data
--------------------------------------------------------------------------------------------------------------------

SELECT
	ga.GrantOpeningId
	, tp.GrantApplicationId

	, c.Id AS ClaimId
	, c.ClaimVersion AS ClaimVersion
	, c.TotalClaimReimbursement AS C_TotalClaimReimbursement
	, c.TotalAssessedReimbursement AS C_TotalAssessedReimbursement

	, tp.Id AS TrainingProgramId
	, tp.EstimatedParticipants AS TP_EstimatedParticipants
	, tp.TotalEstimatedCost AS TP_TotalEstimatedCost
	, tp.TotalEstimatedReimbursement AS TP_TotalEstimatedReimbursement
	, tp.AgreedParticipants AS TP_AgreedParticipants
	, tp.TotalAgreedMaxCost AS TP_TotalAgreedMaxCost
	, tp.AgreedCommitment AS TP_AgreedCommitment

	, ec.Id AS EligibleCostId
	, ec.EstimatedParticipants AS EC_EstimatedParticipants
	, ec.EstimatedCost AS EC_EstimatedCost
	, ec.EstimatedParticipantCost AS EC_EstimatedParticipantCost
	, ec.EstimatedReimbursement AS EC_EstimatedReimbursement
	, ec.EstimatedEmployerContribution AS EC_EstimatedEmployerContribution
	, ec.AgreedMaxParticipants AS EC_AgreedMaxParticipants
	, ec.AgreedMaxCost AS EC_AgreedMaxCost
	, ec.AgreedMaxParticipantCost AS EC_AgreedMaxParticipantCost
	, ec.AgreedMaxReimbursement AS EC_AgreedMaxReimbursement
	, ec.AgreedEmployerContribution AS EC_AgreedEmployerContribution

	, cec.Id As ClaimEligibleCostId
	, cec.ClaimParticipants AS CEC_ClaimParticipants
	, cec.ClaimCost AS CEC_ClaimCost
	, cec.ClaimMaxParticipantCost AS CEC_ClaimMaxParticipantCost
	, cec.ClaimMaxParticipantReimbursementCost AS CEC_ClaimMaxParticipantReimbursementCost
	, cec.ClaimParticipantEmployerContribution AS CEC_ClaimParticipantEmployerContribution
	, cec.AssessedParticipants AS CEC_AssessedParticipants
	, cec.AssessedCost AS CEC_AssessedCost
	, cec.AssessedMaxParticipantCost AS CEC_AssessedMaxParticipantCost
	, cec.AssessedMaxParticipantReimbursementCost AS CEC_AssessedMaxParticipantReimbursementCost
	, cec.AssessedParticipantEmployerContribution AS CEC_AssessedParticipantEmployerContribution

	, pc.Id AS ParticipantCostId
	, pc.ClaimParticipantCost AS PC_ClaimParticipantCost
	, pc.ClaimReimbursement AS PC_ClaimReimbursement
	, pc.ClaimEmployerContribution AS PC_ClaimEmployerContribution
	, pc.AssessedParticipantCost AS PC_AssessedParticipantCost
	, pc.AssessedReimbursement AS PC_AssessedReimbursement
	, pc.AssessedEmployerContribution AS PC_AssessedEmployerContribution
INTO #OriginalClaims
FROM dbo.Claims c 
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
WHERE CAST(c.Id AS VARCHAR) + '_' + CAST(c.ClaimVersion AS VARCHAR) IN (SELECT DISTINCT CAST(ClaimId AS VARCHAR) + '_' + CAST(ClaimVersion AS VARCHAR) FROM #ClaimIssues)

--------------------------------------------------------------------------------------------------------------------
-- Fix Issues
--------------------------------------------------------------------------------------------------------------------

-- Claim Eligible Costs
-- Recalculate claim value based on Agreement
UPDATE cec
SET 
	cec.ClaimParticipants = CASE WHEN (cec.ClaimParticipants > ISNULL(ec.AgreedMaxParticipants, cec.AssessedParticipants)) THEN ISNULL(ec.AgreedMaxParticipants, cec.AssessedParticipants) ELSE cec.ClaimParticipants END
	, cec.ClaimCost = CASE WHEN (cec.ClaimCost > ISNULL(ec.AgreedMaxCost, cec.AssessedCost)) THEN ISNULL(ec.AgreedMaxCost, cec.AssessedCost) ELSE cec.ClaimCost END
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id

-- Recalculate assessed value based on Claim
UPDATE cec
SET 
	cec.AssessedParticipants = CASE WHEN (cec.AssessedParticipants > ec.AgreedMaxParticipants) THEN ec.AgreedMaxParticipants ELSE cec.AssessedParticipants END
	, cec.AssessedCost = CASE WHEN (cec.AssessedCost > ec.AgreedMaxCost) THEN ec.AgreedMaxCost ELSE cec.AssessedCost END
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id	-- Inner join since we only want to update costs with agreement

-- Set all to 0 because claim is 0
UPDATE cec
SET 
	cec.ClaimParticipants = 0
	, cec.ClaimCost = 0
	, cec.ClaimMaxParticipantCost = 0
	, cec.ClaimMaxParticipantReimbursementCost = 0
	, cec.ClaimParticipantEmployerContribution = 0
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE cec.ClaimParticipants = 0
	OR cec.ClaimCost = 0
	OR cec.ClaimMaxParticipantCost = 0
	OR cec.ClaimMaxParticipantReimbursementCost = 0

-- Set all to 0 because assessed is 0
UPDATE cec
SET 
	cec.AssessedParticipants = 0
	, cec.AssessedCost = 0
	, cec.AssessedMaxParticipantCost = 0
	, cec.AssessedMaxParticipantReimbursementCost = 0
	, cec.AssessedParticipantEmployerContribution = 0
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE cec.AssessedParticipants = 0
	OR cec.AssessedCost = 0
	OR cec.AssessedMaxParticipantCost = 0
	OR cec.AssessedMaxParticipantReimbursementCost = 0
	
-- Recalculate the participant cost to be truncated
UPDATE cec
SET 
	cec.ClaimMaxParticipantCost = CASE 
		WHEN (cec.ClaimParticipants > 0 AND ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) <= ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost) ) THEN ROUND(cec.ClaimCost / cec.ClaimParticipants, 2, 1) 
		WHEN (cec.ClaimParticipants > 0) THEN ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)
		ELSE 0 END

	, cec.AssessedMaxParticipantCost = CASE 
		WHEN (cec.AssessedParticipants > 0 AND ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) <= ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost) ) THEN ROUND(cec.AssessedCost / cec.AssessedParticipants, 2, 1) 
		WHEN (cec.AssessedParticipants > 0) THEN ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)
		ELSE 0 END
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id

-- Recalculate the reimbursement to be truncated
UPDATE cec
SET 
	cec.ClaimMaxParticipantReimbursementCost = ROUND(cec.ClaimMaxParticipantCost * ga.ReimbursementRate, 2, 1)
	, cec.ClaimParticipantEmployerContribution = cec.ClaimMaxParticipantCost - ROUND(cec.ClaimMaxParticipantCost * ga.ReimbursementRate, 2, 1)
	
	, cec.AssessedParticipantEmployerContribution = cec.AssessedMaxParticipantCost - cec.AssessedMaxParticipantReimbursementCost
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id

-- Recalculate the reimbursement to be truncated
UPDATE cec
SET 
	cec.AssessedMaxParticipantReimbursementCost = ROUND(cec.AssessedMaxParticipantCost * ga.ReimbursementRate, 2, 1)
	, cec.AssessedParticipantEmployerContribution = cec.AssessedMaxParticipantCost - ROUND(cec.AssessedMaxParticipantCost * ga.ReimbursementRate, 2, 1)
FROM dbo.ClaimEligibleCosts cec
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
INNER JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
WHERE cec.AssessedMaxParticipantReimbursementCost = ROUND(cec.AssessedMaxParticipantCost * ga.ReimbursementRate, 2) -- It is rounded but should be truncated

-- Participant Costs
-- Set to 0 because the Agreement is 0
UPDATE pc
SET pc.ClaimParticipantCost = 0
	, pc.ClaimReimbursement = 0
	, pc.ClaimEmployerContribution = 0

	, pc.AssessedParticipantCost = 0
	, pc.AssessedReimbursement = 0
	, pc.AssessedEmployerContribution = 0
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id
WHERE ISNULL(ec.AgreedMaxReimbursement, cec.AssessedMaxParticipantReimbursementCost) = 0

-- Too many Participant Costs so we need to 0 them all
UPDATE pc
SET pc.ClaimParticipantCost = 0
	, pc.ClaimReimbursement = 0
	, pc.ClaimEmployerContribution = 0

	, pc.AssessedParticipantCost = 0
	, pc.AssessedReimbursement = 0
	, pc.AssessedEmployerContribution = 0
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN (
	SELECT cec.Id AS ClaimEligibleCostId
		, COUNT(pc.Id) AS NumberOfParticipantsWithCosts
	FROM dbo.ClaimEligibleCosts cec
	INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
	WHERE pc.ClaimParticipantCost > 0
	GROUP BY cec.Id
	) AS cec1 ON pc.ClaimEligibleCostId = cec1.ClaimEligibleCostId AND cec.ClaimParticipants < cec1.NumberOfParticipantsWithCosts

-- Too many Participant Costs so we need to 0 them all
UPDATE pc
SET pc.AssessedParticipantCost = 0
	, pc.AssessedReimbursement = 0
	, pc.AssessedEmployerContribution = 0
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN (
	SELECT cec.Id AS ClaimEligibleCostId
		, COUNT(pc.Id) AS NumberOfParticipantsWithCosts
	FROM dbo.ClaimEligibleCosts cec
	INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
	WHERE pc.AssessedParticipantCost > 0
	GROUP BY cec.Id
	) AS cec1 ON pc.ClaimEligibleCostId = cec1.ClaimEligibleCostId AND cec.AssessedParticipants < cec1.NumberOfParticipantsWithCosts

-- Lower the participant costs based on the Agreement
UPDATE pc
SET pc.ClaimParticipantCost = CASE 
								WHEN (pc.ClaimParticipantCost > cec.ClaimMaxParticipantCost) THEN cec.ClaimMaxParticipantCost
								WHEN (pc.ClaimParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)) THEN ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost) 
								ELSE pc.ClaimParticipantCost END

	, pc.AssessedParticipantCost = CASE 
								WHEN (pc.AssessedParticipantCost > cec.AssessedMaxParticipantCost) THEN cec.AssessedMaxParticipantCost
								WHEN (pc.AssessedParticipantCost > ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost)) THEN ISNULL(ec.AgreedMaxParticipantCost, cec.AssessedMaxParticipantCost) 
								ELSE pc.AssessedParticipantCost END
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
LEFT JOIN dbo.EligibleCosts ec ON cec.EligibleCostId = ec.Id

-- Recalculate reimbursement to use truncation
UPDATE pc
SET pc.ClaimReimbursement = ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1)
	, pc.ClaimEmployerContribution = pc.ClaimParticipantCost - ROUND(pc.ClaimParticipantCost * ga.ReimbursementRate, 2, 1)

	, pc.AssessedEmployerContribution = pc.AssessedParticipantCost - pc.AssessedReimbursement
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id

-- Recalculate reimbursement to use truncation
UPDATE pc
SET pc.AssessedReimbursement = ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1)
	, pc.AssessedEmployerContribution = pc.AssessedParticipantCost - ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2, 1)
FROM dbo.ParticipantCosts pc
INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
INNER JOIN #OriginalClaims i ON cec.ClaimId = i.ClaimId AND cec.ClaimVersion = i.ClaimVersion
INNER JOIN dbo.Claims c ON cec.ClaimId = c.Id AND cec.ClaimVersion = c.ClaimVersion
INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE pc.AssessedReimbursement = ROUND(pc.AssessedParticipantCost * ga.ReimbursementRate, 2) -- It is rounded but should be truncated

-- Recalculate totals for Claim
UPDATE c
SET c.TotalClaimReimbursement = pc.SumClaimReimbursement
	, c.TotalAssessedReimbursement = pc.SumAssessedReimbursement
FROM dbo.Claims c
INNER JOIN #OriginalClaims i ON c.Id = i.ClaimId AND c.ClaimVersion = i.ClaimVersion
INNER JOIN (
	SELECT cec.ClaimId
		, cec.ClaimVersion
		, SUM(pc.ClaimReimbursement) AS SumClaimReimbursement
		, SUM(pc.AssessedReimbursement) AS SumAssessedReimbursement
	FROM dbo.ClaimEligibleCosts cec
	INNER JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
	GROUP BY cec.ClaimId
		, cec.ClaimVersion
	) pc ON c.Id = pc.ClaimId AND c.ClaimVersion = pc.ClaimVersion

--------------------------------------------------------------------------------------------------------------------
-- View results
--------------------------------------------------------------------------------------------------------------------
INSERT INTO #OriginalData
SELECT *
FROM #OriginalClaims

SELECT *
INTO dbo._OriginalData
FROM #OriginalData
ORDER BY 
	GrantApplicationId
	, TrainingProgramId
	, EligibleCostId
	, ClaimId
	, ClaimVersion
	, ClaimEligibleCostId
	, ParticipantCostId

--------------------------------------------------------------------------------------------------------------------
-- Clean up
--------------------------------------------------------------------------------------------------------------------
DROP TABLE #OriginalData
DROP TABLE #TrainingProgramIssues
DROP TABLE #TrainingProgramWithIssues
DROP TABLE #ClaimIssues
DROP TABLE #OriginalClaims