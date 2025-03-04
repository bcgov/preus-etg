PRINT 'Update [GrantOpeningFinancials]'

UPDATE gof
SET gof.[CurrentReservations] = ISNULL(
		(SELECT goi.[NewAmt] + goi.[PendingAssessmentAmt] + goi.[UnderAssessmentAmt] + goi.[CommitmentAmt] - goi.[ReductionsAmt] 
		FROM [dbo].[GrantOpeningIntakes] goi
		WHERE goi.[GrantOpeningId] = gof.[GrantOpeningId]), 0),
	gof.[AssessedCommitments] = ISNULL(
		(SELECT SUM(tp.[AgreedCommitment]) 
		FROM [dbo].[TrainingPrograms] tp
		INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id 
			AND (ga.[ApplicationStateInternal] = 9 OR ga.[ApplicationStateInternal] >= 16)
			AND ga.[GrantOpeningId] = gof.[GrantOpeningId]), 0),
	gof.[Cancellations] = ISNULL(
		(SELECT SUM(tp.[AgreedCommitment]) 
		FROM [dbo].[TrainingPrograms] tp
		INNER JOIN [dbo].[GrantApplications] ga ON tp.GrantApplicationId = ga.Id 
			AND ga.[ApplicationStateInternal] IN (14, 15)
			AND ga.[GrantOpeningId] = gof.[GrantOpeningId]), 0)
FROM [dbo].[GrantOpeningFinancials] gof

UPDATE [dbo].[GrantOpeningFinancials]
SET [OutstandingCommitments] = [AssessedCommitments]