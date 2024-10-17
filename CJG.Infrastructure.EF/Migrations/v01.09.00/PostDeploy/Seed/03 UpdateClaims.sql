PRINT 'UPDATE [Claims]'

UPDATE c
SET 
	c.ClaimTypeId = pc.ClaimTypeId 
FROM dbo.Claims c 
	JOIN dbo.GrantApplications ga on ga.Id = c.GrantApplicationId
	JOIN dbo.GrantOpenings g on g.Id = ga.GrantOpeningId
	JOIN dbo.GrantStreams gs on gs.Id = g.GrantStreamId
	JOIN dbo.ProgramConfigurations pc on pc.Id = gs.ProgramConfigurationId