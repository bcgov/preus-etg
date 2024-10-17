SET NOCOUNT ON;

/*
STEP 1:
Add new active training periods with associated stream based on current training periods.
*/
PRINT 'STEP 01: Inserting [TrainingPeriods] - START'
DECLARE @training_period_id INT, @caption VARCHAR(250), @start_date DATETIME2, @end_date DATETIME2, @default_publish_date DATETIME2, @default_opening_date DATETIME2, @fiscalyearId INT, @nextFYStartDate DATETIME2
SELECT @nextFYStartDate = CAST('2022-04-01' AS DATETIME2)

DECLARE training_periods_cursor CURSOR FOR
	SELECT DISTINCT tp.[Id], tp.[Caption], tp.[StartDate], tp.[EndDate], tp.[DefaultPublishDate], tp.[DefaultOpeningDate], tp.[FiscalYearId]
	FROM dbo.TrainingPeriods tp
	ORDER BY FiscalYearId ASC

OPEN training_periods_cursor

FETCH NEXT FROM training_periods_cursor
INTO @training_period_id, @caption, @start_date, @end_date, @default_publish_date, @default_opening_date, @fiscalyearId

WHILE @@FETCH_STATUS = 0
BEGIN

		;WITH cte_GrantStreams AS(
			select s.Id [GrantStreamId], s.GrantProgramId, s.[Name]
			from [dbo].GrantStreams s
			where s.IsActive = 1
		)
		-- Insert TrainingPeriods per stream for legacy and future(beyond 2022) CWRG only
		INSERT INTO dbo.[TrainingPeriods] ([Caption], [StartDate], [EndDate], [DefaultPublishDate], [DefaultOpeningDate], [FiscalYearId], [GrantStreamId], [DateAdded], [IsActive])
		SELECT @caption, @start_date, @end_date, @default_publish_date, @default_opening_date, @fiscalyearId, GrantStreamId, GETUTCDATE(), 1
		FROM cte_GrantStreams
		WHERE NOT (cte_GrantStreams.GrantProgramId = 2 AND @start_date >= @nextFYStartDate)

		PRINT 'Records Count = ' + (STR(@@ROWCOUNT,2))
		PRINT 'Inserted [TrainingPeriods] for Fiscal Year ID = ' + (STR(@fiscalyearId,2)) + ' and Training Period ID = ' + (STR(@training_period_id,2)) + ' and Caption = ' + @caption  + ' and Start Date = ' + FORMAT(@start_date,'yyyy-MM-dd')

		FETCH NEXT FROM training_periods_cursor
			INTO @training_period_id, @caption, @start_date, @end_date, @default_publish_date, @default_opening_date, @fiscalyearId
END
CLOSE training_periods_cursor;
DEALLOCATE training_periods_cursor;

PRINT 'Inserting [TrainingPeriods] - END'

/*
STEP 2:
Map existing Grand Openings to new Training Periods based on streams
*/

PRINT 'STEP 02: Remapping Training Period Id for existing [GrandOpenings] - START'
BEGIN
	DECLARE @go_training_period_id INT, @trainingperiod_fiscalyearId INT, @trainingperiod_caption varchar(50)

	DECLARE go_trainingperiods_cursor CURSOR FOR
	SELECT DISTINCT o.TrainingPeriodId
		FROM dbo.[GrantOpenings] o
		ORDER BY TrainingPeriodId ASC

	OPEN go_trainingperiods_cursor

	FETCH NEXT FROM go_trainingperiods_cursor
		INTO @go_training_period_id

	WHILE @@FETCH_STATUS = 0
	BEGIN

		SELECT @trainingperiod_fiscalyearId = FiscalYearId , @trainingperiod_caption = Caption FROM TrainingPeriods WHERE Id = @go_training_period_id

		;WITH cte_GrantOpenings AS(
			select [Id] [GrantOpeningId], [State], [TrainingPeriodId], [GrantStreamId], [DateUpdated], o.[OpeningDate]
			from [dbo].GrantOpenings o
			where [TrainingPeriodId] = @go_training_period_id
		)
		UPDATE goo
		SET goo.TrainingPeriodId = src.Id
		FROM cte_GrantOpenings goo
		INNER JOIN
		(
			select tp.Id, tp.GrantStreamId
			from TrainingPeriods tp
			where tp.FiscalYearId = @trainingperiod_fiscalyearId
			and Caption = @trainingperiod_caption
			and tp.IsActive = 1
			and tp.GrantStreamId IN (
					select [GrantStreamId]
					from [dbo].GrantOpenings o
					where [TrainingPeriodId] = @go_training_period_id
			)
		) src
			ON goo.GrantStreamId = src.GrantStreamId;

		PRINT 'Records Count = ' + (STR(@@ROWCOUNT,2))
		PRINT 'Updated GrantOpenings for Training Period Id = ' + (STR(@go_training_period_id,2))

		FETCH NEXT FROM go_trainingperiods_cursor
			INTO @go_training_period_id

	END
	CLOSE go_trainingperiods_cursor;
	DEALLOCATE go_trainingperiods_cursor;
END

PRINT 'Remapping [GrandOpenings] - END'