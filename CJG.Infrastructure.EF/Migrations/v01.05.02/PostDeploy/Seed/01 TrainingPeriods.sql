PRINT 'Updating [TrainingPeriods]'

-- First update the time based on daylight savings time.
UPDATE tp
SET tp.[EndDate] = DATEADD(HOUR, (CASE DATEPART(MONTH, tp.[EndDate]) WHEN 12 THEN 8 WHEN 1 THEN 8 WHEN 2 THEN 8 WHEN 3 THEN 8 ELSE 7 END), CAST(CAST(tp.[EndDate] AS DATE) AS DATETIME))
FROM [dbo].[TrainingPeriods] tp

-- Now update the time to midnight with the appropriate offset.
UPDATE tp
SET tp.[EndDate] = DATEADD(SECOND, 59, DATEADD(MINUTE, 1439, TODATETIMEOFFSET(tp.[EndDate], '-0' + CAST(DATEPART(HOUR, tp.[EndDate]) AS NVARCHAR) + ':00')))
FROM [dbo].[TrainingPeriods] tp