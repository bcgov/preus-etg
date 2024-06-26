PRINT 'Updating [TrainingPrograms]'

-- Set the default Start and End dates
UPDATE tp
SET tp.[StartDate] = p.[StartDate],
	tp.[EndDate] = p.[EndDate]
FROM [dbo].[TrainingPrograms] tp
	INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id]
	INNER JOIN [dbo].[GrantOpenings] [go] ON ga.[GrantOpeningId] = [go].[Id]
	INNER JOIN [dbo].[TrainingPeriods] p ON [go].[TrainingPeriodId] = p.[Id]

-- Randomize the Training Start dates to occur within the training period.
UPDATE tp
SET tp.[StartDate] = DATEADD(DAY, (RAND(tp.[Id]*1000)*(60-1)+1), tp.[StartDate])
FROM [dbo].[TrainingPrograms] tp
	INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id]
	INNER JOIN [dbo].[GrantOpenings] [go] ON ga.[GrantOpeningId] = [go].[Id]
	INNER JOIN [dbo].[TrainingPeriods] p ON [go].[TrainingPeriodId] = p.[Id]

-- Randomize the Training End dates to occur any day after the Start day.
UPDATE tp
SET tp.[EndDate] = DATEADD(DAY, (RAND(tp.[Id]*1000)*(100-1)+1), tp.[StartDate])
FROM [dbo].[TrainingPrograms] tp
	INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id]
	INNER JOIN [dbo].[GrantOpenings] [go] ON ga.[GrantOpeningId] = [go].[Id]
	INNER JOIN [dbo].[TrainingPeriods] p ON [go].[TrainingPeriodId] = p.[Id]


SELECT tp.StartDate, tp.EndDate, p.StartDate, p.EndDate
FROM dbo.TrainingPrograms tp
	INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
	INNER JOIN dbo.GrantOpenings g ON ga.GrantOpeningId = g.Id
	INNER JOIN dbo.TrainingPeriods p ON g.TrainingPeriodId = p.Id