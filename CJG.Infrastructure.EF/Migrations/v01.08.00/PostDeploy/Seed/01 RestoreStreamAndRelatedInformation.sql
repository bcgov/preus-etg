PRINT 'Restore Stream Information'

UPDATE gs
SET gs.[Name] = CAST(si.[Name] AS NVARCHAR(500)),
	gs.MaxReimbursementAmt = si.MaxReimbursementAmt,
	gs.ReimbursementRate = si.ReimbursementRate,
	gs.DefaultCancellationRate = si.DefaultCancellationRate,
	gs.DefaultDeniedRate = si.DefaultDeniedRate,
	gs.DefaultReductionRate = si.DefaultReductionRate,
	gs.DefaultSlippageRate = si.DefaultSlippageRate,
	gs.DefaultWithdrawnRate = si.DefaultWithdrawnRate
FROM dbo.GrantStreams gs
INNER JOIN #StreamInformation si on gs.Id = si.Id

PRINT 'Restore Stream Criteria Information'

UPDATE gs
SET gs.Criteria = CAST(s.[Description] AS NVARCHAR(2500))
FROM dbo.GrantStreams gs
INNER JOIN #StreamCritria s on gs.Id = s.Id

PRINT 'Restore Stream Objective Information'

UPDATE gs
SET gs.Objective = CAST(s.[Description] AS NVARCHAR(2500))
FROM dbo.GrantStreams gs
INNER JOIN #StreamObjective s on gs.Id = s.Id

DROP TABLE #StreamInformation
DROP TABLE #StreamCritria
DROP TABLE #StreamObjective