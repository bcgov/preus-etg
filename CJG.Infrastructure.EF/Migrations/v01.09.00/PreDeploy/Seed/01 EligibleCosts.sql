PRINT 'Backup [EligibleCosts]'

SELECT DISTINCT 
	ec.Id
	, ec.TrainingProgramId
	, tp.GrantApplicationId
INTO #EligibleCosts 
FROM dbo.EligibleCosts ec
	INNER JOIN dbo.TrainingPrograms tp on ec.TrainingProgramId = tp.Id