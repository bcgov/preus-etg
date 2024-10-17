IF OBJECT_ID('tempdb..#DaylightSaving') IS NOT NULL
	DROP TABLE #DaylightSaving

IF OBJECT_ID('tempdb..#GrantOpeningUpdates') IS NOT NULL
	DROP TABLE #GrantOpeningUpdates

SELECT 2018 AS [DSYear], CONVERT(DateTime, '2018-03-11') AS DSStart, CONVERT(DateTime, '2018-11-04') AS DSEnd INTO #DaylightSaving
INSERT INTO #DaylightSaving
VALUES
(2019, CONVERT(DateTime, '2019-03-10'), CONVERT(DateTime, '2019-11-03')),
(2020, CONVERT(DateTime, '2020-03-08'), CONVERT(DateTime, '2020-11-01')),
(2021, CONVERT(DateTime, '2021-03-14'), CONVERT(DateTime, '2021-11-07')),
(2022, CONVERT(DateTime, '2022-03-13'), CONVERT(DateTime, '2022-11-06')),
(2023, CONVERT(DateTime, '2023-03-12'), CONVERT(DateTime, '2023-11-05')),
(2024, CONVERT(DateTime, '2024-03-10'), CONVERT(DateTime, '2024-11-03')),
(2025, CONVERT(DateTime, '2025-03-09'), CONVERT(DateTime, '2025-11-02')),
(2026, CONVERT(DateTime, '2026-03-08'), CONVERT(DateTime, '2026-11-01')),
(2027, CONVERT(DateTime, '2027-03-14'), CONVERT(DateTime, '2027-11-07')),
(2028, CONVERT(DateTime, '2028-03-12'), CONVERT(DateTime, '2028-11-05')),
(2029, CONVERT(DateTime, '2029-03-11'), CONVERT(DateTime, '2029-11-04'))

SELECT
	GrantOpenings.Id,
	DATEADD(hour, 24 + (CASE WHEN GrantOpenings.OpeningDate BETWEEN (SELECT DSStart FROM #DaylightSaving ds WHERE ds.DSYear = DATEPART(year, GrantOpenings.OpeningDate)) AND (SELECT DSEnd FROM #DaylightSaving ds WHERE ds.DSYear = DATEPART(year, GrantOpenings.OpeningDate)) THEN 7 ELSE 8 END), DATEADD(day, -1, CAST(FLOOR(CAST(CAST(GrantOpenings.OpeningDate AS DateTime) AS Float)) AS DateTime))) AS UTCStartDate,
	DATEADD(minute, -1 , DATEADD(hour, 24 + (CASE WHEN GrantOpenings.ClosingDate BETWEEN (SELECT DSStart FROM #DaylightSaving ds WHERE ds.DSYear = DATEPART(year, GrantOpenings.ClosingDate)) AND (SELECT DSEnd FROM #DaylightSaving ds WHERE ds.DSYear = DATEPART(year, GrantOpenings.ClosingDate)) THEN 7 ELSE 8 END), DATEADD(day, -1, CAST(FLOOR(CAST(CAST(GrantOpenings.ClosingDate AS DateTime) AS Float)) AS DateTime)))) AS UTCEndDate
INTO #GrantOpeningUpdates
FROM GrantOpenings

UPDATE goTable
	SET
		goTable.OpeningDate = gou.UTCStartDate,
		goTable.ClosingDate = gou.UTCEndDate
FROM GrantOpenings goTable
	INNER JOIN #GrantOpeningUpdates gou
		ON goTable.Id = gou.Id

IF OBJECT_ID('tempdb..#DaylightSaving') IS NOT NULL
	DROP TABLE #DaylightSaving

IF OBJECT_ID('tempdb..#TrainingPeriodUpdates') IS NOT NULL
	DROP TABLE #GrantOpeningUpdates