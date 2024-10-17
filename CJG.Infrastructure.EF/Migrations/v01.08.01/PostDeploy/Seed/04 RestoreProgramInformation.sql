PRINT 'Restore Program Information'
UPDATE p
SET p.[ProgramCode] = t.[Abbreviation],
	p.[BatchRequestDescription] = t.[Description]
FROM dbo.GrantPrograms p
INNER JOIN #ProgramInformation t on p.id = t.id;