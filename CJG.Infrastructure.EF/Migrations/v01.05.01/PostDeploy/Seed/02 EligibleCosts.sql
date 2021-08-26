PRINT 'Updating [EligibleCosts]'

-- We didn't originally allow editing of Training costs in the New and PendingAssessment states.
-- We would copy the estimated costs into the agreed costs when they began assessment 'UnderAssessment'.
-- Now we're allowing editing and so we have to copy the estimated costs into the agreed costs on submit 'New'.
UPDATE ec
SET ec.[AgreedMaxParticipants] = ec.[EstimatedParticipants],
	ec.[AgreedMaxCost] = ec.[EstimatedCost],
	ec.[AgreedMaxParticipantCost] = ec.[EstimatedParticipantCost],
	ec.[AgreedEmployerContribution] = ec.[EstimatedEmployerContribution],
	ec.[AgreedMaxReimbursement] = ec.[EstimatedReimbursement]
FROM [dbo].[EligibleCosts] ec
INNER JOIN [dbo].[TrainingPrograms] tp ON ec.TrainingProgramId = tp.Id
INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id
WHERE ga.[ApplicationStateInternal] IN (1, 2)


PRINT 'Updating [TrainingPrograms]'

UPDATE tp
SET tp.[AgreedParticipants] = tp.[EstimatedParticipants],
	tp.[TotalAgreedMaxCost] = tp.[TotalEstimatedCost],
	tp.[AgreedCommitment] = tp.[TotalEstimatedReimbursement]
FROM [dbo].[TrainingPrograms] tp
INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id
WHERE ga.[ApplicationStateInternal] IN (1, 2)