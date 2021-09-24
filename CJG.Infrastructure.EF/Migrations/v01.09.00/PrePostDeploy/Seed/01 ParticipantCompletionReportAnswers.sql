PRINT 'Migrate [ParticipantCompletionReportAnswers]'

UPDATE pcra
SET
	pcra.GrantApplicationId = bpcra.GrantApplicationId
	, pcra.ParticipantFormId = bpcra.ParticipantFormId
FROM dbo.ParticipantCompletionReportAnswers pcra
	INNER JOIN #ParticipantCompletionReportAnswers bpcra ON pcra.ParticipantEnrollmentId = bpcra.ParticipantEnrollmentId

DROP TABLE #ParticipantCompletionReportAnswers