PRINT 'Backup [TrainingPrograms]'

SELECT DISTINCT 
	tp.Id
	, tp.GrantApplicationId
	, tp.EstimatedParticipants
	, tp.TotalEstimatedCost
	, tp.TotalEstimatedReimbursement
	, tp.AgreedParticipants
	, tp.AgreedCommitment
	, tp.TotalAgreedMaxCost
	, tp.TrainingProgramState
	, tp.TrainingCostState
INTO #TrainingPrograms
FROM dbo.TrainingPrograms tp