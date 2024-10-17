PRINT 'Migrate [EmployerCompletionReportAnswers]'

UPDATE ecra
SET
	ecra.GrantApplicationId = becra.GrantApplicationId
FROM dbo.EmployerCompletionReportAnswers ecra
	INNER JOIN #EmployerCompletionReportAnswers becra ON ecra.EmployerEnrollmentId = becra.EmployerEnrollmentId
		AND ecra.QuestionId = becra.QuestionId

DROP TABLE #EmployerCompletionReportAnswers