PRINT 'Updating [GrantOpeningFinancials]'

CREATE TABLE #GrantOpeningReservations (
	[GrantOpeningId] INT,
	[CurrentReservations] DECIMAL(18,2)
)

-- Current Reservation calculation for all grant applications selected for assessment up to issue offer.
INSERT INTO #GrantOpeningReservations ( [GrantOpeningId], [CurrentReservations] )
SELECT ga.[GrantOpeningId], SUM(tp.[TotalEstimatedReimbursement]) as [CurrentReservations]
FROM [dbo].[GrantApplications] ga
INNER JOIN [dbo].[TrainingPrograms] tp ON ga.Id = tp.GrantApplicationId
WHERE ga.[ApplicationStateInternal] IN (2, 3, 4, 5, 6, 7)
GROUP BY ga.[GrantOpeningId]

-- 2 - PendingAssessment
-- 3 - UnderAssessment
-- 4 - ReturnedToAssessment
-- 5 - RecommendedForApproval
-- 6 - RecommendedForDenial
-- 7 - OfferIssued

UPDATE gof 
SET gof.[CurrentReservations] = gor.[CurrentReservations]
FROM [dbo].[GrantOpeningFinancials] gof
INNER JOIN #GrantOpeningReservations gor ON gof.[GrantOpeningId] = gor.[GrantOpeningId]

DROP TABLE #GrantOpeningReservations
