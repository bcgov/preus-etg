PRINT 'Backup Stream Information'

SELECT DISTINCT 
	gs.[Id],
	s.[Name],
	s.MaxReimbursementAmt,
	s.ReimbursementRate,
	s.DefaultCancellationRate,
	s.DefaultDeniedRate,
	s.DefaultReductionRate,
	s.DefaultSlippageRate,
	s.DefaultWithdrawnRate
INTO #StreamInformation
FROM dbo.Streams s
INNER JOIN dbo.GrantStreams gs on s.Id = gs.StreamId

PRINT 'Backup Stream Critria'

SELECT DISTINCT 
	gs.[Id],
	s.[Description]
INTO #StreamCritria
From dbo.StreamCriterias s
INNER JOIN dbo.GrantStreams gs on gs.StreamCriteriaId = s.Id

PRINT 'Backup Stream Objective'

SELECT DISTINCT
	gs.[Id],
	s.[Description]
INTO #StreamObjective
From dbo.StreamObjectives s
INNER JOIN dbo.GrantStreams gs on gs.StreamObjectiveId = s.Id
