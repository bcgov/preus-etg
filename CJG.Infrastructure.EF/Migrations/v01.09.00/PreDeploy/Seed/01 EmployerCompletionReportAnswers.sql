PRINT 'Backup EmployerCompletionReportAnswers'

SELECT 
	e.EmployerEnrollmentId
	, tp.GrantApplicationId
	, e.QuestionId
INTO #EmployerCompletionReportAnswers
FROM dbo.EmployerCompletionReportAnswers e
	INNER JOIN dbo.EmployerEnrollments ee ON e.EmployerEnrollmentId = ee.Id
	INNER JOIN dbo.TrainingPrograms tp ON ee.TrainingProgramId = tp.Id

