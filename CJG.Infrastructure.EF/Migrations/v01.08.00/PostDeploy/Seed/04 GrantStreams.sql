PRINT 'Updating [GrantStreams]'

-- Update the GrantStreams that are in the 2017 Fiscal Year to use the new CJG GrantProgram.
UPDATE gs
SET [GrantProgramId] = 1
	, [AccountCodeId] = 1
FROM dbo.[GrantStreams] gs
INNER JOIN dbo.[GrantOpenings] g ON gs.Id = g.GrantStreamId
INNER JOIN dbo.[TrainingPeriods] tp ON g.[TrainingPeriodId] = tp.[Id]
INNER JOIN dbo.[FiscalYears] fy ON tp.[FiscalYearId] = fy.[Id]
WHERE DATEPART(YEAR, fy.[StartDate]) <= 2017

-- Update the GrantStreams that are in the 2018 Fiscal Year to use the new ETG GrantProgram.
UPDATE gs
SET [GrantProgramId] = 2
	, [AccountCodeId] = 2
FROM dbo.[GrantStreams] gs
INNER JOIN dbo.[GrantOpenings] g ON gs.Id = g.GrantStreamId
INNER JOIN dbo.[TrainingPeriods] tp ON g.[TrainingPeriodId] = tp.[Id]
INNER JOIN dbo.[FiscalYears] fy ON tp.[FiscalYearId] = fy.[Id]
WHERE DATEPART(YEAR, fy.[StartDate]) >= 2018