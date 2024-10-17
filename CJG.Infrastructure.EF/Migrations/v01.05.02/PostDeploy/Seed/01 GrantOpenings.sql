PRINT 'Updating [GrantOpenings]'

-- First update the time based on daylight savings time.
UPDATE g
SET g.[OpeningDate] = DATEADD(HOUR, (CASE DATEPART(MONTH, g.[OpeningDate]) WHEN 12 THEN 8 WHEN 1 THEN 8 WHEN 2 THEN 8 WHEN 3 THEN 8 ELSE 7 END), CAST(CAST(g.[OpeningDate] AS DATE) AS DATETIME)),
	g.[ClosingDate] = DATEADD(HOUR, (CASE DATEPART(MONTH, g.[ClosingDate]) WHEN 12 THEN 8 WHEN 1 THEN 8 WHEN 2 THEN 8 WHEN 3 THEN 8 ELSE 7 END), CAST(CAST(g.[ClosingDate] AS DATE) AS DATETIME))
FROM [dbo].[GrantOpenings] g

-- Now update the time to midnight with the appropriate offset.
UPDATE g
SET g.[ClosingDate] = DATEADD(SECOND, 59, DATEADD(MINUTE, 1439, TODATETIMEOFFSET(g.[ClosingDate], '-0' + CAST(DATEPART(HOUR, g.[ClosingDate]) AS NVARCHAR) + ':00')))
FROM [dbo].[GrantOpenings] g