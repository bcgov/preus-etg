PRINT 'Migrate [TrainingPrograms]'

UPDATE tp
SET
	tp.TrainingProgramState = CASE WHEN btp.TrainingProgramState = 0 THEN 0 ELSE btp.TrainingProgramState - 1 END
FROM dbo.TrainingPrograms tp
	INNER JOIN #TrainingPrograms btp ON tp.Id = btp.Id

PRINT 'Populate [TrainingCosts]'

INSERT INTO dbo.TrainingCosts (
	GrantApplicationId
	, TrainingCostState
	, EstimatedParticipants
	, TotalEstimatedCost
	, TotalEstimatedReimbursement
	, AgreedParticipants
	, TotalAgreedMaxCost
	, AgreedCommitment
	, DateAdded
) 
select 
	ga.Id
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, GETUTCDATE()
FROM dbo.GrantApplications ga;

UPDATE dbo.TrainingCosts 
SET 
	TrainingCostState = btp.TrainingCostState
	, EstimatedParticipants = btp.EstimatedParticipants
	, TotalEstimatedCost = btp.TotalEstimatedCost
	, TotalEstimatedReimbursement = btp.TotalEstimatedReimbursement
	, AgreedParticipants = btp.AgreedParticipants
	, TotalAgreedMaxCost = btp.TotalAgreedMaxCost
	, AgreedCommitment = btp.AgreedCommitment
	, DateAdded = GETUTCDATE()
FROM #TrainingPrograms btp
WHERE btp.GrantApplicationId = dbo.TrainingCosts.GrantApplicationId;

DROP TABLE #TrainingPrograms