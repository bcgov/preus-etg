PRINT 'Migrate [ParticipantForms]'

UPDATE pf
SET
	pf.GrantApplicationId = bpf.GrantApplicationId
FROM dbo.ParticipantForms pf
	INNER JOIN #ParticipantForms bpf ON pf.TrainingProgramId = bpf.TrainingProgramId

DROP TABLE #ParticipantForms