-- Find all missing EligibleCosts and copy them into the Claim.
INSERT INTO dbo.ClaimEligibleCosts (
	ClaimId
	,ClaimVersion
	,EligibleExpenseTypeId
	,EligibleCostId
	,SourceId
	,ClaimCost
	,ClaimParticipants
	,ClaimMaxParticipantCost
	,ClaimMaxParticipantReimbursementCost
	,ClaimParticipantEmployerContribution
	,AssessedCost
	,AssessedParticipants
	,AssessedMaxParticipantCost
	,AssessedMaxParticipantReimbursementCost
	,AssessedParticipantEmployerContribution
	,AddedByAssessor
)
SELECT DISTINCT c.Id
	,c.ClaimVersion
	,ec.EligibleExpenseTypeId
	,ec.Id
	,NULL
	,ec.AgreedMaxCost
	,ec.AgreedMaxParticipants
	,ec.AgreedMaxParticipantCost
	,ROUND(ec.AgreedMaxParticipantCost * ga.ReimbursementRate, 2, 1)
	,ec.AgreedMaxParticipantCost - ROUND(ec.AgreedMaxParticipantCost * ga.ReimbursementRate, 2, 1)
	,0
	,ec.AgreedMaxParticipants
	,0
	,0
	,0
	,0
FROM dbo.EligibleCosts ec
	INNER JOIN dbo.TrainingPrograms tp ON ec.TrainingProgramId = tp.Id
	INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId AND c.ClaimState IN (0, 1, 21)
	INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
WHERE ec.Id NOT IN (
	SELECT DISTINCT cec.EligibleCostId
	FROM dbo.ClaimEligibleCosts cec
	WHERE cec.EligibleCostId IS NOT NULL
		AND cec.ClaimId = c.Id
		AND cec.ClaimVersion = c.ClaimVersion
	)

DECLARE @ParticipantCosts TABLE (
	ClaimEligibleCostId INT NOT NULL
	,ParticipantEnrollmentId INT NOT NULL
	,ClaimParticipantCost DECIMAL NOT NULL DEFAULT 0
	,ClaimReimbursement DECIMAL NOT NULL DEFAULT 0
	,ClaimEmployerContribution DECIMAL NOT NULL DEFAULT 0
	,AssessedParticipantCost DECIMAL NOT NULL DEFAULT 0
	,AssessedReimbursement DECIMAL NOT NULL DEFAULT 0
	,AssessedEmployerContribution DECIMAL NOT NULL DEFAULT 0
	,DateAdded DATETIME NOT NULL DEFAULT GETUTCDATE()
)

-- Find all missing ParticipantCosts and add them, but set them to 0.
INSERT INTO @ParticipantCosts (
	ClaimEligibleCostId
	,ParticipantEnrollmentId
	,ClaimParticipantCost
	,ClaimReimbursement
	,ClaimEmployerContribution
	,AssessedParticipantCost
	,AssessedReimbursement
	,AssessedEmployerContribution
	,DateAdded
)
SELECT DISTINCT 
	cec.Id
	,pe.Id AS ParticipantEnrollmentId
	,0 --cec.ClaimMaxParticipantCost
	,0 --cec.ClaimMaxParticipantReimbursementCost
	,0 --cec.ClaimParticipantEmployerContribution
	,0 --cec.AssessedMaxParticipantCost
	,0 --cec.AssessedMaxParticipantReimbursementCost
	,0 --cec.AssessedParticipantEmployerContribution
	,GETUTCDATE() AS DateAdded
FROM dbo.ParticipantEnrollments pe
	INNER JOIN dbo.EmployerEnrollments ee ON pe.EmployerEnrollmentId = ee.Id
	INNER JOIN dbo.TrainingPrograms tp ON ee.TrainingProgramId = tp.Id
	INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId AND c.ClaimState IN (0, 1, 21)
	INNER JOIN dbo.ClaimEligibleCosts cec ON c.Id = cec.ClaimId AND c.ClaimVersion = cec.ClaimVersion
	LEFT JOIN dbo.ParticipantCosts pc ON cec.Id = pc.ClaimEligibleCostId
WHERE pc.Id IS NULL

INSERT INTO dbo.ParticipantCosts (
	ClaimEligibleCostId
	,ParticipantEnrollmentId
	,ClaimParticipantCost
	,ClaimReimbursement
	,ClaimEmployerContribution
	,AssessedParticipantCost
	,AssessedReimbursement
	,AssessedEmployerContribution
	,DateAdded
)
SELECT 
	ClaimEligibleCostId
	,ParticipantEnrollmentId
	,ClaimParticipantCost
	,ClaimReimbursement
	,ClaimEmployerContribution
	,AssessedParticipantCost
	,AssessedReimbursement
	,AssessedEmployerContribution
	,DateAdded
FROM @ParticipantCosts

-- Need to update the new ParticipantCosts with values to ensure we don't get a validation error.
DECLARE @ParticipantCostCursor CURSOR
DECLARE @ParticipantCostId INT
BEGIN
	-- Find all ClaimEligibleCosts that have not assigned ParticipantCosts.
	SET @ParticipantCostCursor = CURSOR FOR	(
		SELECT DISTINCT pc.Id
		FROM @ParticipantCosts ipc
			INNER JOIN dbo.ParticipantCosts pc ON ipc.ParticipantEnrollmentId = pc.ParticipantEnrollmentId AND ipc.ClaimEligibleCostId = pc.ClaimEligibleCostId
	)
	
	OPEN @ParticipantCostCursor
	FETCH NEXT FROM @ParticipantCostCursor
		INTO @ParticipantCostId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		Update pc
		SET pc.ClaimParticipantCost = cec.ClaimMaxParticipantCost
			,pc.ClaimReimbursement = cec.ClaimMaxParticipantReimbursementCost
			,pc.ClaimEmployerContribution = cec.ClaimParticipantEmployerContribution
			,pc.DateUpdated = GETUTCDATE()
		FROM dbo.ParticipantCosts pc
			INNER JOIN dbo.ClaimEligibleCosts cec ON pc.ClaimEligibleCostId = cec.Id
		WHERE pc.Id = @ParticipantCostId
			AND cec.ClaimParticipants > (SELECT COUNT(*) FROM dbo.ParticipantCosts WHERE ClaimEligibleCostId = cec.Id AND ClaimParticipantCost > 0)

		FETCH NEXT FROM @ParticipantCostCursor
			INTO @ParticipantCostId
	END

	CLOSE @ParticipantCostCursor
	DEALLOCATE @ParticipantCostCursor
END
