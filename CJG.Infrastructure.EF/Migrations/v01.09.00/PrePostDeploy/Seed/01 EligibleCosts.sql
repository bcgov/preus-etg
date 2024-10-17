PRINT 'Migrate EligibleCosts'

UPDATE ec
SET
	ec.GrantApplicationId = bec.GrantApplicationId
FROM dbo.EligibleCosts ec
	INNER JOIN #EligibleCosts bec ON ec.Id = bec.Id

DROP TABLE #EligibleCosts