PRINT 'Backup [Claims]'

SELECT DISTINCT 
	c.Id
	, c.ClaimVersion
	, c.TrainingProgramId
	, tp.GrantApplicationId
INTO #Claims
FROM dbo.Claims c 
	INNER JOIN dbo.TrainingPrograms tp ON c.TrainingProgramId = tp.Id
