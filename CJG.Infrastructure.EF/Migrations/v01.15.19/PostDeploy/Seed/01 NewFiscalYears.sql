PRINT 'Inserting [FiscalYears] - START'
DECLARE @Incrementor INT
DECLARE @IntValue INT
DECLARE @IntFYStartingId INT
DECLARE @DTStartingYear datetime2
DECLARE @IntYearDifference INT
DECLARE @IntYearEnd INT
DECLARE @FYToAdd INT

SET @Incrementor = 0
SET @IntFYStartingId = (SELECT MAX(Id) FROM [dbo].[FiscalYears])
SET @DTStartingYear = (SELECT TOP 1 EndDate FROM [dbo].[FiscalYears] ORDER BY StartDate DESC)
SET @IntYearDifference =  (SELECT DATEDIFF(YEAR, GETUTCDATE(), @DTStartingYear))
SET @IntYearEnd = 2040
SET @FYToAdd = @IntYearEnd - (DATEPART(YEAR, GETUTCDATE()) + @IntYearDifference) - 1

PRINT @IntFYStartingId
PRINT 'Years in difference (Last available FY vs. current year): ' PRINT @IntYearDifference
PRINT 'Number of FYs to add: ' PRINT @FYToAdd

IF @IntYearDifference <= 5
	BEGIN
		SET IDENTITY_INSERT [dbo].[FiscalYears] ON
		WHILE @Incrementor <= @FYToAdd
			BEGIN
				INSERT [dbo].[FiscalYears]
				 ([Id], [Caption], [NextAgreementNumber], [StartDate], [EndDate], [DateAdded]) VALUES
				 (@IntFYStartingId + @Incrementor + 1,
				 CAST(DATEPART(YEAR, GETUTCDATE()) + (@IntYearDifference + @Incrementor) AS VARCHAR) + '/' + CAST(DATEPART(YEAR, GETUTCDATE()) + (@IntYearDifference + @Incrementor + 1) AS VARCHAR),
				 50000,
				 CAST(CAST((DATEPART(YEAR, GETUTCDATE()) + @IntYearDifference + @Incrementor) AS VARCHAR) + N'-04-01T07:00:00.000' AS DateTime),
				 CAST(CAST((DATEPART(YEAR, GETUTCDATE()) + @IntYearDifference + @Incrementor + 1) AS VARCHAR) + N'-03-31T06:59:59.000' AS DateTime),
				 GETUTCDATE())

				 SET @Incrementor = @Incrementor + 1
			END
		SET IDENTITY_INSERT [dbo].[FiscalYears] OFF
	END

PRINT 'Inserting [FiscalYears] - END'

PRINT 'Updating [TrainingPeriods] - START'

SET NOCOUNT ON;

DECLARE @fiscal_year_id int, @fiscal_caption VARCHAR(50), @start_date DATETIME, @end_date DATETIME

DECLARE fiscal_year_cursor CURSOR FOR
SELECT DISTINCT fy.Id, fy.Caption, fy.StartDate, fy.EndDate
FROM dbo.FiscalYears fy
	LEFT JOIN dbo.TrainingPeriods tp
		ON fy.Id = tp.FiscalYearId
WHERE tp.Id IS NULL

OPEN fiscal_year_cursor

FETCH NEXT FROM fiscal_year_cursor
INTO @fiscal_year_id, @fiscal_caption, @start_date, @end_date

WHILE @@FETCH_STATUS = 0
BEGIN

	DECLARE @fiscal_first_year VARCHAR(4)
	DECLARE @fiscal_second_year VARCHAR(4)
	SELECT @fiscal_first_year = CONVERT(CHAR(4), DATEPART(year, @start_date)), @fiscal_second_year = CONVERT(CHAR(4), DATEPART(year, @end_date))

	-- Insert TrainingPeriods for the FiscalYears.
	INSERT INTO dbo.[TrainingPeriods] (
		[Caption]
		,[StartDate]
		,[EndDate]
		,[DefaultPublishDate]
		,[DefaultOpeningDate]
		,[FiscalYearId]
		,[DateAdded]
	) VALUES (
		'Intake Period 1', CAST(@fiscal_first_year + '-04-01 07:00:00' AS DATETIME), CAST(@fiscal_first_year + '-09-01 06:59:00' AS DATETIME), CAST(@fiscal_first_year + '-01-01 08:00:00' AS DATETIME), CAST(@fiscal_first_year + '-02-01 08:00:00' AS DATETIME), @fiscal_year_id, GETUTCDATE()
	), (
		'Intake Period 2', CAST(@fiscal_first_year + '-09-01 07:00:00' AS DATETIME), CAST(@fiscal_second_year + '-01-01 07:59:00' AS DATETIME), CAST(@fiscal_first_year + '-03-01 08:00:00' AS DATETIME), CAST(@fiscal_first_year + '-04-01 07:00:00' AS DATETIME), @fiscal_year_id, GETUTCDATE()
	), (
		'Intake Period 3', CAST(@fiscal_second_year + '-01-01 08:00:00' AS DATETIME), CAST(@fiscal_second_year + '-04-01 06:59:00' AS DATETIME), CAST(@fiscal_first_year + '-09-01 07:00:00' AS DATETIME), CAST(@fiscal_first_year + '-10-01 07:00:00' AS DATETIME), @fiscal_year_id, GETUTCDATE()
	)

	PRINT 'Inserted [TrainingPeriods] for ID = ' + CONVERT(VARCHAR(2), @fiscal_year_id) + ' and Caption = ' + @fiscal_caption

	FETCH NEXT FROM fiscal_year_cursor
		INTO @fiscal_year_id, @fiscal_caption, @start_date, @end_date

END
CLOSE fiscal_year_cursor;
DEALLOCATE fiscal_year_cursor;

PRINT 'Updating [TrainingPeriods] - END'