PRINT N'Update EligibleCosts'

UPDATE ec
SET 
	ec.[EstimatedReimbursement] = ROUND(ISNULL(ec.[EstimatedCost] * ga.[ReimbursementRate],0), 2),
	ec.[EstimatedEmployerContribution] = ISNULL(ec.[EstimatedCost] - ec.[EstimatedReimbursement], 0)
FROM [dbo].[EligibleCosts] ec
	INNER JOIN [dbo].[TrainingPrograms] tp ON ec.[TrainingProgramId] = tp.[Id]
	INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id]

PRINT N'Update TrainingPrograms'

UPDATE tp
SET
	tp.[TotalEstimatedCost] = (SELECT ROUND(ISNULL(SUM(ec.[EstimatedCost]), 0), 2) FROM [dbo].[EligibleCosts] ec WHERE ec.[TrainingProgramId] = tp.[Id]),
	tp.[TotalEstimatedReimbursement] = (SELECT ROUND(ISNULL(SUM(ec.[EstimatedReimbursement]),0), 2) FROM [dbo].[EligibleCosts] ec WHERE ec.[TrainingProgramId] = tp.[Id]),
	tp.[TotalAgreedMaxCost] = (SELECT ROUND(ISNULL(SUM(ec.[AgreedMaxCost]),0), 2) FROM [dbo].[EligibleCosts] ec WHERE ec.[TrainingProgramId] = tp.[Id]),
	tp.[AgreedCommitment] = (SELECT ROUND(ISNULL(SUM(ec.[AgreedMaxReimbursement]),0), 2) FROM [dbo].[EligibleCosts] ec WHERE ec.[TrainingProgramId] = tp.[Id])
FROM [dbo].[TrainingPrograms] tp