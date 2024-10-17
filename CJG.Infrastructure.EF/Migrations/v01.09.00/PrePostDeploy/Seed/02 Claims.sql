PRINT 'Migrate [Claims]'

UPDATE c
SET
	c.GrantApplicationId = bc.GrantApplicationId
	, c.ClaimTypeId = 1
FROM dbo.Claims c
	INNER JOIN #Claims bc ON c.Id = bc.Id AND c.ClaimVersion = bc.ClaimVersion

DROP TABLE #Claims