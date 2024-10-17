PRINT 'Backup Grant Program Information'
SELECT DISTINCT 
	p.[Id],
	p.[Abbreviation],
	p.[Description]
INTO #ProgramInformation
FROM dbo.GrantPrograms p