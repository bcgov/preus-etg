PRINT 'Backup [TrainingProviders]'

SELECT
	tp.Id
	, tp.GrantApplicationId
	, tp.TrainingProgramId
	, tp.TrainingProviderState
INTO #TrainingProviders
FROM dbo.TrainingProviders tp 