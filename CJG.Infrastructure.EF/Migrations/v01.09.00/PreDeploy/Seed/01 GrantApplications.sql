PRINT 'Backup [GrantApplications]'

SELECT DISTINCT 
	ga.Id
	, tp.StartDate
	, tp.EndDate
	, ee.InvitationKey
	, ee.InvitationExpiresOn
	, ee.TrainingProgramId
	, tp.HoldPaymentRequests
INTO #GrantApplications
FROM dbo.GrantApplications ga 
	INNER JOIN dbo.TrainingPrograms tp on ga.Id = tp.GrantApplicationId
	INNER JOIN dbo.EmployerEnrollments ee on ee.TrainingProgramId = tp.Id