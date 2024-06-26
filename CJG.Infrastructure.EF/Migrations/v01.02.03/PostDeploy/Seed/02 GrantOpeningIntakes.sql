PRINT 'Update [GrantOpeningIntakes]'

-- New, Pending, UnderAssessment
UPDATE goi
SET [NewCount] = ISNULL((SELECT COUNT(*) FROM [dbo].[GrantApplications] WHERE [ApplicationStateInternal] = 1 AND [GrantOpeningId] = goi.[GrantOpeningId]), 0),
	[NewAmt] = ISNULL((SELECT SUM(tp.[TotalEstimatedReimbursement]) 
				FROM [dbo].[TrainingPrograms] tp
				INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id AND ga.[ApplicationStateInternal] = 1 AND ga.[GrantOpeningId] = goi.[GrantOpeningId]), 0),

	[PendingAssessmentCount] = ISNULL((SELECT COUNT(*) FROM [dbo].[GrantApplications] WHERE [ApplicationStateInternal] = 2 AND [GrantOpeningId] = goi.[GrantOpeningId]), 0),
	[PendingAssessmentAmt] = ISNULL((SELECT SUM(tp.[TotalEstimatedReimbursement]) 
				FROM [dbo].[TrainingPrograms] tp
				INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id AND ga.[ApplicationStateInternal] = 2 AND ga.[GrantOpeningId] = goi.[GrantOpeningId]), 0),

	[UnderAssessmentCount] = ISNULL((SELECT COUNT(*) FROM [dbo].[GrantApplications] WHERE [ApplicationStateInternal] = 3 AND [GrantOpeningId] = goi.[GrantOpeningId]), 0),
	[UnderAssessmentAmt] = ISNULL((SELECT SUM(tp.[TotalEstimatedReimbursement]) 
				FROM [dbo].[TrainingPrograms] tp
				INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id AND ga.[ApplicationStateInternal] = 3 AND ga.[GrantOpeningId] = goi.[GrantOpeningId]), 0)
FROM [dbo].[GrantOpeningIntakes] goi
