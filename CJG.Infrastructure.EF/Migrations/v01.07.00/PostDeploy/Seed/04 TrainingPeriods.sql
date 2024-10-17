PRINT 'Updating [TrainingPeriods]'

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