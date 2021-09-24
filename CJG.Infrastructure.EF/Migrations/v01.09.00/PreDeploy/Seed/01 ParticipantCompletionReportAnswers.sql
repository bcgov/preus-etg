PRINT 'Backup [ParticipantCompletionReportAnswers]'

SELECT 
	p.ParticipantEnrollmentId
	, tp.GrantApplicationId
	, pe.ParticipantFormId
INTO #ParticipantCompletionReportAnswers
FROM dbo.ParticipantCompletionReportAnswers p
	INNER JOIN dbo.ParticipantEnrollments pe ON p.ParticipantEnrollmentId = pe.Id
	INNER JOIN dbo.EmployerEnrollments ee ON pe.EmployerEnrollmentId = ee.Id
	INNER JOIN dbo.TrainingPrograms tp ON ee.TrainingProgramId = tp.Id

