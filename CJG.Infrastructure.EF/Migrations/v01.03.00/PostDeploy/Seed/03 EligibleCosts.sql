PRINT 'Updating [EligibleCosts]'

-- Updating the calculated values with the updated reimbursement rates.

UPDATE ec
SET ec.[EstimatedReimbursement] = ROUND(ec.[EstimatedCost] * ga.[ReimbursementRate], 2),
	ec.[EstimatedEmployerContribution] = ec.[EstimatedCost] - ROUND(ec.[EstimatedCost] * ga.[ReimbursementRate], 2)
FROM [dbo].[EligibleCosts] ec
INNER JOIN [dbo].[TrainingPrograms] tp ON ec.[TrainingProgramId] = tp.[Id]
INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id]

PRINT 'Updating [TrainingPrograms]'

-- Updating the calculated values with the updated reimbursement amounts.

UPDATE tp
SET tp.[TotalEstimatedReimbursement] = g.[TotalEstimatedReimbursement]
FROM [dbo].[TrainingPrograms] tp
INNER JOIN (
	SELECT ec.[TrainingProgramId], 
		SUM(ec.[EstimatedReimbursement]) AS [TotalEstimatedReimbursement]
	FROM [dbo].[EligibleCosts] ec 
	GROUP BY ec.[TrainingProgramId]
) g ON tp.[Id] = g.[TrainingProgramId]
