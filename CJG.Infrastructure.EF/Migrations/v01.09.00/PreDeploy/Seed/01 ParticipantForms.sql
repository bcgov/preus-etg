PRINT 'Backup [ParticipantForms]'

SELECT 
	p.TrainingProgramId
	, tp.GrantApplicationId
INTO #ParticipantForms
FROM dbo.ParticipantForms p
	INNER JOIN dbo.TrainingPrograms tp ON p.TrainingProgramId = tp.Id

