PRINT 'Updating [FiscalYears]'

-- insert an entry for the current fiscal year if it doesn't exist using the incorrect end date which will then get updated in the next step
IF NOT EXISTS(
	SELECT 1 
	FROM [dbo].[FiscalYears] 
	WHERE [StartDate] = CAST(CAST(DATEPART(YEAR, GETDATE()) - 1 AS VARCHAR) + '-04-01 07:00:00.0000000' AS DATETIME2)
		AND [EndDate] = CAST(CAST(DATEPART(YEAR, GETDATE()) AS VARCHAR) + '-03-31 07:00:00.0000000' AS DATETIME2)
	)
BEGIN
	INSERT INTO [dbo].[FiscalYears] (
		[Caption]
		,[StartDate]
		,[EndDate]
		,[NextAgreementNumber]
		,[DateAdded]
	) VALUES (
		'FY' + CAST(DATEPART(YEAR, GETDATE()) - 1 AS VARCHAR) + '/' + SUBSTRING(CAST(DATEPART(YEAR, GETDATE()) AS VARCHAR), 3, 2)
		, CAST(DATEPART(YEAR, GETDATE()) - 1 AS VARCHAR) + '-04-01 07:00:00.0000000'
		, CAST(DATEPART(YEAR, GETDATE()) AS VARCHAR) + '-03-31 07:00:00.0000000'
		, 50000
		, getdate()
	)
END

-- change 31-03T07:00:00 UTC to 01-04T06:59:59 UTC
UPDATE [dbo].[FiscalYears] 
SET [EndDate] = DATEADD(second, -1, DATEADD(day, 1, CONVERT(DATETIME, [EndDate])))

